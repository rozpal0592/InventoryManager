# Inventory Manager – Console-Based Supply Tracking

## 📘 Project Summary
The **Inventory Manager** is a C# console application for managing suppliers and inventory items with full CRUD support and SQLite persistence.  
It demonstrates core Object-Oriented Programming (OOP) principles including **interfaces, abstract classes, inheritance, composition, polymorphism, constructors, and access specifiers**.  

This project was developed as part of a capstone-style course project and is portfolio-ready for showcasing C# development, database integration, and software engineering practices.

## 🚀 Features
- **Suppliers**
  - Add new suppliers
  - List suppliers in a formatted table

- **Items**
  - Add Generic, Medical, or Office items
  - List items with tabular formatting
  - Update item quantities
  - Delete items by ID

- **Database**
  - SQLite backend for persistent storage
  - Auto-seeding ensures default suppliers and demo items exist
  - Enforces foreign key relationships between suppliers and items

- **OOP Concepts**
  - `IReportable` interface
  - `InventoryBase` abstract class
  - `InventoryItem`, `MedicalSupply`, `OfficeSupply` classes demonstrating inheritance & polymorphism
  - Composition: each `InventoryItem` has a `Supplier`
  - Proper constructors and access specifiers

## How to Run
1. Clone the repository:
     git clone https://github.com/rozpal0592/InventoryManager.git
     cd InventoryManager
2. Build & Run
    From the project root:
      dotnet new colsole
      dotnet run

## Requirements Mapping
| Requirement       | Design Element               | Implementation                                 |
|-------------------|------------------------------|------------------------------------------------|
| Interface         | IReportable                  | Implemented in InventoryBase & subclasses      |
| Abstract Class    | InventoryBase                | Defines shared logic and abstract methods      |
| Composition       | Items reference Supplier     | InventoryItem contains a Supplier object       |
| Polymorphism      | Specialized item classes     | MedicalSupply, OfficeSupply overrides methods  |
| Constructors      | All classes use constructors | Example: MedicalSupply(...), Supplier(...)     |
| Access Specifiers | Public get, protected set    | Enforces encapsulation across properties       |
| CRUD Opertaions   | SupplierDB, ItemDB           | Full Create, Read, Update, Delete in SQLite    |
| Terminal I/O      | ConsoleApp class             | Menu-driven console UI with formatted reports  |

##  UML Class Diagram
See UML class diagram.

## Demo Video



