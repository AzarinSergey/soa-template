# Postgres database server
### Container publish process:
1. Container will start with persistent location on host machine for database data.
2. If container volume data is empty [script](/dev-kit/build/deploy/local/postgres/bash/server_config.sh) will be executed on server started. 
    - It create db user, tablespace and new database. 
    - [sample_db](/dev-kit/build/deploy/local/postgres/sample_db/dvdrental.tar) will have restored
    - [sql scripts](/dev-kit/build/deploy/local/postgres/sql) will have executed on __firstly__ created database.

# Postgres web client (pgadmin now)

It use pre defined config to create connections with existing server. [Check it](/dev-kit/build/deploy/local/postgres/pgadmin/servers.json) and use browser: 

- UI located at http://localhost:9822 ( Very slow on container start, please bear with it to use )
- UI Login/pass: admin@admin.ru/password
- Passwor for any user is 'password'
