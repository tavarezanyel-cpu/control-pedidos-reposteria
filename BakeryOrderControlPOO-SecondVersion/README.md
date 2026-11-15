# Bakery Order Control - Second Version (OOP)

C# console application for a bakery that manages products, customers and orders.
Data is kept in memory (it is lost when the program closes).

## Structure
- `Models/`: `BaseEntity` (abstract), `Product`, `Customer`, `Order`
- `Repositories/`: `IRepository<T>` and `InMemoryRepository<T>`
- `Services/`: business rules (`ProductService`, `CustomerService`, `OrderService`)
- `UI/`: `ConsoleHelper`, `MenuBase` and the menus that inherit from it, `App`
- `Exceptions/`: `BakeryException`

## OOP concepts used
Classes and objects, encapsulation (properties with validation), inheritance (`BaseEntity`, `MenuBase`),
polymorphism (`Describe()`, `Execute()`), abstraction (`IRepository<T>`), generics and custom exceptions.

## Modern C# features used
Top-level statements, file-scoped namespaces, primary constructors, collection expressions (`[]`, `[.. x]`),
target-typed `new()`, switch expressions, pattern matching (`is { }`, `is null`, `is not`), throw expressions and `using var`.

## Run
```
dotnet run
```
