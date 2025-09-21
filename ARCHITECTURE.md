
---

## 📘 ARCHITECTURE.md

```markdown
# Wealth Management Assessment - Architecture

This document describes the **architecture and design decisions** behind the Wealth Management Assessment project.

---

## 🏛️ Architectural Style

The project is inspired by **Clean Architecture** and **DDD principles**:

- **Domain Layer** → Pure business rules (Entities, Enums, Contracts).
- **Application Layer** → Use cases and orchestration (Asset management, Portfolio orchestration).
- **Infrastructure Layer** → Data access implementations (CSV/JSON providers, repositories).
- **Presentation Layer** → Console application (Program.cs).

---

## 📂 Layer Responsibilities

### 1. Domain
- **Entities**: Core models (`Investment`, `Investor`, `Quote`, `Transaction`).
- **Enums**: Investment types, transaction types, investor profile definitions.
- **Contracts**: Interfaces that define repository and service contracts.
- **Services**: Domain-level service interfaces (e.g. `IStockService`).

---

### 2. Application
- **Orchestration**: Coordinates services (e.g. `AssetManagementService`, `PortfolioService`).
- **Configuration**: AppConfig and environment settings.
- **Models**: Transport objects (e.g. `InvestorBalanceResult`).

---

### 3. Infrastructure
- **DataProviders**: Input adapters (CSV, JSON, API).
- **Repository**: Implements repositories that interact with `DataProviders`.

---

### 4. Presentation
- **Console Application** (Program.cs):
  - Provides the interactive menu.
  - Handles user inputs.
  - Calls orchestration services.
  - Displays formatted results.

---

## 🔄 Data Flow

1. **Investor request** (menu) triggers an orchestration service.
2. **Application layer** fetches investments via repository.
3. **Repositories** load data from **CSV/JSON providers** (infrastructure).
4. **Services** calculate balances per asset type.
5. **Results** are aggregated and returned as `InvestorBalanceResult`.
6. **Console** prints results with execution time.

---

## 🧠 Design Considerations

- **Caching**: Fonds hydration uses cache for performance optimization.
- **Performance metrics**: Stopwatch prints execution times per step.
- **Extensibility**: Can easily replace `CSV source` with `API` or `Database`.
- **Testability**: Interfaces and layered design make it suitable for unit tests.

---

## 📊 Example Flow

Investor90 → PortfolioService → AssetManagementService
↳ PortfolioRepository → Transactions (CSV)
↳ FondsService + Hydration
↳ StockService + Quotes
↳ RealEstateService


---

## ✅ Conclusion

The project demonstrates **modular design**, **clear separation of concerns**, and **performance-awareness**.  
It can be extended to real-world investment platforms or serve as a reference for DDD + Clean Architecture applications in .NET.
