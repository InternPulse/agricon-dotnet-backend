# Agricon Backend

A secure and scalable backend service built with ASP.NET Core for handling payments and transactions in the Agricon ecosystem.

## 🚀 Project Overview

Agricon backend provides RESTful APIs for Payment initialization and verification, Transaction management (CRUD, status update) and Webhook event handling (e.g., Paystack)


### Key features include:
- Transaction creation, listing, and per-agent analytics
- Dispute management (create, view, update, delete, statistics)
- Notification system with read tracking

## 🛠️ Tech Stack

- C# / ASP.NET Core
- Entity Framework Core
- PostgreSQL / SQL Server
- Swagger (OpenAPI 3.0)
- Paystack API
- RESTful architecture


## 📦 Getting Started

### Prerequisites

- .NET 8 SDK
- PostgreSQL Server
- Paystack Developer Account


## Installation Instructions

1. Clone the repository:

```bash
https://github.com/InternPulse/agricon-dotnet-backend.git
```

2. Change into the parent directory:

```bash
cd agricon
```

3. Set appropriate values for the following Compulsory Environment Variables:

```txt
# Postgres connection string
DATABASE_URL=""
# Secret key for signing JWTs
JWT_SECRET_KEY=
# API Port
PORT=5000
```

4. Apply Migrations:

```bash
dotnet ef database update
```

5. Run the Application:

```bash
dotnet run
```


The API should now be running locally at [http://localhost:7180/](http://localhost:7180/)

## 📄 API Documentation
You can explore and test the endpoints via the live Postman documentation:

🔗 [View Postman Collection](https://documenter.getpostman.com/view/43614350/2sB2ixjZkQ)

##  🔌 Available Endpoints
Here's an overview of available routes:

### Webhook
```
POST /api/webhook/paystack
```

### 💳 Transactions
```
POST /api/payment/initiate – Create a new transaction

GET /api/payment/verify/:reference – verify transaction

GET /api/v1/transaction/:id – Get a transaction by ID

PUT /api/v1/transactions/:id/status – Update a transaction

GET /api/v1/transactions/stats – Get overall transaction statistics

GET /api/v1/transaction/usertransactions/:Id – Get transaction stats for a specific booking
```
#### (More endpoints available in the Postman Docs)


## 🧑‍💻 Contributing

- Fork the repo
- Create your branch (git checkout -b feat/feature-name)
- Commit your changes
- Push and open a Pull Request
