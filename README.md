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