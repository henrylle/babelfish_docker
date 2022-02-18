# babelfish_docker

Project to build babelfish on docker and try migration journey from Sql Server database to Postgresql.

## Before to execute
- Update path to a valid on __volumes > data > device__.

## Steps to execute
```
docker-compose up
``` 

## Observations
 - First execution will be very slow because build steps (Dockerfile) will compile based on original source repository babelfish.
 - After env up with success to restart your data will be persisted, becasue if you need discard your env, and clean data referenced on  __volumes > data > device__.
  Ex:
  ```
  docker compose down -v
  rm -rf /home/henrylle/Projetos/babelfish/babelfish-fonte-oficial-by-henrylle/data/*
  ```
## Default data to connect on postgresql
  - Host: localhost
  - Port: :5432
  - Login: postgres
  - Password: postgres

## How to connect via terminal on container
  - docker-compose exec database sh

## How to start psql on container
  - /usr/local/pgsql-13.4/bin/psql


## Onde parei o processo

https://babelfishpg.org/docs/installation/single-multiple

Obs: Atentar para o comando "ALTER SYSTEM SET babelfishpg_tsql.database_name"
Ele só roda já estando no banco demo.