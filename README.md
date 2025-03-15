# Project Setup and Environment Guidelines


## 1. Required NuGet Packages


1. **Microsoft.EntityFrameworkCore**

    ```bash
    dotnet add package Microsoft.EntityFrameworkCore --version 7.0.10
    ```

    **Pomelo.EntityFrameworkCore.MySql**

    ```bash
    dotnet add package Pomelo.EntityFrameworkCore.MySql --version 7.0.0
    ```

    This is the EF Core provider for MySQL.

    **DotNetEnv (Optional but recommended for loading .env files)**

    ```bash
    dotnet add package DotNetEnv
    ```

    **Microsoft.AspNetCore.Http (for session usage or other HTTP features)**

    ```bash
    dotnet add package Microsoft.AspNetCore.Http
    ```

    **MySql.Data (only if you need raw ADO.NET usage)**

    ```bash
    dotnet add package MySql.Data
    ```

    (Adjust versions to match your .NET runtime and project requirements.)

## 2. The .env File Format

We do not commit our real `.env` file to source control. Instead, we commit a `.env.example` as a template. Below is an example format you can follow:

<details> <summary><strong>.env.example</strong> (template)</summary>

```dotenv
# Logging configuration
Logging__LogLevel__Default=Information
Logging__LogLevel__Microsoft.AspNetCore=Warning

# Allowed hosts
AllowedHosts=*

# MySQL connection string (adjust placeholders as needed)
ConnectionStrings__MySqlConnection=server=127.0.0.1;port=3306;database=animal_shelter_db;uid=ACCOUNT NAME;pwd=YOUR_PASSWORD_HERE;
