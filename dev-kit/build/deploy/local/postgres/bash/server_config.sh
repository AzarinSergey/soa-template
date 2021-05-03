psql -U admin -d postgres -c "create user ${POSTGRES_USER_NAME} with createdb createrole PASSWORD '${POSTGRES_USER_PASSWORD}'"
psql -U admin -d postgres -c "create tablespace ${POSTGRES_USER_TABLESPACE} owner ${POSTGRES_USER_NAME} location '/var/lib/postgresql'"
psql -U admin -d postgres -c "create database ${POSTGRES_USER_DB} owner=${POSTGRES_USER_NAME} tablespace=${POSTGRES_USER_TABLESPACE}"
#
for file in /tmp/sql/*.sql; do 
	psql -U ${POSTGRES_USER_NAME} -d ${POSTGRES_USER_DB} -a -f "${file}" 
done

