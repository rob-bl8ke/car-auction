## Command Line Stuff

Use these commands find out information about your .NET  SDK installations.
```
dotnet sdk check
dotnet --info
dotnet new list
```

Use a installation manager like `winget` to install and upgrade SDKS.
```
winget list --upgrade-available --include-unknown
winget install --id Microsoft.DotNet.SDK.8

winget update --id Microsoft.DotNet.SDK.8
```

Prepare the folder structure for the solution.
```
dir
mkdir car-auction
cd .\car-auction\
code . -n
```
If you do have multiple versions of .NET installed and you wish to use a specific version that isn't the latest version you can specify this in a `global.json` file. You can generate this file using `dotnet new globaljson` in your project folder.

It creates a file like:

```json
{
  "sdk": {
    "version": "7.0.101"
  }
}
```
Build up the solution and its project structure.
```
dotnet new sln
dotnet new webapi -o src/AuctionService -controllers
dotnet sln add .\src\AuctionService

dotnet watch --project .\src\AuctionService\
```

Add the packages

```
dotnet add .\src\AuctionService\ package AutoMapper --version 13.0.1
dotnet add .\src\AuctionService\ package AutoMapper.Extensions.Microsoft.DependencyInjection 
dotnet add .\src\AuctionService\ package Microsoft.EntityFrameworkCore.Design --version 8.0.11
dotnet add .\src\AuctionService\ package Npgsql.EntityFrameworkCore.PostgreSQL --version 8.0.11
```
Remove the unnecessary stuff

- Remove all Swagger code and configuration.
- Remove any template controllers and entities.
- Remove all unnecessary profiles from  `launchSettings` except for `http`. 
- Remove HTTPS configuration and middleware.

### Run and watch the solution

```
cd .\src\AuctionService\
dotnet watch
```

### Adding the `DbContext`

- Once the `DbContext` has been defined in code go to the terminal.
- Look for the `dotnet-ef` tool. If its installed it should show up with `dotnet tool list -g` for globally installed or `dotnet tool list -l` for locally installed.
- Install it with `dotnet tool install dotnet-ef -g` if it doesn't exist. If it exists, just update it with `dotnet tool update dotnet-ef -g`. If you have a number of projects, you may wish to install it locally rather than globally.

You can simply uninstall and install it again.
```
dotnet tool uninstall -g dotnet-ef
dotnet tool install dotnet-ef -g --version 8.0.11
```

Finally create the first migration

```
cd .\src\AuctionService\
dotnet ef migrations add "InitialCreate" -o Data/Migrations
```

### Dockerizing the Database

- Create the `docker-compose.yml` file as shown and run

```
docker compose up
```

- Use `docker compose up -d` to run the postgres server in detached mode (you won't see the logs in the terminal).
- Look for `LOG:  database system is ready to accept connections`.
- Run the migrations to make sure it works...

```
dotnet ef database update
```
- Should end with `Done.`
- Install [PostgreSQL extension by Chris Kolkman for VS Code](https://marketplace.visualstudio.com/items?itemName=ckolkman.vscode-postgres) to see the database.
- A new tab is created and you can navigate to the PostgreSQL explorer and create a connection to your database. Follow the instructions using your connection string as a guide to open the database to view.
