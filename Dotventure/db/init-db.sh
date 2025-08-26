#!/bin/bash
# Wait for SQL Server to start
echo "Waiting for SQL Server to start..."
sleep 10s

echo "Running database restore..."
/opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P $SA_PASSWORD -C -i /var/opt/mssql/backup/init.sql

echo "Database restore completed."
