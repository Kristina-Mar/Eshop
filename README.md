# Eshop case study
C# REST API for order processing includes:
- Domain project - Models and DTOs
- controller-based REST API
- Persistence project - SQLite, Entity Framework Core, includes a repository, a concurrent queue to add and fetch update requests and a background service to update order payment status
- controller unit tests

The program allows accessing/reading the orders (all and by Id), updating their payment status, and creating new orders.
