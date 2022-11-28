# 📡WEB API💻

#### Informations :
> I tried my best to create the API as great as i can, it propably need
> to be improved. I do not really comments at all, i apologize but i
> specify my sources. I have issues on load test the API does not
> support multiple thread.

## Project :

**Creation from scratch of an API that manages Users in .Net 6🚀** 
*(ORM : Entity Framework Core)*

> -   Method SCRUM [Azure DevOps]
> -   Clean architecture [Application(api) - Infrastructure - Core]
> -   Code First
> -   Gitflow [branch management: dev; realease(test); main(production)]

-Create Models (Entities),  
-Create DTOs,  
-Create DbContext
-Migration( add-migration…),  
-Create Service + interface ,  
-Create Contoller (using Interfaces),  
-Configure Connection String,  
-Configure AutoMapper,  
-Routing : Methode Async (Task / await),  
-Configure Swagger( docs/ version management.. ),  
-Configure SeriLog( +appsetting / MSSqlServer),  
-Read AppSetting: Option Pattern( IOptions; appsettings.json… )  
-Authentication and Authorization (swagger / EntitiesAuth)  
-Unit Test ( Xunit [serviceProvider] )  
-Unit Test with Moq (Test double)  
-ASP.NET Core SignalR (Streaming DATA from client(console) to server(API)🔁)  
-Background Service(using Signal for Notification & ServiceScopeFactory to get service )  
-Actually on SoapUI...

## 📄License
[MIT](https://choosealicense.com/licenses/mit/)
