@echo off
set dir=%cd%

cd %dir%\sql_create_database
echo | call create_database.bat

cd %dir%\sql_map_database
echo | call map_database.bat

pause