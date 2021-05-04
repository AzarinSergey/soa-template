# Infrastructure
<p>How to publish required third party services to run application </p>

## Local 
Local infrastructure was prepared described in [compose file](../build/deploy/local/docker-compose.infrastructure.yaml) and that used [environment](../build/deploy/local/infrastructure.env) variables. It contains now:

 - Postgres database server and pgadmin UI ([how it works](infrastructure_postgres.md))
 - Redis server and UI 

Here using powershell script to start local services throught 'docker-compose .. up -d' command. 

</br>

#### __Start__:
<p>To run infrastructure open powershell, go to '/dev-kit' directory and execute script: </p>

```
.\build\run_local_infrastructure.ps1
```

#### __Stop__:
<p>To stop and remove containers execute script below from same directory '/dev-kit' : </p>

```
.\build\kill_local_infrastructure.ps1
```

#### __Cleanup persistent values__:
<p>Containers use host machine for data persitance while restart/recreate. Follow below steps when defaults should be restored: </p>


- check existing docker volumes:
```
docker volume ls
```
- remove necessary by name witn 'docker volume rm {list of volumes}', example command:
```
docker volume rm local_dev-kit-pgadmin, local_dev-kit-postgres-db, local_dev-kit-redis
```