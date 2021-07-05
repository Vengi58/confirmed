# Confirmed
confirmed API 1.0

ASP.NET Core Web API

## Context

<img src="resources/context.png" width="400">

## Containers

<img src="resources/containers.png" width="400">

## Components

<img src="resources/components.png" width="800">

## Code

### Implementation Details

ASP.Net Core (5.0) Web Application, implemented in C#, using Visual Studio 2019. For database connection and data management it uses Entity Framework Core and supports SQL or InMemory Postgres database. \
The API exposes a Swagger UI for documentation and testing purposes.

The Solution contains 2 projects

#### ConfirmedAPI

##### Controllers

***ProductController***

- 
##### Data
The `ConfirmedDbContext` represents the database schema and used in Entity Framework to create database schema.\
The `IConfirmedRepository` maps CRUD operations to functions required by the controllers.
The connectionString must be configured in the `appSettings.json` file.

##### DTO
Data Transfer Objects carry data between the different layers.

##### Models
Contains the models classes used to create database schema.


#### ConfirmedAPITests

The tests project contains several test for the `ProductController`functions and also for the `ConfirmedRepository`. It uses prepopulated in-memory Postgres database for test data.

### Demo

The Confirmed web API is deployed to an AWS ElasticBeanstalk environment using the AWS SDK Tools in VisualStudio.
It uses and AWS RDS Postgres database for data persistance.

The Demo can be found here: http://confirmed-stock.us-east-1.elasticbeanstalk.com/swagger/index.html

### Notes, considerations

- The solution does not contain authentication or authorization. To improve security, auth layer or a separate could be added later.
- The solution is a simple web API, hosted in AWS in the demo. The ElastikBeanstalk provides several options for scalability, monitoring, alerting which could be easily used to prepare the service for incrementing load.
- Currently any size of images are allowed to upload, which could affect the perfomance, especially that the upload/download is synchronous. 
- For simplicity, the web method's response is HTTP 400 BadRequest in every case when an Exception occurs, otherwise HTTP 200 Ok.
- The Swagger UI is exposed also when not in development mode and it does not contain any AUTH.
- The code does not contain any logging, but NLog could be used for detailed logging.
- Exception handling, input validation and error handling could be further improved accross the code.

