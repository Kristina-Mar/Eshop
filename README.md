# Eshop case study
C# REST API for order processing includes:
- Domain project - Models and DTOs
- controller-based REST API with a Kafka producer for PUT requests
- Persistence project - SQLite database, Entity Framework Core, includes a repository
- background service = Kafka consumer for updating order status (PUT requests)
- controller unit tests
- dockerfile for the app
- docker compose files for Kafka (3 broker set-up) and the app, both containers run in the same network

The application allows accessing/reading the orders (all and by Id), updating their payment status, and creating new orders. API is available on port 5247. Kafka is configured to run in Docker, the application can run both locally and in Docker.
