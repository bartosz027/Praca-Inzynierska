@echo off
echo - mapping database...

set edmx_dir=%cd%\EDMX
set ef_utility_dir=%cd%\EF6.Utility

cd ..\..\
set database_dir=%cd%\libs\Network.Server\Database

del %database_dir% /F /Q
xcopy %edmx_dir%\template\PiDbModel.* %database_dir%

cd %database_dir%
%windir%\Microsoft.NET\Framework\v4.0.30319\edmgen.exe /mode:fullgeneration /c:"Data Source=(localdb)\MSSQLLocalDB; Initial Catalog=PI_DB; Integrated Security=SSPI" /project:PiDbModel /entitycontainer:PiDbContext /namespace:PiDbModel /language:CSharp /pl

cd %edmx_dir%
start EDMX.exe %database_dir%\ ..\..\sql_create_database\sql_query\create_tabels.sql

cd %ef_utility_dir%
call text_transform.bat %database_dir% cs

del %database_dir%\PiDbModel.csdl
del %database_dir%\PiDbModel.msl
del %database_dir%\PiDbModel.ssdl

del %database_dir%\PiDbModel.ObjectLayer.cs
del %database_dir%\PiDbModel.Views.cs

del %database_dir%\t4list.txt

echo - mapping finished
echo.

pause > nul