The Mediatonic Test solution has an API for implementing "Service based" functionality such as Stroking / Feeding of Animals and "Admin" based API CRUD functionality which is used to create, alter modify etc...

Included in this folder are DB structural schemas (script.sql) for the Model architecture for data persistance. This was implemented via Entity Framework with an underlying SQL Server DB.

The DB Architecture needs running from the provided schema

The ConnectionString will need modifying to your local version:

""MediatonicDatabase": "Server=*INSERT SERVER NAME HERE;Trusted_Connection=True;Database=Mediatonic;"

The Solution contains the main project and a unit test project.

The Service functionality is:

GET /service
/service/getAnimalsForUser/*INSERT ID*
/service/getAnimal/*INSERT ID*
/service/stroke/*INSERT ID*
/service/feed/*INSERT ID*

The API functionality is:
GET /api/Users/
GET /api/Users/*INSERT ID*
PUT /api/Users
POST /api/Users
GET /api/Users/delete/*INSERT ID*
GET /api/Animals/
GET /api/Animals/*INSERT ID*
PUT /api/Animals
POST /api/Animals
GET /api/Animals/delete/*INSERT ID*
GET /api/AnimalOwnerships/
GET /api/AnimalOwnerships/*INSERT ID*
PUT /api/AnimalOwnerships
POST /api/AnimalOwnerships
GET /api/AnimalOwnerships/delete/*INSERT ID*

Additionally, there is a POSTMAN collection file (Mediatonic.postman_collection.json) to run all of the API endpoints against.