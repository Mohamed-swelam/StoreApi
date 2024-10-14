# Store
### Key features:

-Product Management: Add, update, and remove products with ease.

-Category Management: Efficiently organize and retrieve products by categories.

-Order Processing: Streamlined order handling and management.

-User Authentication: Secure user management with ASP.NET Core Identity.

-Payment Processing: Stripe Integration enables smooth and secure online payment processing.

### 🎯 Architecture Highlights:

-N-Tier Architecture: The API follows a clean N-Tier Architecture, separating concerns across layers such as Presentation, Business Logic, and Data Access. This makes the application scalable and easier to maintain.

-Repository Pattern: Repositories are used to manage data access logic, making database operations for products, categories, and orders modular and reusable.

-Unit of Work: By implementing the Unit of Work pattern, I ensure that all database transactions are handled as a single atomic operation, promoting consistency and reducing potential issues with multiple changes.

-Service Layer: The business logic is abstracted into a dedicated service layer, keeping controllers lightweight and focused on HTTP requests.

-DTOs (Data Transfer Objects): To ensure secure and efficient data exchange between the API and the client, DTOs are used for handling requests and responses.

💳 Stripe Payment Integration:

Seamlessly integrated Stripe for handling secure online payments, allowing customers to complete transactions with ease.
