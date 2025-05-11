# Notification Service

## Description
Service for managing notifications with real-time capabilities. Provides REST API, SignalR, and integration with PostgreSQL and Redis for reliable notification delivery and processing.

## Features
- **Create notification**: Create new notifications with various payloads
- **Get notification**: Retrieve notification by unique ID
- **Notification list**: Fetch notifications with pagination and filtering
- **Mark as read**: Update notification status
- **Real-time streaming**: SignalR for instant notification delivery
- **Background processing**: Hangfire for scheduled and recurring tasks
- **Caching**: Redis-based caching for improved performance

## Technologies
- ASP.NET Core 9.0
- PostgreSQL (data storage)
- Redis (caching and real-time features)
- BackgroundService (background processing)
- Docker (containerization)

## Getting Started

### Prerequisites
Ensure you have Docker and Docker Compose installed on your machine.

### Setup
1. Clone the repository:
```bash
git clone https://github.com/alibekovvl/NotificationService.git
cd your-repository
```

2. Create .env and paste the following:
```bash
POSTGRES_DB=notificationDb
POSTGRES_USER=postgres
POSTGRES_PASSWORD=postgres
REDIS_HOST=redis
REDIS_PORT=6379
REDIS_CONNETCION=redis:6379
DEFAULT_CONNECTION=Host=postgres;Port=5432;Database=your_database;Username=postgres;Password=postgres
ASPNETCORE_ENVIRONMENT=Development
```

3. Start an application:
```bash
docker-compose up --build
```

## How to use

If you used my .env, so you can use the following urls:
Swagger for REST API: http://localhost:8080/swagger/index.html
