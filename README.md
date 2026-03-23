# Airline Ticketing API

**Author:** Batuhan Salcan  
**Course:** SE 4458 Software Architecture & Design of Modern Large Scale Systems  
**Assignment:** Midterm Project (Group 1 - API Project for Airline Company)

## 📌 Project Overview
This project is a RESTful Web API built with **.NET 8** to serve as the backend for a high-traffic airline ticketing system. The API allows administrators to upload flight schedules, and enables passengers to query flights, buy tickets, and check in. The system is designed with enterprise-grade architectural patterns, prioritizing scalability, security, and maintainability.

## ☁️ Cloud Infrastructure & Deployment
To demonstrate a production-ready environment, the entire system has been deployed to the Microsoft Azure cloud ecosystem:
* **API Hosting:** Deployed to **Azure App Service**, providing a scalable and fully managed web server environment.
* **Database Hosting:** Migrated from a local environment to **Azure Database for MySQL Flexible Server**. The API securely communicates with this cloud database, ensuring data persistence and high availability.

## 🏛️ Architectural Decisions & Design Patterns

To ensure clean code and separation of concerns, the project strictly adheres to an N-Tier architecture, utilizing several gang-of-four design patterns:

### 1. The Facade Pattern (Service Layer)
Controllers in this API act strictly as traffic directors. They contain zero business or database logic. Instead, the complexity of the ticketing and flight management systems is hidden behind Facade interfaces (`IFlightService` and `ITicketService`). This keeps the Presentation Layer (Controllers) extremely thin and decoupled from the Data Access Layer.

### 2. The Strategy Pattern (File Parsing)
The midterm requires an "Add Flight by File" endpoint that accepts a `.csv` file. Instead of hardcoding CSV parsing logic directly into the `FlightService`, the **Strategy Pattern** was implemented. 
* An `IFlightFileParser` interface defines the behavior.
* A specific `CsvFlightParser` strategy implements the reading logic. 
* **Justification:** This adheres to the Open/Closed Principle (OCP). If the airline eventually requires JSON or XML uploads, a new parser can be created without modifying or breaking the core `FlightService`.

### 3. Data Transfer Objects (DTO Pattern)
To prevent "Over-Posting" attacks and strictly control the data flowing in and out of the API, DTOs are used for every endpoint. The raw database models (`Flight`, `Ticket`, `User`) are never exposed directly to the client.

### 4. Repository & Unit of Work Patterns
Entity Framework (EF) Core is utilized as the ORM. The `DbSet<T>` properties inside `ApplicationDbContext` act as the in-memory **Repositories**, while `_context.SaveChangesAsync()` acts as the **Unit of Work**. This ensures that complex transactions (such as decreasing flight capacity and generating a ticket simultaneously) are committed to the database atomically and safely.

### 5. Dependency Injection (Inversion of Control)
ASP.NET Core's built-in DI container is used extensively. Services and parsers are registered in `Program.cs` with Scoped lifecycles, ensuring seamless testability and loose coupling.

---

## 💾 Database Design & Technologies

* **Database Engine:** MySQL (Azure Flexible Server)
* **ORM:** Pomelo Entity Framework Core (Code-First Approach)
* **Data Seeding (Best Practice):** To avoid hardcoded credentials while allowing seamless testing, EF Core's `HasData` method is used to automatically seed a default Administrator account into the database during initial migration.
* **Entities & Relationships:**
  * `Flight`: Stores schedule, origin/destination, and capacity constraints.
  * `User`: Stores passenger details and roles.
  * `Ticket`: Acts as the junction entity mapping a `User` to a `Flight`, storing specific generated identifiers and assigned seat numbers.

*(Insert a screenshot/link to your Data Model / ER Diagram here)*

---

## ✅ Midterm Requirements & Assumptions

| Feature | Implementation Notes |
| :--- | :--- |
| **Authentication** | Implemented using **JWT Bearer Tokens**. A seeded admin user (`Username: admin`, `Password: admin123`) is verified directly against the database to generate the token. Endpoints like adding flights and buying tickets are secured with the `[Authorize]` attribute. |
| **Paging** | Implemented on `Query Flight` and `Passenger List` endpoints with a default page size of 10. |
| **Capacity Management** | Handled transactionally. When a ticket is bought, the flight's capacity is decreased. If capacity is 0, the API returns a "Sold out" response. |
| **Seat Assignment** | The `Check-In` endpoint automatically generates and assigns a sequential seat number to the passenger. |
| **Rate Limiting (3 calls/day)** | **Architectural Assumption:** In modern large-scale systems, rate limiting should not be handled at the application code level. This requirement is intentionally deferred to be configured within the **API Gateway** (e.g., Azure API Management or AWS API Gateway) during deployment to prevent unnecessary load on the backend servers. |

---

## 🚀 Deliverables & Links

* **Deployed Swagger URL:** *https://batu-airline-api-argehsbgendkhzb3.italynorth-01.azurewebsites.net/swagger/index.html*
* **Load Test Results:** *(Link or screenshots will be added after testing)*
* **Project Presentation Video:** *(Link to Google Drive / YouTube will be added)*
* **Data Model (ER Diagram):** *(Link or image to be added)*

---

## 🛠️ How to Run Locally

1. Clone the repository.
2. Update the `DefaultConnection` string in `appsettings.json` with your local MySQL credentials. (Note: The current connection string points to the live Azure Database for testing purposes).
3. Open a terminal in the project root and run the following commands:
   ```bash
   dotnet restore
   dotnet ef database update
   dotnet build
   dotnet run
