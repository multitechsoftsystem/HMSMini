# HMS Mini - Local Deployment Guide

This guide explains how to deploy the Hotel Management System on your local machine for production use.

## Prerequisites

- Windows 10/11
- .NET 8.0 Runtime installed
- SQL Server (Express or higher)
- Admin privileges (for IIS deployment)

## Deployment Options

You have two options for deployment:

### **Option 1: Self-Hosted (Recommended for Testing)**
Run as standalone executable using Kestrel web server

### **Option 2: IIS Deployment (Recommended for Production)**
Host on Windows Internet Information Services (IIS)

---

## Option 1: Self-Hosted Deployment

### Step 1: Publish the Applications

Open Command Prompt or PowerShell in the project root directory:

```bash
# Navigate to project root
cd D:\DOTNET\test\HMSMini

# Publish API
dotnet publish src/HMSMini.API/HMSMini.API.csproj -c Release -o publish/api

# Publish Web
dotnet publish src/HMSMini.Web/HMSMini.Web.csproj -c Release -o publish/web
```

### Step 2: Configure Database Connection

Edit the API configuration file:

```bash
notepad publish/api/appsettings.json
```

Update the connection string to point to your SQL Server:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER_NAME;Database=HMSMiniDB;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

Replace `YOUR_SERVER_NAME` with:
- `localhost` or `(localdb)\\MSSQLLocalDB` for SQL Server Express
- Your actual server name for full SQL Server

### Step 3: Run Database Migrations

```bash
cd publish/api
dotnet HMSMini.API.dll
```

The API will automatically create the database and seed initial data on first run.

Press `Ctrl+C` to stop it after you see "Database seeded successfully"

### Step 4: Configure API URL in Web Application

Edit the Web configuration:

```bash
notepad publish/web/wwwroot/appsettings.json
```

If the file doesn't exist, create it with this content:

```json
{
  "ApiBaseUrl": "http://localhost:5096"
}
```

If you want to use different ports, update the API's `appsettings.json`:

```json
{
  "Urls": "http://localhost:5096",
  "ConnectionStrings": {
    "DefaultConnection": "..."
  }
}
```

### Step 5: Create Startup Scripts

#### For API (save as `start-api.bat` in publish/api folder):

```batch
@echo off
echo Starting HMS Mini API Server...
dotnet HMSMini.API.dll
pause
```

#### For Web (save as `start-web.bat` in publish/web folder):

```batch
@echo off
echo Starting HMS Mini Web Application...
dotnet HMSMini.Web.dll
pause
```

### Step 6: Run the Applications

1. **Start API Server:**
   - Navigate to `D:\DOTNET\test\HMSMini\publish\api`
   - Double-click `start-api.bat`
   - Wait for "Now listening on: http://localhost:5096"

2. **Start Web Server:**
   - Navigate to `D:\DOTNET\test\HMSMini\publish\web`
   - Double-click `start-web.bat`
   - Wait for "Now listening on: http://localhost:5131"

3. **Access the Application:**
   - Open browser and go to: `http://localhost:5131`

### Default Login Credentials

After first deployment:
- **Admin User:**
  - Username: `admin`
  - Password: `Admin@123`

- **Manager User:**
  - Username: `manager`
  - Password: `Manager@123`

---

## Option 2: IIS Deployment

### Prerequisites

1. Enable IIS on Windows:
   - Open "Turn Windows features on or off"
   - Enable "Internet Information Services"
   - Enable "World Wide Web Services" > "Application Development Features" > "ASP.NET 4.8" (or higher)
   - Install ASP.NET Core Runtime Hosting Bundle from Microsoft

### Step 1: Publish for IIS

```bash
# Publish API for IIS
dotnet publish src/HMSMini.API/HMSMini.API.csproj -c Release -o C:\inetpub\wwwroot\HMSMiniAPI

# Publish Web for IIS
dotnet publish src/HMSMini.Web/HMSMini.Web.csproj -c Release -o C:\inetpub\wwwroot\HMSMiniWeb
```

### Step 2: Configure Database

Edit `C:\inetpub\wwwroot\HMSMiniAPI\appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER_NAME;Database=HMSMiniDB;Trusted_Connection=True;TrustServerCertificate=True;"
  },
  "Jwt": {
    "SecretKey": "YourSecretKeyHere_MustBe32CharactersOrMore!",
    "Issuer": "HMSMiniAPI",
    "Audience": "HMSMiniWeb",
    "ExpiryInMinutes": 1440
  }
}
```

### Step 3: Create IIS Application Pools

Open IIS Manager and create two application pools:

1. **HMSMiniAPI_Pool**
   - .NET CLR Version: No Managed Code
   - Managed Pipeline Mode: Integrated

2. **HMSMiniWeb_Pool**
   - .NET CLR Version: No Managed Code
   - Managed Pipeline Mode: Integrated

### Step 4: Create IIS Websites

1. **API Website:**
   - Site name: HMSMiniAPI
   - Physical path: `C:\inetpub\wwwroot\HMSMiniAPI`
   - Binding: `http://*:5096`
   - Application Pool: HMSMiniAPI_Pool

2. **Web Website:**
   - Site name: HMSMiniWeb
   - Physical path: `C:\inetpub\wwwroot\HMSMiniWeb`
   - Binding: `http://*:5131`
   - Application Pool: HMSMiniWeb_Pool

### Step 5: Set Folder Permissions

Grant `IIS_IUSRS` and `IUSR` read/execute permissions to:
- `C:\inetpub\wwwroot\HMSMiniAPI`
- `C:\inetpub\wwwroot\HMSMiniWeb`

### Step 6: Configure Web Application

Edit `C:\inetpub\wwwroot\HMSMiniWeb\wwwroot\appsettings.json`:

```json
{
  "ApiBaseUrl": "http://localhost:5096"
}
```

### Step 7: Update Web Program.cs for IIS

The application is already configured for IIS. No changes needed.

### Step 8: Start Websites in IIS Manager

1. Start HMSMiniAPI website
2. Start HMSMiniWeb website
3. Access application at `http://localhost:5131`

---

## Production Considerations

### Security

1. **Change JWT Secret Key:**
   - Use a strong, unique secret key (32+ characters)
   - Store in environment variables or secure configuration

2. **HTTPS Configuration:**
   - For production, configure HTTPS with SSL certificates
   - Update CORS settings to allow only specific origins

3. **Database Security:**
   - Use SQL Server authentication instead of Windows Authentication
   - Create dedicated database user with minimal privileges
   - Regular backups

### Performance

1. **Connection Pooling:**
   - Already configured in Entity Framework Core
   - Default pool size is sufficient for most scenarios

2. **Static File Caching:**
   - Already configured in Web application
   - Browser caching enabled for CSS, JS, images

3. **API Response Caching:**
   - Consider adding response caching for frequently accessed data
   - Redis cache for distributed scenarios

### Monitoring

1. **Logging:**
   - Check API logs in console output or Windows Event Viewer
   - Configure Serilog or NLog for file logging

2. **Application Insights:**
   - Consider adding Application Insights for monitoring
   - Track performance metrics and errors

---

## Troubleshooting

### API won't start

- Check if port 5096 is already in use: `netstat -ano | findstr :5096`
- Check database connection string
- Verify SQL Server is running
- Check Windows Firewall settings

### Web won't connect to API

- Verify API is running and accessible at configured URL
- Check CORS settings in API startup
- Verify authentication tokens are being sent
- Check browser console for errors

### Database errors

- Verify SQL Server is running
- Check connection string format
- Ensure database user has proper permissions
- Run migrations manually if needed

### IIS Issues

- Verify ASP.NET Core Hosting Bundle is installed
- Check Application Pool identity has proper permissions
- Review IIS logs: `C:\inetpub\logs\LogFiles`
- Verify web.config is present in published folders

---

## Updating the Application

When deploying updates:

1. **Stop the running applications** (or stop IIS websites)
2. **Backup the database:**
   ```sql
   BACKUP DATABASE HMSMiniDB TO DISK = 'C:\Backups\HMSMiniDB.bak'
   ```
3. **Publish new version** to the same directory
4. **Run any new migrations** (API will do this automatically)
5. **Start the applications** again

---

## Uninstallation

### Self-Hosted:
1. Stop the running applications
2. Delete `D:\DOTNET\test\HMSMini\publish` folder
3. Drop database: `DROP DATABASE HMSMiniDB`

### IIS:
1. Stop and delete IIS websites
2. Delete application pools
3. Delete `C:\inetpub\wwwroot\HMSMiniAPI` and `HMSMiniWeb` folders
4. Drop database

---

## Support

For issues or questions:
- Check the API logs for detailed error messages
- Review browser console for client-side errors
- Verify all prerequisites are met
- Ensure all configuration files are correct

---

## Summary

Your HMS Mini application is now deployed and ready to use!

**Quick Start:**
1. Published files are in `D:\DOTNET\test\HMSMini\publish`
2. Run `start-api.bat` to start API
3. Run `start-web.bat` to start Web
4. Access at `http://localhost:5131`
5. Login with `admin` / `Admin@123`
