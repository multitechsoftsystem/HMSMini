@echo off
cls
echo ========================================
echo   HMS Mini - API Server
echo ========================================
echo.
echo Starting API Server on http://localhost:5096
echo.
echo Press Ctrl+C to stop the server
echo ========================================
echo.
dotnet HMSMini.API.dll
pause
