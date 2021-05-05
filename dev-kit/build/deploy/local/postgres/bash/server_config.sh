psql -U admin -d postgres -c "create user ${POSTGRES_USER_NAME} with createdb createrole PASSWORD '${POSTGRES_USER_PASSWORD}'"
psql -U admin -d postgres -c "create tablespace ${POSTGRES_USER_TABLESPACE} owner ${POSTGRES_USER_NAME} location '/var/lib/postgresql'"
psql -U admin -d postgres -c "create database ${POSTGRES_USER_DB} owner=${POSTGRES_USER_NAME} tablespace=${POSTGRES_USER_TABLESPACE}"
#
#execute sql
#
for file in /tmp/sql/*.sql; do 
	psql -U ${POSTGRES_USER_NAME} -d ${POSTGRES_USER_DB} -a -f "${file}" 
done
#
#sample_db
#
psql -U admin -d postgres -c "create user postgres createdb createrole superuser PASSWORD '${POSTGRES_USER_PASSWORD}'"
psql -U admin -d postgres -c "grant admin to postgres"
psql -U postgres -d postgres -c "create database otus_dvdrental_db owner=postgres tablespace=${POSTGRES_USER_TABLESPACE}"
pg_restore -U postgres -d otus_dvdrental_db -v /sample_db/dvdrental.tar --no-password
#
#demo db
#
psql -U ${POSTGRES_USER_NAME} -d ${POSTGRES_USER_DB} -a -f "/sample_db/airport_sample.sql"