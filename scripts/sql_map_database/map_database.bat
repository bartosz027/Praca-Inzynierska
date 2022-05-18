@echo off
echo - mapping database...

set edmx_source_dir=%cd%\EDMX
set entity_framework_dir=%cd%\EntityFramework

cd ..\..\
set edmx_target_dir=%cd%\libs\Network.Server\Database

del %edmx_target_dir% /F /Q
xcopy %edmx_source_dir%\PiDbModel.* %edmx_target_dir%

cd %edmx_target_dir%
%windir%\Microsoft.NET\Framework\v4.0.30319\edmgen.exe /mode:fullgeneration /c:"Data Source=(localdb)\MSSQLLocalDB; Initial Catalog=PI_DB; Integrated Security=SSPI" /project:PiDbModel /entitycontainer:PiDbContext /namespace:PiDbModel /language:CSharp /pl

cd %edmx_source_dir%
start EDMX.exe %edmx_target_dir%\ ..\..\sql_create_database\sql_query\create_tabels.sql

cd %entity_framework_dir%
call text_transform.bat %edmx_target_dir% cs

del %edmx_target_dir%\PiDbModel.csdl
del %edmx_target_dir%\PiDbModel.msl
del %edmx_target_dir%\PiDbModel.ssdl

del %edmx_target_dir%\PiDbModel.ObjectLayer.cs
del %edmx_target_dir%\PiDbModel.Views.cs

del %edmx_target_dir%\t4list.txt

echo - mapping finished
echo.

pause > nul