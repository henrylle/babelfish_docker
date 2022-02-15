

# start socket-only postgresql server for setting up or running scripts
# all arguments will be passed along as arguments to `postgres` (via pg_ctl)
docker_temp_server_start() {
	if [ "$1" = 'postgres' ]; then
		shift
	fi

	# internal start of server in order to allow setup using psql client
	# does not listen on external TCP/IP and waits until start finishes
	set -- "$@" -c listen_addresses='' -p "${PGPORT:-5432}"

	$INSTALLATION_PATH/bin/pg_ctl -D "$PGDATA" start
}

# check to see if this file is being run or sourced from another script
_is_sourced() {
	# https://unix.stackexchange.com/a/215279
	[ "${#FUNCNAME[@]}" -ge 2 ] \
		&& [ "${FUNCNAME[0]}" = '_is_sourced' ] \
		&& [ "${FUNCNAME[1]}" = 'source' ]
}

docker_create_db_directories() {
  local user; user="$(id -u)"  
  mkdir -p "$PGDATA"
  # ignore failure since there are cases where we can't chmod (and PostgreSQL might fail later anyhow - it's picky about permissions of this directory)
	chmod 700 "$PGDATA" || :
  
	# allow the container to be started with `--user`
	if [ "$user" = '0' ]; then
		find "$PGDATA" \! -user postgres -exec chown postgres '{}' +
		find /var/run/postgresql \! -user postgres -exec chown postgres '{}' +
	fi
}

docker_setup_env() {
  DATABASE_ALREADY_EXISTS=''  
	# look specifically for PG_VERSION, as it is expected in the DB dir    
	if [ -s "$PGDATA/PG_VERSION" ]; then  
  echo "$PGDATA/PG_VERSION"
		DATABASE_ALREADY_EXISTS='true'
	fi  
}

initdb(){  
  $INSTALLATION_PATH/bin/initdb -D $PGDATA    
}

docker_temp_server_stop(){
  $INSTALLATION_PATH/bin/pg_ctl -D "$PGDATA" -m fast -w stop  
}

pg_setup_pg_conf(){
  cp /usr/local/pg_conf/postgresql.conf /usr/local/pgsql/data/
  cp /usr/local/pg_conf/pg_hba.conf /usr/local/pgsql/data/
}

setup_postgres_password(){
  $INSTALLATION_PATH/bin/psql -c "ALTER USER postgres PASSWORD '$POSTGRES_PASSWORD';"
}

setup_babelfish_db(){
  #Create a user that will own the sample database
   $INSTALLATION_PATH/bin/psql -c "CREATE USER $BABELFISH_USER WITH CREATEDB \
	  CREATEROLE PASSWORD '$BABELFISH_PASSWORD' INHERIT;"

  $INSTALLATION_PATH/bin/psql -c "DROP DATABASE IF EXISTS $BABELFISH_DB;"

  
  $INSTALLATION_PATH/bin/psql -c "CREATE DATABASE $BABELFISH_DB OWNER $BABELFISH_USER;"
  
  #Rodando comandos no banco que acabei de criar  
  $INSTALLATION_PATH/bin/psql -U postgres -d $BABELFISH_DB -c "CREATE EXTENSION IF NOT EXISTS \"babelfishpg_tds\" CASCADE;"  
  $INSTALLATION_PATH/bin/psql -U postgres -d $BABELFISH_DB -c "GRANT ALL ON SCHEMA sys to $BABELFISH_USER;"
  $INSTALLATION_PATH/bin/psql -U postgres -d $BABELFISH_DB -c "ALTER SYSTEM SET babelfishpg_tsql.database_name = \"$BABELFISH_DB\";"  
  $INSTALLATION_PATH/bin/psql -U postgres -d $BABELFISH_DB -c "ALTER SYSTEM SET babelfishpg_tds.set_db_session_property = true;"
  $INSTALLATION_PATH/bin/psql -U postgres -d $BABELFISH_DB -c "ALTER DATABASE babelfish_db SET babelfishpg_tsql.migration_mode = \"$MIGRATION_MODE\";"
  $INSTALLATION_PATH/bin/psql -U postgres -d $BABELFISH_DB -c "SELECT pg_reload_conf();"
  $INSTALLATION_PATH/bin/psql -U postgres -d $BABELFISH_DB -c "CALL SYS.INITIALIZE_BABELFISH('$BABELFISH_USER');"  
}

_main(){  

  docker_setup_env

  #Caso pgdata nao exista ainda
  if [ -z "$DATABASE_ALREADY_EXISTS" ]; then     
    initdb
    pg_setup_pg_conf  
  fi

  docker_temp_server_start
  
  setup_postgres_password
  setup_babelfish_db
  
  docker_temp_server_stop
  # docker_temp_server_start
  # /usr/local/pgsql-13.4/bin/psql -c "ALTER USER postgres PASSWORD 'postgres';"
  # docker_temp_server_stop

  $INSTALLATION_PATH/bin/postgres
}

_main







#Setando senha para postgres
# sleep 10
# /usr/local/pgsql-13.4/bin/psql -c "ALTER USER postgres PASSWORD 'postgres';"

#Setando senha para babelfish_user
#/usr/local/pgsql-13.4/bin/psql -c "ALTER USER babelfish_user PASSWORD 'babelfish';"