# Wealth Management Assessment

A **.NET application** for investment portfolio evaluation, designed with **DDD (Domain-Driven Design)**, **Clean Architecture**, and **Dependency Injection**.

## 🚀 Purpose
The system simulates an **investment portfolio** that can include multiple asset types (stocks, funds, real estate, etc.), processing calculations for profitability, allocation, and investor balance.

## ✨ Highlights
- **Clean and decoupled architecture**: clear separation between Application, Domain, and Infrastructure.  
- **Central orchestration**: `AssetManagementService` coordinates the use cases.  
- **Aggregate Root (Portfolio)**: centralizes data composition and ensures consistency.  
- **Specialized Domain Services**: calculation-only classes per asset type, with no repository access.  
- **Pluggable data sources**: support for CSV, JSON, and API through **interfaces + DI**.  
- **Extensibility**: easy to add new assets (e.g., Crypto) or new data providers.  
- **Testability**: domain layer is independent of infrastructure, enabling fast and reliable unit testing.  

## 🛠 Tech Stack
- .NET 8 (C#)  
- Native Dependency Injection  
- Config via `appsettings.*.json`  

## ▶️ Run the Project
1. Clone the repository  
2. Adjust `appsettings.*.json` if needed  
3. Use the sample **CSV files** from the `Workload` folder  
4. Run the project with:

```bash
dotnet run
