@echo off
cls
echo ========================================
echo   HMS Mini - Setup Verification
echo ========================================
echo.

echo Checking .NET Runtime...
dotnet --version > nul 2>&1
if %errorlevel% equ 0 (
    echo [OK] .NET Runtime is installed
    dotnet --version
) else (
    echo [ERROR] .NET Runtime is NOT installed
    echo Please install .NET 8.0 Runtime from: https://dotnet.microsoft.com/download
)
echo.

echo Checking SQL Server...
sqlcmd -S (localdb)\MSSQLLocalDB -Q "SELECT @@VERSION" > nul 2>&1
if %errorlevel% equ 0 (
    echo [OK] SQL Server LocalDB is accessible
) else (
    echo [WARNING] SQL Server LocalDB not accessible
    echo Please verify SQL Server is installed and running
)
echo.

echo Checking API files...
if exist "api\HMSMini.API.dll" (
    echo [OK] API files found
) else (
    echo [ERROR] API files not found
    echo Please ensure the application was published correctly
)
echo.

echo Checking Web files...
if exist "web\HMSMini.Web.dll" (
    echo [OK] Web files found
) else (
    echo [ERROR] Web files not found
    echo Please ensure the application was published correctly
)
echo.

echo Checking ports...
netstat -ano | findstr :5096 > nul
if %errorlevel% equ 0 (
    echo [WARNING] Port 5096 is already in use
    echo You may need to stop the existing process or change the API port
) else (
    echo [OK] Port 5096 is available for API
)

netstat -ano | findstr :5131 > nul
if %errorlevel% equ 0 (
    echo [WARNING] Port 5131 is already in use
    echo You may need to stop the existing process or change the Web port
) else (
    echo [OK] Port 5131 is available for Web
)
echo.

echo ========================================
echo Setup verification complete!
echo.
echo If all checks passed, you can run:
echo START_HMS_MINI.bat
echo ========================================
echo.
pause
