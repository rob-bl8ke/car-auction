# Auction Service

### Start up

Terminal 1
```
docker compose up
```

Terminal 2
```
cd .\src\AuctionService\
dotnet watch
```

### Reset the database

Simplest way is as follows (ensure that the postgres container is running):

```
dotnet ef database drop 
```
Once the database has been dropped, go and restart to create and reset the seed data. This is helpful if you want to run all the Postman tests without failure to ensure all your endpoints are working.

```
dotnet watch
```

## Development Notes

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

> **Note:** .NET CLI now allows one to create a `.gitignore` file using the following command: `dotnet new gitignore` 

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
- Check that the container is up and running with `docker container ls -a`.
- `docker container stop [id]` to stop instances.
- Look for `LOG:  database system is ready to accept connections`.
- Run the migrations to make sure it works...

```
dotnet ef database update
```
- Should end with `Done.`
- Install [PostgreSQL extension by Chris Kolkman for VS Code](https://marketplace.visualstudio.com/items?itemName=ckolkman.vscode-postgres) to see the database.
- A new tab is created and you can navigate to the PostgreSQL explorer and create a connection to your database. Follow the instructions using your connection string as a guide to open the database to view.

### Seeding Data

This project hard codes the seeding of data, but it might be worth looking at imports.

- [CsvHelper](https://github.com/JoshClose/CsvHelper) looks to be a good candidate for importing CSV Data and there is some mention of [DataFrame](https://devblogs.microsoft.com/dotnet/an-introduction-to-dataframe/).

The database can be recreated and reseeded using the following commands. Confirm it in the Postgres Explorer.
```
cd .\src\AuctionService\
dotnet ef database drop
```
Simply running the application should seed the data if no data exists.
```
dotnet watch
```

# Search Service

Build up the solution and its project structure.
```
dotnet new webapi -o src/SearchService -controllers
dotnet sln add .\src\SearchService

dotnet watch --project .\src\SearchService\
```

### MongoDB

Add MongoDB to the `docker-compose.yml` file. Test that the container runs successfully. Go and install `MongoDB for VS Code` by `MongoDB`. The extension will ask you to connect to `mongodb://root:mongopw@localhost:27017/` once you've supplied the username and password as it is defined in the `docker-compose.yml` file. A new tab should show up with a leaf icon. Here you can explore the database and run stuff.

To grab hold of the seed data simply use the Postman call to `Get all auctions` and copy and paste the JSON data into a new file in the Data folder. Call it `auctions.json`.

The `Item` class does not contain and Id property because it inherits from `Entity` which provides its own `MongoDB` equivalent. The import from the `auctions.json` file should populate the `_id` property in MongoDB for each `Item` document.

[MongoDB.Entitites Official Website](https://mongodb-entities.com/) is a good place to go understand how to perform CRUD and other operations against the MongoDB database. It provides a LINQ like interface to the MongoDB database. Take a look at the [Code Samples](https://mongodb-entities.com/wiki/Code-Samples.html) for a crash course.


# References and Side-notes

#### Take control of auto-generate VS Code Features
[This lecture](https://www.udemy.com/course/build-a-microservices-app-with-dotnet-and-nextjs-from-scratch/learn/lecture/39040266) discusses how to make code generated private class level members names with an underscore rather than using the `this` keyword.

Simply put you need to add a `.editorconfig` in the solution root.

```
[*.{cs,vb}]
dotnet_naming_rule.private_members_with_underscore.symbols  = private_fields
dotnet_naming_rule.private_members_with_underscore.style    = prefix_underscore
dotnet_naming_rule.private_members_with_underscore.severity = suggestion

dotnet_naming_symbols.private_fields.applicable_kinds           = field
dotnet_naming_symbols.private_fields.applicable_accessibilities = private

dotnet_naming_style.prefix_underscore.capitalization = camel_case
dotnet_naming_style.prefix_underscore.required_prefix = _
```
> This solution does not use it, as my personal preference is to use `this`.
