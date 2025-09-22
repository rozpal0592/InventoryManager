# Inventory Management System (IMS)

## 📌 Project Overview
The Inventory Management System (IMS) is a C# console application designed to track and manage supply records. Users can add, view, update, and delete inventory items with key details such as name, quantity, expiration date, and supplier information.  
This project demonstrates **Object-Oriented Programming (OOP)** concepts, including interfaces, abstract classes, inheritance, composition, and polymorphism, while preparing for **SQLite database integration** (Week 4).

## 🛠 Features (Week 3 – Class Implementation)
- **Terminal I/O:** Menu-driven console interface for managing items.  
- **Item Types:**  
  - `InventoryItem` (generic item)  
  - `MedicalSupply` (cold-chain, lot tracking, expiry priority)  
  - `OfficeSupply` (office/admin supplies, stock-only priority)  
- **Supplier Management:** Each item is associated with a supplier (aggregation).  
- **Reports:**  
  - Low stock items (ROP threshold)  
  - Expiring soon (≤30 days)  
- **Polymorphism:** Item types override `CalculateReorderPriority`, `GenerateReport`, and `ToString`.

## 📂 Class Structure
- **IReportable** – Interface for generating reports.  
- **InventoryBase (abstract)** – Shared state & behavior for items.  
- **InventoryItem** – Concrete base item type.  
- **MedicalSupply** – Extends `InventoryItem`, adds cold-chain and lot tracking.  
- **OfficeSupply** – Extends `InventoryItem`, adds model info.  
- **Supplier** – Entity used via aggregation.  
- **ConsoleApp** – Handles menu UI and item management.  

## 📊 Requirements Mapping
| Requirement                       | Implementation                                                                 |
|-----------------------------------|--------------------------------------------------------------------------------|
| Terminal I/O                      | ConsoleApp (menu, prompts, reports)                                            |
| Interface                         | IReportable                                                                    |
| Abstract Class                    | InventoryBase                                                                  |
| Composition/Aggregation           | InventoryItem ◊– Supplier                                                      |
| Polymorphism                      | Overrides in MedicalSupply / OfficeSupply                                      |
| Constructors & Access Specifiers  | All classes use parameterized constructors; `protected set` for IDs/timestamps |
| SQLite CRUD                       | Planned for Week 4                                                             |

## 🖼 UML Class Diagram
See UML class diagram.

## 🚀 Build & Run
From the project root:
  dotnet new colsole
  dotnet run

## 📌 Project Phases
- **Week 1 – Proposal:** Defined project scope & purpose.  
- **Week 2 – Design:** Created class definitions & database schema.  
- **Week 3 – Implementation:** Developed classes and console framework (no DB).  
- **Week 4 – Database Integration:** Add SQLite CRUD operations.  
- **Week 5 – Final Submission:** Documentation, GitHub polish, demo video.
