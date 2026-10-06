# ASP.NET 8 Web API Coding Exercise

ASP.NET Core 8 Web API for managing pizzas and toppings.

## Requirements

* .NET 8
* SQL Server

## Setup

There are two ways to set up the database.

### Option 1: Use the included empty database

Use the included empty database if you prefer.

Update the connection string in `appsettings.json` to point to the database.

### Option 2: Create the database using EF Migration

Open PowerShell and navigate to the project folder:

cd "path\to\WitPay_Assessment"

An `efbundle.exe` is included in the project.

Update the connection string in `appsettings.json`, then run:

```powershell
.\efbundle.exe
```

Or provide the connection string directly:

```powershell
.\efbundle.exe --connection "YOUR_CONNECTION_STRING"
```

This will apply the included EF Core migrations and create the required tables.

## Run the API

You can run the project from Visual Studio or using:

```powershell
dotnet run
```

## Test

Open Swagger:

ex. `http://localhost:5123/swagger`

Use Swagger to test the Pizza and Topping endpoints.
