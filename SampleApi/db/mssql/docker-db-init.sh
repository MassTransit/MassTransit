#run the setup script to create the DB and the schema in the DB
#do this in a loop because the timing for when the SQL instance is ready is indeterminate
for i in {1..50};
do
    #run the setup script to create the DB and the schema in the DB
    /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P MTsample1 -d master -i db-init.sql
    if [ $? -eq 0 ]
    then
        echo "db-init.sql completed"
        break
    else
        echo "not ready yet..."
        sleep 1
    fi
done
