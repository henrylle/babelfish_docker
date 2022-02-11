/usr/local/pgsql-13.4/bin/postgres -D /usr/local/pgsql/data
#Setando senha para postgres
/usr/local/pgsql-13.4/bin/psql -c "ALTER USER postgres PASSWORD 'postgres';"