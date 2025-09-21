# Architecture — Wealth Management Assessment

## Overview
The project follows **Clean Architecture** and **DDD** principles, with a clear separation of **Application**, **Domain**, and **Infrastructure** layers.  
Its main responsibility is to **compose and evaluate investment portfolios**, while keeping the domain independent from technical persistence details.

---

## Layers

### 1) Application
- **Orchestration**:  
  - `AssetManagementService` → Orchestrator class, responsible for coordinating use cases.  
  - `PortfolioService` → Entry point for portfolio-related operations.  
- **Configuration**: `AppConfig` centralizes configuration parameters.  
- **Models**: Data transfer objects, such as `InvestorBalanceResult`.

### 2) Domain
- **Aggregate Root**:  
  - `Portfolio` → central entity that manages the composition of investor assets.  
- **Entities**: `Investment`, `Investor`, `Quote`, `Transaction`.  
- **Services**:  
  - `StockService`, `RealEstateService`, `FondsService`.  
  - Perform only **calculations** (no repository access).  
- **Contracts**:  
  - Interfaces for repositories and data sources (`IStockRepository`, `ITransactionDataSource`, etc.).  
  - Ensure the domain layer remains independent from infrastructure.  
- **Enums**: Context descriptors (`InvestmentType`, `TransactionType`, etc.).

### 3) Infrastructure
- **DataProviders**: Implementations for different data sources.  
  - `InvestmentCsvSource`, `InvestmentJsonSource`, `InvestmentApiSource`.  
- **Repository**:  
  - `PortfolioRepository` implements repository contracts.  
- **Workload**: CSV files simulating real-world data sources.

---

## Project Tree
```text
WealthManagementAssessment
├─ Application
│  ├─ Configuration
│  │  └─ AppConfig.cs
│  ├─ Models
│  │  └─ InvestorBalanceResult.cs
│  └─ Orchestration
│     ├─ Interfaces
│     │  ├─ IAssetManagementService.cs
│     │  └─ IPortfolioService.cs
│     ├─ AssetManagementService.cs
│     └─ PortfolioService.cs
├─ Domain
│  ├─ Contracts
│  │  ├─ Repositories
│  │  │  ├─ IFondsRepository.cs
│  │  │  ├─ IInvestmentDataSource.cs
│  │  │  ├─ IPortfolioRepository.cs
│  │  │  ├─ IQuoteDataSource.cs
│  │  │  ├─ IRealEstateRepository.cs
│  │  │  ├─ IStockRepository.cs
│  │  │  └─ ITransactionDataSource.cs
│  ├─ Services
│  │  ├─ IFondsService.cs
│  │  ├─ IRealEstateService.cs
│  │  └─ IStockService.cs
│  ├─ Entities
│  │  ├─ Investment.cs
│  │  ├─ Investor.cs
│  │  ├─ Quote.cs
│  │  └─ Transaction.cs
│  └─ Enums
│     ├─ InvestmentDataSourceTypeEnum.cs
│     ├─ InvestmentTypeEnum.cs
│     ├─ InvestorProfileEnum.cs
│     └─ TransactionTypeEnum.cs
├─ Infrastructure
│  ├─ DataProviders
│  │  ├─ InvestmentApiSource.cs
│  │  ├─ InvestmentCsvSource.cs
│  │  └─ InvestmentJsonSource.cs
│  └─ Repository
│     └─ PortfolioRepository.cs
├─ Workload
│  ├─ Investments.csv
│  ├─ InvestmentsT.csv
│  ├─ Quotes.csv
│  ├─ QuotesT.csv
│  ├─ Transactions.csv
│  └─ TransactionsT.csv
├─ appsettings.json
├─ appsettings.Development.json
├─ appsettings.Production.json
├─ appsettings.Staging.json
├─ Program.cs
└─ Todos.txt
