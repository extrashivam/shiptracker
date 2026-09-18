# ShipTracker.Api

A real-time maritime vessel tracking API. Ingests live AIS (Automatic
Identification System) data from ships at sea and exposes it over a
REST API, with real-time updates planned via WebSockets.

## Stack

- **.NET 8** — Web API with controllers
- **Entity Framework Core** + **Npgsql** — PostgreSQL data access
- **PostgreSQL** — primary datastore (local dev via Docker, production on Railway)

## Running locally

1. Clone the repo and restore dependencies:
```bash
   dotnet restore
```

2. Start a local PostgreSQL instance (Docker):
```bash
   docker run --name shiptracker-db -e POSTGRES_PASSWORD=postgres -p 5432:5432 -d postgres
```

3. Update the connection string in `appsettings.Development.json` if needed.

4. Apply migrations:
```bash
   dotnet ef database update
```

5. Run the API:
```bash
   dotnet run
```

6. Open Swagger UI at `http://localhost:<port>/swagger` to explore the API.
