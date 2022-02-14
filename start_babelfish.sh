

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

  #Create a database named demo, owned by the above user */
  $INSTALLATION_PATH/bin/psql -c "CREATE DATABASE demo OWNER babelfish_user;"
 
  $INSTALLATION_PATH/bin/psql -c "ALTER SYSTEM SET babelfishpg_tsql.database_name = 'demo';"
  $INSTALLATION_PATH/bin/psql -c "SELECT pg_reload_conf();"
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