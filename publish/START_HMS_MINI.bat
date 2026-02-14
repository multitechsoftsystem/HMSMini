@echo off
cls
echo ========================================
echo   HMS Mini - Hotel Management System
echo ========================================
echo.
echo This will start both API and Web servers
echo.
echo API Server:  http://localhost:5096
echo Web App:     http://localhost:5131
echo.
echo ========================================
echo.
echo Starting API Server...
start "HMS Mini API" cmd /k "cd /d %~dp0api && start-api.bat"
timeout /t 5 /nobreak > nul
echo.
echo Starting Web Application...
start "HMS Mini Web" cmd /k "cd /d %~dp0web && start-web.bat"
timeout /t 3 /nobreak > nul
echo.
echo ========================================
echo Both servers are starting!
echo.
echo You can now access the application at:
echo http://localhost:5131
echo.
echo Default Login:
echo   Username: admin
echo   Password: Admin@123
echo.
echo Press any key to open in browser...
pause > nul
start http://localhost:5131
