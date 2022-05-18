@echo off
SETLOCAL ENABLEDELAYEDEXPANSION

FOR /F "tokens=* USEBACKQ" %%F IN (`"%ProgramFiles(x86)%\Microsoft Visual Studio\Installer\vswhere.exe" -latest -property installationPath`) DO (
	SET vs_installation_path=%%F
)

set edmx_target_dir=%1
set extension=%2

dir %edmx_target_dir%\*.tt /b /s > %edmx_target_dir%\t4list.txt
type %edmx_target_dir%\t4list.txt

set entity_framework_dir=%cd%
cd %vs_installation_path%\Common7\IDE

for /f %%d in (%edmx_target_dir%\t4list.txt) do (
	set file_name=%%d
	set file_name=!file_name:~0,-3!.%extension%
	
	echo:  \--^> !file_name!
	TextTransform.exe -I %entity_framework_dir% -out !file_name! %%d
)