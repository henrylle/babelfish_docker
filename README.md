# babelfish_docker

Project to build babelfish on docker and try migration journey from Sql Server database to Postgresql.

## Before to execute
- Update path to a valid on __volumes > data > device__.

## Steps to execute
- docker-compose up

## Observations:
 First execution will be very slow because build steps (Dockerfile) will compile based on original source repository babelfish.


## Default data to connect on postgresql
  - Host: localhost
  - Port: :5432
  - Login: postgres
  - Password: postgres