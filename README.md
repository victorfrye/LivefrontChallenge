# Livefront Challenge

This repository contains my solution to a coding challenge by Livefront. The challenge was to create an API specification for a new referrals feature and a mock implementation to test against.

My solution for this challenge includes:

🌐 A microservice .NET Web API application using ASP.NET Core minimal APIs

📃 API specification using OpenAPI 3.0 generated at runtime

🛢️ A containerized SQL Server instance for mock data and persistence

🐳 .NET Aspire to orchestrate the local development environment

🧪 A test suite using xUnit and .NET Aspire

🚀 GitHub Actions workflows for continuous integration

## Table of Contents

- [Get Started](#get-started)
    - [Prerequisites](#prerequisites) 
    - [.NET Aspire](#net-aspire)
    - [HTTPS](#https)
    - [Run the app](#run-the-app)
- [API Specification](#api-specification)
    - [Endpoints](#endpoints)
    - [Scenario considerations](#scenario-considerations)
- [Helpful Links](#helpful-links)

## To Do List

- [x] Create repository
- [x] Create referrals models
- [x] Create EFCore database context
- [x] Create Aspire app host
- [x] Create Aspire service defaults
- [x] Create retrieve list endpoint
- [x] Create retrieve single endpoint
- [x] Create create endpoint
- [x] Create update status endpoint
- [x] Add OpenAPI documentation
    - [x] Add documentation base information
    - [x] Add operation response types
    - [x] Add operation summaries
    - [x] Add operation descriptions
    - [x] Add model field descriptions
- [ ] Add unit tests
- [x] Add format workflow to pipeline
- [x] Add build workflow to pipeline
- [x] Add test workflow to pipeline
- [x] Add README documentation
    - [x] Write API Specification details
    - [x] Write how to run details
- [ ] Remove this list when complete

## ✅ Get Started

### Prerequisites

To run this project, you will need to have the following software installed on your machine:

- [Git](https://git-scm.com/downloads)
- [.NET 9 SDK](https://dotnet.microsoft.com/en-us/download)
- [Docker Desktop](https://www.docker.com/get-started/)

*WARNING: This project requires an OCI compliant container runtime for .NET Aspire. However, additional changes are potentially required if using an alternative to Docker Desktop.*

### .NET Aspire

This project uses .NET Aspire to orchestrate the local development environment. This allows the application to be run alongside a local SQL Server container for mock data testing.

For more information on or troubleshooting .NET Aspire, see the [Aspire documentation](https://learn.microsoft.com/en-us/dotnet/aspire/get-started/aspire-overview).

### HTTPS

This project uses HTTPS for local development. This requires the use of a trusted certificate. Before running the application, the following command will check for an existing certificate or generate a new one.

```pwsh
dotnet dev-certs https --trust
```

### Run the app

To run the application, simply run the following command in the root of the project:

```pwsh
dotnet run --project .\src\AppHost\AppHost.csproj
```

This will start the database and application. You will find the following components at the following locations:

- **Aspire Dashboard**: [https://localhost:17236](https://localhost:17236)
- **Referrals API Specification**: [https://localhost:7258/openapi/v1.json](https://localhost:7258/openapi/v1.json)

### Seed data

The application will seed mock data on startup to allow getting started without creating any.

These three referrals will be created and can be retrieved by calling the `GET /referrals` endpoint with code query parameter `XY7G4D`.

```json
[
  {
    "id": "01948986-814b-7dc7-be3b-1542b8650112",
    "code": "XY7G4D",
    "referee": {
      "id": "01948986-f2b5-775f-b50f-ac0a7c9ffb4a",
      "firstName": "Archer",
      "lastName": "King"
    },
    "status": "Complete"
  },
  {
    "id": "01948987-0ddc-7591-9750-74604d5b3c28",
    "code": "XY7G4D",
    "referee": {
      "id": "01948987-1ffb-7ab6-ab76-32d4eef94e3b",
      "firstName": "Helen",
      "lastName": "Yang"
    },
    "status": "Pending"
  },
  {
    "id": "01948986-41e1-7412-85c9-790850ef1d9e",
    "code": "XY7G4D",
    "referee": {
      "id": "01948986-687c-7532-8f82-4b4c1134db3d",
      "firstName": "Jenny",
      "lastName": "Smith"
    },
    "status": "Complete"
  }
]
```

## 📃 API Specification

The API specification for this project is generated at runtime using OpenAPI 3.0 and RESTful design. The most up-to-date document can be found at the endpoint `/openapi/v1.json`. Additionally, a copy of this document is included in the repository [here](/docs/openapi.json).

### Endpoints

The following endpoints are available in this API:

- `GET /referrals`: Retrieve a collection of referrals.
- `POST /referrals`: Create a new referral.
- `GET /referrals/{id}`: Retrieve a referral by ID.
- `PUT /referrals/{id}`: Create or update a referral by ID.
- `PATCH /referrals/{id}/status`: Update the status of a referral by ID.

Additional endpoints related to service operations are also included but not referenced in the actual API specification.

- `GET /openapi/v1.json`: Retrieve the OpenAPI specification document.
- `GET /health`: Retrieve the health status of the application.
- `GET /alive`: Retrieve the liveness status of the application.

### Scenario considerations

Some design assumptions were taken in building this API specification. Additionally, questions were presented to be considered in building. The result of these relating to the API will be answered in the following question/answer format.

#### How will existing users create new referrals using their existing referral code?

Existing users will be able to create new referrals using their existing referral code by including the `code` field in the request body when creating a new referral with the `POST /referrals` endpoint. The API will validate the request and create the new referral with the provided code.

#### How will the app generate referral links for the share feature?

There are three ways included in the scenario to share a referral:

1. **Share via Text**: A pre-populated text message sent via SMS or other text messaging solution.
2. **Share via Email**: A pre-populated email sent via email client.
3. **Share via Share Sheet**: Integration with the device's share sheet to allow sharing via any app that supports sharing or quick copy of the link.

In a production environment, the text and email solution would be automated. These scenarios are mocked in this API and considered by including the `email` and `phone` fields in the `Referee` model. One way this would be handled is a change feed that detects new or updates to a referral that include an email address or phone number and then sending an event to send these pre-populated messages.

However, the share sheet solution would be handled by the client application. This API does not include any functionality for this and considers that possibility that neither email address or phone number are included with the referee.

In the event of all three, the generated link would include the referral ID to provide a direct link back to the referral. Alternatively the link could include the referrer's code and search to find matching information, but as referral code is not unique per referral this is not recommended design.

#### How will existing users check the status of their referrals?

Existing users will be able to check the status of their referrals by retrieving a collection of referrals with the `GET /referrals` endpoint. The API will return a list of all referrals, including their status. A query parameter for referral `code` is required for existing users to filter their referrals by their unique code. Optionally, the API provides a `status` parameter to additionally filter by a given status.

#### How will the app know where to direct new users after they install the app via a referral?

New users will be directed to the app via a referral link. The link will include the referral ID, which can be used to identify the referrer and referee, as well as any additional information needed to direct the new user to the appropriate location in the app. This link is assumed to be handled by the third-party vendor for deferred deep link support and the client application.

The API will operate agnostic of how the link is handled, but provide the necessary endpoints to support completing the referral, i.e. `PATCH /referrals/{id}/status`., and retrieving the referral information, i.e. `GET /referrals/{id}`.

There is an assumption that the client application can recognize that a referral is involved during the first run experience.

#### Since users may eventually earn rewards for referrals, should we take extra steps to mitigate abuse?

Yes, and there is a multitude of ways this can be handled. One is to limit the number of referrals a user can create in a given window. This could be handled on either the client side or server-side.

Another way to validate referees are real users with third-party identity verification platforms. This could be before creation or event-driven afterwards with a status update between `Pending` and `Complete`.

A third way is to rate limit the API. This could be handled by API gateways or by the API itself depending on the desired architecture.

Given the vast myriad of ways to handle this, no specific implementation is included in this API. This is assumed to be future scope in a real implementation.

## Helpful Links

- [Referrals API Document](./docs/openapi.json)
- [.NET Aspire documentation](https://learn.microsoft.com/en-us/dotnet/aspire/get-started/aspire-overview)
- [Example Requests](./src/WebApi/Examples.http)