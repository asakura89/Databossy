@echo off

set appname=Databossy
set config=Release
set cwd=%CD%
set outputdir=%cwd%\build
set commonflags=/p:Configuration=%config%;AllowUnsafeBlocks=true /p:CLSCompliant=False

set nugetversion=latest
set cachednuget=%LocalAppData%\NuGet\nuget.%nugetversion%.exe

if %PROCESSOR_ARCHITECTURE%==x86 (
    set msbuild="%WINDIR%\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe"
) else (
    set msbuild="%WINDIR%\Microsoft.NET\Framework64\v4.0.30319\MSBuild.exe"
)
goto build

:build-error
echo Failed to compile.
goto exit

:build
echo ---------------------------------------------------------------------
echo Building AnyCpu release...
rem %msbuild% %appname%.sln %commonflags% /p:TargetFrameworkVersion=v2.0 /p:Platform="Any Cpu" /p:OutputPath="%outputdir%\net20"
rem if errorlevel 1 goto build-error
%msbuild% %appname%.sln %commonflags% /p:TargetFrameworkVersion=v3.5 /p:Platform="Any Cpu" /p:OutputPath="%outputdir%\net35"
if errorlevel 1 goto build-error
%msbuild% %appname%.sln %commonflags% /p:TargetFrameworkVersion=v4.0 /p:Platform="Any Cpu" /p:OutputPath="%outputdir%\net40"
if errorlevel 1 goto build-error
%msbuild% %appname%.sln %commonflags% /p:TargetFrameworkVersion=v4.5 /p:Platform="Any Cpu" /p:OutputPath="%outputdir%\net45"
if errorlevel 1 goto build-error

:done
echo.
echo ---------------------------------------------------------------------
echo Compile finished.
echo.

if exist %cachednuget% goto copynuget
echo Downloading latest version of NuGet.exe...
if not exist %LocalAppData%\NuGet md %LocalAppData%\NuGet
@powershell -NoProfile -ExecutionPolicy unrestricted -Command "$ProgressPreference = 'SilentlyContinue'; Invoke-WebRequest 'https://dist.nuget.org/win-x86-commandline/%nugetversion%/nuget.exe' -OutFile '%cachednuget%'"
goto createpackage

:copynuget
if exist .nuget\nuget.exe goto createpackage
md .nuget
copy %cachednuget% .nuget\nuget.exe > nul

:createpackage
echo Creating nuget package...
.nuget\nuget.exe pack %appname%.nuspec -OutputDirectory "%outputdir%"
cd %cwd%
goto exit

:exit