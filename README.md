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


Using a paged search with MongoDB.Entities is very simple. Simply use the `PagedSearch` method. The result will include the `PageCount`, `TotalCount`, and the results. See `SearchController`.

#### More Advanced Search stuff

Important that you go over [this tutorial](https://www.udemy.com/course/build-a-microservices-app-with-dotnet-and-nextjs-from-scratch/learn/lecture/39040426). It uses a "new style" switch statment to add some advanced filtering to your search query. Useful for (probably) just about any filtering logic you may require at some point.

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

# RabbitMQ and Mass Transit

Take a look at the docker-compose.yml file. Use `docker compose up -d` to run everything. Navigate to http://localhost:15672/ and type in `guest` for both the username and password.

Add the following packages: `MassTransit.RabbitMQ` by Chris Patterson.

Create  class library for the RabbitMQ infrastructure (shared by all services). Although this feels like we're breaking the "dependency" principle, its not a direct coupling between our services and it will be limited to the messaging infrastructure. This avoids having to add a `Contracts` folder for each service.

```
dotnet new classlib -o src/Contracts
dotnet sln add .\src\Contracts

cd ./src/SearchService/
dotnet add reference ../../src/Contracts

cd ../../

 cd ./src/AuctionService/
 dotnet add reference ../../src/Contracts

cd ../..
cd ./src/Contracts/
dotnet new gitignore
```

See the `AuctionCreated` event class. There is no guarantee that the event will arrive instantaneously and there is no guarantee of the order, either. The idea is to achieve "eventual consistency" between the `AuctionService` and the `SearchService`.

**Important**: Mass Transit requires the event class to be in the same namespace for both the producer and the consumer: another reason to create a shared library rather than copy and paste to each service.

When creating consumers, always use the following naming convention: `[name]Consumer` and have it inherit from the generic `IConsumer` interface where the generic parameter is the auction class. See `AuctionCreatedConsumer`.

To enhance the consumer Exchange and Queue name, look for how `SetEndpointNameFormatter` is used in the `Program.cs` class.

## Asynchronous communication

- Don't reach for synchronous communication without reviewing the design of your services.
- Typically, a fully asynchronous approach is preferred over synchronous http requests.
- Publish messages to a message broker and have other services subscribe to it.

The `SearchService` will have its `MongoDB` database updated to become consistent with the `Postgres` database via async messaging via the `RabbitMQ` message broker. None of the services will be aware of each other. Services will only be aware of the service bus.

Here are a couple examples of message brokers:
- Rabbitmq
- Azure Service Bus
- Amazon

> The preference for `RabbitMQ` in this solution is to satisfy the requirement of running the solution on the local developer computer.

What if the event bus goes down? Is it a single point of failure? Services can no longer communicate with each other. However, if designed properly the Microservices will still do the jobs they're designed to do.

- The message broker should be given high availability. If the message broker fails a new one should pop up in its place. It should be clustered, it should be fault tolerant, and it should have persistent storage.
- Each service should implement some kind of retry policy if the service bus cannot immediately be reached. Aim for smart services that are able to carry out those tasks.
- Aim for a stupid pipe that's only responsible for receiving messages and storing them in queues.

### [RabbitMQ](https://www.rabbitmq.com/)

And what is Rabbitmq? It's a message broker. It accepts and forwards messages.

- It accepts a message into an exchange.
- It forwards the messages to a queue that services can subscribe to
- It uses the producer and consumer model (or publish/subscribe model).
- Messages are stored on queues (message buffer).
- Rabbitmq can have persistence associated with it too (for failure recovery)

#### Exchanges

- Publish a message, send it to an exchange.
- Exchanges have bound queues.

> Rabbitmq uses, as do other transports, the advanced message queuing protocol (AMQC).

- The `AuctionService` is a publisher that's going to publish an "auction created event", to an exchange.
- This exchange has one or more queues bound to that exchange
- The message will be placed on each queue and wait for a consumer to pick up and consume the message.
- There can be one queue, there can be two, there can be a thousand. The publisher of the message doesn't care.
- Multiple consumers consume from the same queue or multiple queues.

> There are other types of exchanges not used in this solution. This solution follows the "fan out" approach.

### [Mass Transit](https://masstransit.io/introduction)

There are various options to connect to RabbitMQ.
- Use the Rabbitmq client and directly connect and send messages.
- Use a layer of abstraction such as Mass Transit

It provides a consistent abstraction on top of supported message transports. Similar to how Entity Framework provides an abstraction from a database technology. Mass transit achieves the same goal, but it provides an abstraction from the message transport technology.

> In addition to RabbitMQ, Mass Transport supports Azure Service Bus, Amazon SQS, ActiveMQ, gRPC, and Kafka.

And a different transport is decided on in the future other than RabbitMQ, then by using Mass Transit, code changes should be minimal.

- It also provides other features such as message routing exception handling.
- It provides a test harness to test the messaging behavior (in-memory), and to avoid the need for an external infrastructure service to provide testing.
- It provides dependency injection capabilities.

# Dealing with Failure Points

![Current Architecture](https://www.plantuml.com/plantuml/png/POrB3e9038RtFKN3dYiGCDo04zIXqKdcWPtMnDiR57MGJTF_bQylIg8M0pOZBo4_8YV5qTD5A3P0JhFWi5vmEIuvhV5WJdrE4ylTEuQPbCvK-ECNW9knCMw5anMerXEJkEjX_BO1NZ1ikt_ANbQ50eUixMhsxzsIchzJ-8qHUawpnV04)

### What should happen when RabbitMQ is down and the `AuctionService` send a message to the `SearchService`?

Is this possibility is left unsolved, it will lead to an inconsistent state in that the `Auction` database will end up being inconsistent with the `Search` database. The Auction database will have been updated with a new record but the Search database will not record the newly added Auction.

If the message broker is down the call to send a message to the queue will fail, but using an Email Outbox type of approach, one can get around this by setting up an "Outbox" for messages. An EF Core `Mass Transit` nuget package (`MassTransit.EntityFrameworkCore`) exists that will handle this type of behavior. One only needs to install the package, wire it up to the `AuctionService` web app and generate and execute another EF Core Migration.

Add it to the `AuctionService` and run a new migration.

```
dotnet ef migrations add "Outbox" -o Data/Migrations
```

Mass Transit creates three new tables `InboxState`, `OutboxState`, `MessageState` which it uses to manage message Outbox state.

> Look at the `DbContext` and `Program` classes to see what changes are required.

```csharp
x.AddEntityFrameworkOutbox<AuctionDbContext>( o => {
    o.QueryDelay = TimeSpan.FromSeconds(10);
    o.UsePostgres();
    o.UseBusOutbox();
});
```

Note the changes in the `AuctionsController` class. Since EF Core is being used to handle the state of the messages the entire scenario is based on the same transaction. This means that the "publish" statement is moved up above the `context.SaveChangesAsync()` statement in the `CreateAuction()` controller method. If the message is not published successfully, an Auction is never created and the inconsistent state problem that can occur if a messsage is not properly delivered is avoided.

#### Test the failure mitigation behaviour

Once this is set up make sure that all Docker containers except for the message broker are up and running. Running a Postman call to create an auction will result in a 201 created response. However, the call to the message broker would have failed behind the scenes.
- Note how the `OutboxMessage` and the `OutboxState` tables contain a single record.
- Note the terminal shows how the `MassTransit.EntityFrameworkCore` package code continuously fetches any unsent message data and attempts to resent to the message broker repeatedly.
- Note how the `SearchService` continuously outputs an exception message because it is trying to communicate with the message broker.

Now run the event broker container and note how the call from the `AuctionService` to the message broker is finally successful. Query the `SearchService` and verify that the Auction has been successfully created in the Search database.

This way, a resilient "eventually consistent" state exists between the AuctionService and thee SearchService regardless of whether the event broker fails or not. Assuming ofcourse, that the event broker will at some point in the future be brought back online. 

