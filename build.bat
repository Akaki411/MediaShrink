@echo off
rem Builds MediaShrink with Framework MSBuild. Usage: build.bat [Debug|Release]
setlocal
cd /d "%~dp0"
set "PF86=%ProgramFiles(x86)%"
set "CONFIG=Release"

:args
if "%~1"=="" goto find
if /i "%~1"=="debug" set "CONFIG=Debug"
if /i "%~1"=="release" set "CONFIG=Release"
shift
goto args

:find
set "MSB="
if exist "%PF86%\Microsoft Visual Studio\Installer\vswhere.exe" (
  for /f "usebackq delims=" %%i in (`"%PF86%\Microsoft Visual Studio\Installer\vswhere.exe" -latest -products * -requires Microsoft.Component.MSBuild -find MSBuild\**\Bin\amd64\MSBuild.exe`) do set "MSB=%%i"
)
if not defined MSB (
  for /d %%y in ("%PF86%\Microsoft Visual Studio\*") do (
    for /d %%e in ("%%y\*") do (
      if exist "%%e\MSBuild\Current\Bin\amd64\MSBuild.exe" set "MSB=%%e\MSBuild\Current\Bin\amd64\MSBuild.exe"
    )
  )
)
if not defined MSB (
  echo MSBuild not found. Install "Build Tools for Visual Studio" with the ".NET desktop build tools" workload.
  exit /b 1
)

echo MSBuild: %MSB%
"%MSB%" MediaShrink.csproj /restore /t:Rebuild /p:Configuration=%CONFIG% /verbosity:minimal /nologo
if errorlevel 1 exit /b 1
echo Done: %~dp0bin\%CONFIG%\MediaShrink.exe

exit /b 0
