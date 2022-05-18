@echo off
SETLOCAL ENABLEDELAYEDEXPANSION

FOR /F "tokens=* USEBACKQ" %%F IN (`"%ProgramFiles(x86)%\Microsoft Visual Studio\Installer\vswhere.exe" -latest -property installationPath`) DO (
	SET vs_installation_path=%%F
)

FOR /F "tokens=* USEBACKQ" %%F IN (`"%ProgramFiles(x86)%\Microsoft Visual Studio\Installer\vswhere.exe" -latest -property catalog_productLineVersion`) DO (
	SET vs_version=%%F
)

set database_dir=%1
set extension=%2

dir %database_dir%\*.tt /b /s > %database_dir%\t4list.txt
type %database_dir%\t4list.txt

if %vs_version% == 2019 (
	set ef_utility_dir=%cd%\vs2019
)

if %vs_version% == 2022 (
	set ef_utility_dir=%cd%\vs2022
)

:: Set current dir to path where "TextTransform.exe" is located.
cd %vs_installation_path%\Common7\IDE

for /f %%d in (%database_dir%\t4list.txt) do (
	set file_name=%%d
	set file_name=!file_name:~0,-3!.%extension%
	
	echo:  \--^> !file_name!
	TextTransform.exe -I %ef_utility_dir% -out !file_name! %%d
)