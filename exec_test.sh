docker-compose down -v && rm -rf data/*

docker-compose up -d

#docker-compose exec database sh -c '/opt/mssql-tools/bin/sqlcmd -S localhost -U $BABELFISH_USER -P $BABELFISH_PASSWORD -Q "CREATE DATABASE pocaurora"'

#PENDENTE EXECUÇÃO DA MIGRATION.
### OBS: PODE SER QUE O "GO" DO SCRIPT DE MIGRATION ESTEJA FERRANDO A EXECUÇÃO
echo '## Preparando banco ##'
dotnet test poc_dotnet/PocAurora.sln
echo '## Encerrando teste ##'
#docker-compose down