docker-compose up -d
echo '## Preparando banco ##'
sleep 10
dotnet test
echo '## Encerrando teste ##'
docker-compose down