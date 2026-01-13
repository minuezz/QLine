# QLine - Virtual Queue Management System

**QLine** is a modern web application designed to replace physical waiting lines with an efficient digital reservation system. It allows users to book appointments online, check in via QR codes, and enables staff to manage queues in real-time.

## Key Features

### For Clients
* **Smart Booking Wizard:** Easy 2-step reservation process (Service & Time selection).
* **Real-time Availability:** Automatic concurrency control prevents double bookings.
* **Ticket Generation:** unique QR codes generated for every reservation.
* **Live Updates:** Real-time notification of queue status.

### For Staff
* **Queue Dashboard:** Real-time view of waiting customers (powered by SignalR).
* **Ticket Processing:** Call next ticket, mark as completed, or report no-shows.
* **Multi-location Support:** Switch between different service points dynamically.

### For Administrators
* **Service Point Management:** Create and manage physical locations and opening hours.
* **Service Configuration:** Define service types, duration, and buffer times.
* **User Management:** Role-Based Access Control (RBAC) for Admins, Staff, and Clients.

## Tech Stack

The project follows **Clean Architecture** principles and utilizes modern .NET technologies:

* **Backend:** .NET 8 (C#)
* **Frontend:** Blazor Server
* **UI Framework:** MudBlazor
* **Database:** PostgreSQL (Entity Framework Core)
* **Real-time Communication:** SignalR
* **Architecture:** CQRS (Command Query Responsibility Segregation) with MediatR
* **Containerization:** Docker (for Database and MailDev)

## Prerequisites

Before running the project, ensure you have the following installed:

* [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
* [Docker Desktop](https://www.docker.com/products/docker-desktop) (for PostgreSQL & MailDev)
* Any IDE (Visual Studio 2022, JetBrains Rider, or VS Code)

## Getting Started

### 1. Clone the repository

git clone [https://github.com/your-username/QLine.git](https://github.com/minuezz/QLine.git)
cd QLine

### 2. Start Infrastructure (Docker)
This project uses Docker to run the PostgreSQL database and a local SMTP server for testing emails.

# Run PostgreSQL container
docker run -d -p 5432:5432 --name qline_db -e POSTGRES_PASSWORD=postgres postgres

# Run MailDev (Local Email Server)
docker run -d -p 1080:1080 -p 1025:1025 --name maildev maildev/maildev

### 3. Configure Database
Update the connection string in QLine.Web/appsettings.json if necessary (default is set to localhost). Then apply migrations:

cd QLine.Web
dotnet ef database update

### 4. Run the Application
Bash

dotnet run

# The application will be available at https://localhost:7001 (or similar port).

### 5. Email Testing
To view emails sent by the system (e.g., booking confirmations), open your browser and go to: http://localhost:1080

### Project Structure
The solution is organized according to Clean Architecture layers:
QLine.Domain: Core entities and business logic (no dependencies).
QLine.Application: Use cases, CQRS commands/queries, interfaces.
QLine.Infrastructure: Implementation of interfaces (DB Context, Repositories, Email Sender).
QLine.Web: The presentation layer (Blazor components, UI).

### License
This project was developed as a diploma thesis.
