# Wealth Management Assessment

This project simulates an **investment portfolio analysis system**, allowing balance queries, investor profiling, and portfolio performance insights.  
It is built with **C# .NET**, applying clean architecture principles, layered design, and strong separation of concerns.

---

## 🚀 Features

- ✅ Check total balance by investor
- ✅ Check balance per asset type (Real Estate, Stocks, Funds)
- ✅ Identify investor profile (Conservative, Moderate, Aggressive)
- ✅ Aggregate balances across all investors
- ✅ Analyze brokerage profile based on investor distribution
- ✅ Performance analysis with execution time metrics

---

## 🏗️ Architecture

The project follows a **Clean/DDD-inspired layered architecture**:

- **Domain** → Core business logic (Entities, Enums, Contracts).
- **Application** → Orchestration layer (use cases, configuration, models).
- **Infrastructure** → Data providers and repository implementations.
- **Workload** → Input dataset files (CSV) with investments, quotes, and transactions.

Detailed architecture explanation is in [`ARCHITECTURE.md`](ARCHITECTURE.md).

---

## 📂 Project Structure

<details>
  <summary><b>📂 Project Structure</b></summary>

```text
WealthManagementAssessment/
├─ Application/
│  ├─ Configuration/
│  │  └─ AppConfig.cs
│  │     (App settings and configs)
│  ├─ Models/
│  │  └─ InvestorBalanceResult.cs
│  │     (DTO/result objects)
│  └─ Orchestration/
│     ├─ Interfaces/
│     │  ├─ IAssetManagementService.cs
│     │  └─ IPortfolioService.cs
│     ├─ AssetManagementService.cs
│     └─ PortfolioService.cs
│        (Orchestrators / Application layer entrypoints)
├─ Domain/
│  ├─ Contracts/
│  │  └─ Repositories/
│  │     ├─ IFondsRepository.cs
│  │     ├─ IInvestmentDataSource.cs
│  │     ├─ IPortfolioRepository.cs
│  │     ├─ IQuoteDataSource.cs
│  │     ├─ IRealEstateRepository.cs
│  │     ├─ IStockRepository.cs
│  │     └─ ITransactionDataSource.cs
│        (Repository interfaces / abstractions)
│  ├─ Services/
│  │  ├─ IFondsService.cs
│  │  ├─ IRealEstateService.cs
│  │  └─ IStockService.cs
│        (Domain services - pure calculations)
│  ├─ Entities/
│  │  ├─ Investment.cs
│  │  ├─ Investor.cs
│  │  ├─ Quote.cs
│  │  └─ Transaction.cs
│        (Core business entities)
│  └─ Enums/
│     ├─ InvestmentDataSourceTypeEnum.cs
│     ├─ InvestmentTypeEnum.cs
│     ├─ InvestorProfileEnum.cs
│     └─ TransactionTypeEnum.cs
│        (Context enums / type definitions)
├─ Infrastructure/
│  ├─ DataProviders/
│  │  ├─ InvestmentApiSource.cs
│  │  ├─ InvestmentCsvSource.cs
│  │  └─ InvestmentJsonSource.cs
│        (CSV / JSON / API input providers)
│  └─ Repository/
│     └─ PortfolioRepository.cs
│        (Repository implementations)
├─ Workload/
│  ├─ Investments.csv
│  ├─ InvestmentsT.csv
│  ├─ Quotes.csv
│  ├─ QuotesT.csv
│  ├─ Transactions.csv
│  └─ TransactionsT.csv
│        (Sample dataset files)
├─ Program.cs
│  (Application entrypoint)
├─ appsettings.json
├─ appsettings.Development.json
├─ appsettings.Production.json
└─ appsettings.Staging.json
   (Environment configs)
```
</details>

---

## ⚙️ Requirements

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download)
- Rider / Visual Studio / VS Code

---

## ▶️ Running

```bash
dotnet run --project WealthManagementAssessment

1 - Check total balance invested by investor
2 - Check investment balance by asset type per investor
3 - Check investor profile (Conservative / Moderate / Aggressive)
4 - Check total balance across all investors
5 - Check brokerage profile based on investors
6 - Exit

[1] Checking balance for investor Investor90 at 2025-09-15...

Performance Analysis:
⏱️ HydrateFondsInvestorsPortfolios executed in 25,34 seconds (0,42 minutes).

✅ Balance analysis completed in 26,70 seconds.

🏢 Real Estate balance: 62.199.104,00 Euros.
📈 Stock balance: 1.347.195,06 Euros.
💰 Fonds balance: 943.709.182,95 Euros.
💼 Total wallet value: 1.007.255.482,01 Euros.
