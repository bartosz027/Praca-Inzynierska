@echo off
echo - creating database...

SQLCMD -S (LocalDB)\MSSQLLocalDB -E -i sql_query\create_database.sql
SQLCMD -S (LocalDB)\MSSQLLocalDB -E -i sql_query\create_tabels.sql
SQLCMD -S (LocalDB)\MSSQLLocalDB -E -i sql_query\insert_values.sql

echo - database created
echo.

pause > nul