@echo off
cls
echo ========================================
echo   HMS Mini - Web Application
echo ========================================
echo.
echo Starting Web Server on http://localhost:5131
echo.
echo Press Ctrl+C to stop the server
echo ========================================
echo.
echo IMPORTANT: Make sure the API server is running first!
echo.
dotnet HMSMini.Web.dll
pause
