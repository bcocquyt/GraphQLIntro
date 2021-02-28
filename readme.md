# GraphQL Intro

This project illustrates some of the functionalities of GraphQL implemented with HotChocolate. I also included a rest api to the same data as comparison.

For more information on Arcus, see https://github.com/arcus-azure/arcus.webapi

The following GraphQL concepts are demonstrated:
* Query
* Mutation
* Subscription

## Creating the project

``
dotnet new arcus-webapi
``

##  Entity Framework

## EF Tools

``
dotnet tool install --global dotnet-ef
``
or
``
dotnet tool update --global dotnet-ef
``

### Initial Migration

``
dotnet ef migrations add InitialCreate
``
``
dotnet ef database update
``

### Remove Migration
``
dotnet ef migrations remove
``

## GraphQL Hot Chocolate
See https://chillicream.com/docs/hotchocolate

## YouTube

See:
https://www.youtube.com/watch?v=Yy9wOhiWBJg
