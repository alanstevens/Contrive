@echo off
setlocal enableextensions

set SCRIPT=%0
set DQUOTE="
set OLDDIR=%CD%

:: Uncomment this line to have the MSBUILD generated files written to disk
:: set msbuildemitsolution=1

:: Detect how script was launched
@echo %SCRIPT:~0,1% | findstr /l %DQUOTE% > NUL

if %ERRORLEVEL% EQU 0 set PAUSE_ON_CLOSE=1

:: Change to the directory containing the script
cd /d %0\..

:: Load Visual Studio build environment
if not defined DevEnvDir call "%VS110COMNTOOLS%vsvars32.bat"

IF %1.==. GOTO Default
msbuild.exe /filelogger BuildContriveSample.proj /t:%*
GOTO End

:Default
msbuild.exe /filelogger BuildContriveSample.proj /t:Debug
GOTO End

:End
:: Return to the original directory
cd /d %OLDDIR%

:: Keep the window open if launched from explorer
if defined PAUSE_ON_CLOSE pause
