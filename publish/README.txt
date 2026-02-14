========================================
  HMS Mini - Deployment Package
========================================

QUICK START:
------------
1. Double-click "START_HMS_MINI.bat" to start the application
2. Wait for both servers to start (two command windows will open)
3. Browser will open automatically at http://localhost:5131
4. Login with:
   Username: admin
   Password: Admin@123

MANUAL START:
-------------
If you prefer to start servers individually:

1. Start API Server:
   - Go to "api" folder
   - Double-click "start-api.bat"
   - Wait for "Now listening on: http://localhost:5096"

2. Start Web Application:
   - Go to "web" folder
   - Double-click "start-web.bat"
   - Wait for "Now listening on: http://localhost:5131"

3. Open browser and go to: http://localhost:5131

REQUIREMENTS:
-------------
- .NET 8.0 Runtime (already installed)
- SQL Server (Express or higher)
- Windows 10/11

DATABASE:
---------
The application uses SQL Server with this default connection:
Server: (localdb)\MSSQLLocalDB
Database: HMSMiniDB

To change the database:
1. Edit "api\appsettings.json"
2. Update the "DefaultConnection" connection string

TROUBLESHOOTING:
----------------
Problem: API won't start
Solution: Check if port 5096 is already in use
         Run: netstat -ano | findstr :5096

Problem: Web won't connect to API
Solution: Make sure API is running first
         Check browser console for errors

Problem: Database errors
Solution: Verify SQL Server is running
         Check connection string in api\appsettings.json

STOPPING THE APPLICATION:
-------------------------
Press Ctrl+C in each command window, or simply close the windows.

For detailed deployment instructions, see:
DEPLOYMENT_GUIDE.md in the project root folder

========================================
