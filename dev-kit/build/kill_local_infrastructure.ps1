docker-compose -f .\build\deploy\local\docker-compose.infrastructure.yaml --env-file .\build\deploy\local\infrastructure.env down

# Run command below to reinit postgres data for next start
# docker volume rm local_dev-kit-postgres-db

# local_dev-kit-redis
# local_dev-kit-pgadmin