## docker run
docker compose  up --build  -d

## docker run
docker compose  down

## docker run mssql databse
docker compose -f docker-compose-mssql.yml up --build  -d
docker compose -f docker-compose-mssql-arm.yml up --build  -d

## docker down mssql databse
docker compose -f docker-compose-mssql.yml down

## Installing the tools
dotnet tool install --global dotnet-ef

## Create your first migration
dotnet ef migrations add InitialCreate

## Create your database and schema
dotnet ef database update

## เข้าไปใน container
docker exec -it  mssql-server bash

## docker copy file
docker cp "D:\Dragon Database Backup" mssql-server:/home