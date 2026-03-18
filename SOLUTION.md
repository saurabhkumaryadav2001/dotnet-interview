# Refactoring Plan: TodoApi

## Identified Issues & Areas of Improvement

1. **SQL Injection Vulnerabilities**: 
   The `TodoService` builds SQL queries using string interpolation (e.g., `$"SELECT * FROM Todos WHERE Id = {id}"`). This exposes the application to SQL injection attacks.

2. **Tight Coupling and Lack of Dependency Injection**: 
   `TodoController` and `UnitTest1` currently instantiate `TodoService` directly using `new TodoService()`. This violates the Dependency Inversion Principle and makes the class hard to independently test. Additionally, `TodoService` instantiates a new SQLite connection internally.

3. **Non-RESTful API Design**: 
   - All controller actions are decorated with HTTP POST (`[HttpPost]`), even for fetching, updating, or deleting.
   - The endpoints do not conform to standard REST routing conventions (e.g., uses `/api/createTodo` instead of `/api/todos`).
   - The `GetTodo` endpoint attempts to double as both "GetAll" and "GetById" depending on the incoming DTO payload.

4. **Testing Hygiene**: 
   - Unit tests are directly hitting a physical database ("todos.db") instead of mocking dependencies. 
   - Test methods rely on state from previous tests (Integration testing instead of true unit testing).

## Refactoring Strategy

### Part 1: Fix Architectural & Security Fundamentals (The Service)
- Extract an `ITodoService` interface from `TodoService` to enable Dependency Injection and test mocking.
- Modify `TodoService` queries to use parameterized SQLite commands (`SqliteParameter`), resolving the SQL injection vulnerability.

### Part 2: Refactor the Presentation Layer (The Controller)
- Introduce standard Dependency Injection into `TodoController` (injecting `ITodoService` via constructor).
- Refactor endpoints (`Get`, `Post`, `Put`, `Delete`) to adhere to standard REST routing conventions (`api/todos`, `api/todos/{id}`).
- Remove redundant request DTOs (`GetTodoRequest`, `DeleteTodoRequest`) in favor of standard URL parameters.

### Part 3: Configuration & Bootstrapping
- Register the `ITodoService` in `Program.cs`'s DI container.

### Part 4: Updating the Test Suite
- Update existing tests in `UnitTest1.cs` to handle the new DI structure and architectural changes.

---

*This document will track my ongoing rationale and decisions as I incrementally commit changes.*
