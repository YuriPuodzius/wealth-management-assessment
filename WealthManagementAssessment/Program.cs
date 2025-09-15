using System.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WealthManagementAssessment.Application.Configuration;
using WealthManagementAssessment.Application.Models;
using WealthManagementAssessment.Application.Orchestration;
using WealthManagementAssessment.Application.Orchestration.Interfaces;
using WealthManagementAssessment.Domain.Contracts.Repositories;
using WealthManagementAssessment.Domain.Contracts.Services;
using WealthManagementAssessment.Domain.Enums;
using WealthManagementAssessment.Domain.Services;
using WealthManagementAssessment.Infrastructure.DataProviders;
using WealthManagementAssessment.Infrastructure.Repository;

namespace WealthManagementAssessment;

class Program
{
    private static void Main(string[] args)
    { 
        

        var host = Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((ctx, cfg) =>
            {
                cfg.SetBasePath(AppContext.BaseDirectory);

                cfg.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                cfg.AddJsonFile($"appsettings.{ctx.HostingEnvironment.EnvironmentName}.json", optional: true, reloadOnChange: true);

                cfg.AddEnvironmentVariables();

            })
            .ConfigureServices((ctx, services) =>
            {
                var dataBindingsSection = ctx.Configuration.GetSection("DataBindings");
                services.Configure<AppConfig>(options =>
                {
                    dataBindingsSection.Bind(options.DataBindings);
                });

                services.AddSingleton<IAssetManagementService, AssetManagementService>();
                
                services.AddSingleton<IPortfolioService, PortfolioService>();
                services.AddSingleton<IPortfolioRepository, PortfolioRepository>();
                
                services.AddSingleton<IRealEstateService, RealEstateService>();
                services.AddSingleton<IStockService, StockService>();
                services.AddSingleton<IFondsService, FondsService>();
                
                var dataBindings = dataBindingsSection.Get<DataBindingsConfig>();

                switch (dataBindings?.InvestmentDataSourceType)
                {
                    case InvestmentDataSourceTypeEnum.Csv:
                        services.AddSingleton<InvestmentCsvSource>();
                        services.AddSingleton<IInvestmentDataSource>(sp => sp.GetRequiredService<InvestmentCsvSource>());
                        services.AddSingleton<ITransactionDataSource>(sp => sp.GetRequiredService<InvestmentCsvSource>());
                        services.AddSingleton<IQuoteDataSource>(sp => sp.GetRequiredService<InvestmentCsvSource>());
                        break;
                    case InvestmentDataSourceTypeEnum.Json:
                        services.AddSingleton<InvestmentJsonSource>();
                        services.AddSingleton<IInvestmentDataSource>(sp => sp.GetRequiredService<InvestmentJsonSource>());
                        services.AddSingleton<ITransactionDataSource>(sp => sp.GetRequiredService<InvestmentJsonSource>());
                        services.AddSingleton<IQuoteDataSource>(sp => sp.GetRequiredService<InvestmentJsonSource>());
                        break;
                    case InvestmentDataSourceTypeEnum.Api:
                        services.AddSingleton<InvestmentApiSource>();
                        services.AddSingleton<IInvestmentDataSource>(sp => sp.GetRequiredService<InvestmentApiSource>());
                        services.AddSingleton<ITransactionDataSource>(sp => sp.GetRequiredService<InvestmentApiSource>());
                        services.AddSingleton<IQuoteDataSource>(sp => sp.GetRequiredService<InvestmentApiSource>());
                        break;
                    default:
                        throw new InvalidOperationException("Unknown InvestmentDataSource configuration value.");
                }
            })
            .Build();

        IAssetManagementService assetManagementService = host.Services.GetRequiredService<IAssetManagementService>();

        string greeting = DateTime.Now.Hour < 12 ? "Good morning" : DateTime.Now.Hour < 18 ? "Good afternoon" : "Good evening";
        
        while (true)
        {
            Console.WriteLine($"\n=== {greeting}, welcome to the Wealth Management Assessment! ===");
            Console.WriteLine($"\n=== Please select the operation you would like to perform ===\n");
            Console.WriteLine("1 - Check total balance invested by investor");
            Console.WriteLine("2 - Check investment balance by asset type per investor");
            Console.WriteLine("3 - Check investor profile (Conservative / Moderate / Aggressive)");
            Console.WriteLine("4 - Check total balance across all investors");
            Console.WriteLine("5 - Check brokerage profile based on investors (Conservative / Moderate / Aggressive)");
            Console.WriteLine("6 - Exit");

            string choice;
            while (true)
            {
                Console.Write("\nEnter your choice: ");
                choice = (Console.ReadLine() ?? string.Empty).Trim();
                if (choice is "1" or "2" or "3" or "4" or "5" or "6") break;
                Console.WriteLine("Invalid choice. Please enter a number from 1 to 6.");
            }

            switch (choice)
            {
                case "1":
                {
                    string investorId = string.Empty;
                    while (true)
                    {
                        Console.Write("Enter investor ID (required, or '#' to return to the main menu): ");
                        investorId = (Console.ReadLine() ?? string.Empty).Trim();

                        if (investorId == "#")
                            goto EndCase1; // back to main menu

                        if (string.IsNullOrWhiteSpace(investorId))
                        {
                            Console.WriteLine("Investor ID is required. Please try again.");
                            continue;
                        }

                        if (!assetManagementService.InvestorExists(investorId))
                        {
                            Console.WriteLine($"Investor '{investorId}' was not found or has no investments. Please enter a valid investor ID or '#' to return to the menu.");
                            continue;
                        }

                        break; // valid investor
                    }

                    DateTime valuationDate;
                    while (true)
                    {
                        Console.Write("Enter date (optional, format: yyyy-MM-dd, or '#' to return to the main menu): ");
                        string? valuationDateInput = Console.ReadLine();

                        if ((valuationDateInput ?? string.Empty).Trim() == "#")
                            goto EndCase1; // back to main menu

                        if (string.IsNullOrWhiteSpace(valuationDateInput))
                        {
                            valuationDate = DateTime.Today;
                        }
                        else if (!DateTime.TryParse(valuationDateInput, out valuationDate))
                        {
                            Console.WriteLine("Invalid date. Please use yyyy-MM-dd, leave blank for today, or '#' to return to the menu.");
                            continue;
                        }

                        if (!assetManagementService.HasTransactionsForValuationDate(investorId, valuationDate))
                        {
                            Console.WriteLine($"No transactions found for the informed valuation date ({valuationDate:yyyy-MM-dd}). Please review the date and try again, or enter '#' to return to the menu.");
                            continue;
                        }

                        break; // valid date with transactions
                    }

                    Console.WriteLine($"\n[1] Checking balance for investor {investorId} at {valuationDate:yyyy-MM-dd}...");
                    Console.WriteLine("\nPerformance Analysis: 🐢vs 🐇");

                    Stopwatch stopwatch = Stopwatch.StartNew();
                    InvestorBalanceResult balanceResult = assetManagementService.GetTotalBalanceByInvestor(investorId, valuationDate);
                    stopwatch.Stop();
                    Console.WriteLine($"\n✅ Balance analysis for investor {investorId} at {valuationDate:yyyy-MM-dd} completed. | ⏱️ {stopwatch.Elapsed.TotalSeconds:F2} seconds ({stopwatch.Elapsed.TotalMinutes:F2} minutes).");
                    Console.WriteLine($"\n🏢Real Estate balance: {balanceResult.RealEstateBalance:N2} Euros.");
                    Console.WriteLine($"📈Stock balance: {balanceResult.StockBalance:N2} Euros.");
                    Console.WriteLine($"💰Fonds balance: {balanceResult.FondsBalance:N2} Euros.");
                    Console.WriteLine($"💼Total wallet value: {balanceResult.TotalBalance:N2} Euros.\n");
                    
                    // Wait here until the user decides to return to the main menu
                    while (true)
                    {
                        Console.Write("Enter '#' to return to the main menu: ");
                        string? goBack = Console.ReadLine();
                        if ((goBack ?? string.Empty).Trim() == "#")
                            break;
                    }

                    EndCase1:
                    break;
                }
                case "2":
                {
                    string investorId = string.Empty;
                    while (true)
                    {
                        Console.Write("Enter investor ID (required, or '#' to return to the main menu): ");
                        investorId = (Console.ReadLine() ?? string.Empty).Trim();

                        if (investorId == "#")
                            goto EndCase2; // back to main menu

                        if (string.IsNullOrWhiteSpace(investorId))
                        {
                            Console.WriteLine("Investor ID is required. Please try again.");
                            continue;
                        }

                        if (!assetManagementService.InvestorExists(investorId))
                        {
                            Console.WriteLine($"Investor '{investorId}' was not found or has no investments. Please enter a valid investor ID or '#' to return to the menu.");
                            continue;
                        }

                        break; // valid investor
                    }

                    InvestmentTypeEnum investmentType;
                    while (true)
                    {
                        Console.Write("Select investment type (1 = Stock, 2 = Real Estate, 3 = Fond, or '#' to return to the main menu): ");
                        var typeInput = (Console.ReadLine() ?? string.Empty).Trim();

                        if (typeInput == "#")
                            goto EndCase2; // back to main menu

                        if (int.TryParse(typeInput, out int typeCode) && Enum.IsDefined(typeof(InvestmentTypeEnum), typeCode))
                        {
                            investmentType = (InvestmentTypeEnum)typeCode;
                            break;
                        }
                        Console.WriteLine("Invalid type. Please enter 1, 2 or 3, or '#' to return to the menu.");
                    }

                    DateTime valuationDate;
                    while (true)
                    {
                        Console.Write("Enter date (optional, format: yyyy-MM-dd, or '#' to return to the main menu): ");
                        var valuationDateInput = Console.ReadLine();

                        if ((valuationDateInput ?? string.Empty).Trim() == "#")
                            goto EndCase2; // back to main menu

                        if (string.IsNullOrWhiteSpace(valuationDateInput))
                        {
                            valuationDate = DateTime.Today;
                        }
                        else if (!DateTime.TryParse(valuationDateInput, out valuationDate))
                        {
                            Console.WriteLine("Invalid date. Please use yyyy-MM-dd, leave blank for today, or '#' to return to the menu.");
                            continue;
                        }

                        if (!assetManagementService.HasTransactionsForValuationDate(investorId, valuationDate))
                        {
                            Console.WriteLine($"No transactions found for the informed valuation date ({valuationDate:yyyy-MM-dd}). Please review the date and try again, or enter '#' to return to the menu.");
                            continue;
                        }

                        break; // valid date with transactions
                    }

                    string investmentTypeLabel = investmentType switch
                    {
                        InvestmentTypeEnum.Stock => "Stock",
                        InvestmentTypeEnum.RealEstate => "Real Estate",
                        InvestmentTypeEnum.Fonds => "Fonds",
                        _ => "Unknown"
                    };

                    Console.WriteLine($"[2] Checking {investmentTypeLabel} balance for investor {investorId} at {valuationDate:yyyy-MM-dd}\n");
                    Console.WriteLine("\nPerformance Analysis: 🐢vs 🐇");
                    
                    Stopwatch stopwatch = Stopwatch.StartNew();
                    InvestorBalanceResult balanceResult = assetManagementService.GetTotalBalanceByInvestorAndInvestmentType(investorId, valuationDate, investmentType);
                    stopwatch.Stop();

                    Console.WriteLine($"\n✅ Balance analysis for investor {investorId} at {valuationDate:yyyy-MM-dd} completed. | ⏱️ {stopwatch.Elapsed.TotalSeconds:F2} seconds ({stopwatch.Elapsed.TotalMinutes:F2} minutes).");
                    Console.WriteLine($"\n💼 Total wallet for asset {investmentTypeLabel} is: {balanceResult.TotalBalance:N2} Euros.\n");
                    
                    // Wait here until the user decides to return to the main menu
                    while (true)
                    {
                        Console.Write("Enter '#' to return to the main menu: ");
                        string? goBack = Console.ReadLine();
                        if ((goBack ?? string.Empty).Trim() == "#")
                            break;
                    }

                    EndCase2:
                    break;
                }
                case "3":
                {
                    string investorId;
                    while (true)
                    {
                        Console.Write("Enter investor ID (required, or '#' to return to the main menu): ");
                        investorId = (Console.ReadLine() ?? string.Empty).Trim();

                        if (investorId == "#")
                            goto EndCase3; // back to main menu

                        if (string.IsNullOrWhiteSpace(investorId))
                        {
                            Console.WriteLine("Investor ID is required. Please try again.");
                            continue;
                        }

                        if (!assetManagementService.InvestorExists(investorId))
                        {
                            Console.WriteLine($"Investor '{investorId}' was not found or has no investments. Please enter a valid investor ID or '#' to return to the menu.");
                            continue;
                        }

                        break;
                    }

                    DateTime valuationDate;
                    while (true)
                    {
                        Console.Write("Enter date (optional, format: yyyy-MM-dd, or '#' to return to the main menu): ");
                        string? valuationDateInput = Console.ReadLine();

                        if ((valuationDateInput ?? string.Empty).Trim() == "#")
                            goto EndCase3; // back to main menu

                        if (string.IsNullOrWhiteSpace(valuationDateInput))
                        {
                            valuationDate = DateTime.Today;
                        }
                        else if (!DateTime.TryParse(valuationDateInput, out valuationDate))
                        {
                            Console.WriteLine("Invalid date. Please use yyyy-MM-dd, leave blank for today, or '#' to return to the menu.");
                            continue;
                        }

                        if (!assetManagementService.HasTransactionsForValuationDate(investorId, valuationDate))
                        {
                            Console.WriteLine($"No transactions found for the informed valuation date ({valuationDate:yyyy-MM-dd}). Please review the date and try again, or enter '#' to return to the menu.");
                            continue;
                        }

                        break; // valid date with transactions
                    }

                    Console.WriteLine($"[3] Checking profile for investor {investorId} at {valuationDate:yyyy-MM-dd}");
                    // Wait here until the user decides to return to the main menu
                    while (true)
                    {
                        Console.Write("Enter '#' to return to the main menu: ");
                        string? goBack = Console.ReadLine();
                        if ((goBack ?? string.Empty).Trim() == "#")
                            break;
                    }
                    EndCase3:
                    break;
                }
                case "4":
                {
                    DateTime date;
                    while (true)
                    {
                        Console.Write("Enter date (optional, format: yyyy-MM-dd, or '#' to return to the main menu): ");
                        string? input = Console.ReadLine();

                        if ((input ?? string.Empty).Trim() == "#")
                            goto EndCase4; // back to main menu

                        if (string.IsNullOrWhiteSpace(input)) { date = DateTime.Today; break; }
                        if (DateTime.TryParse(input, out date)) break;
                        Console.WriteLine("Invalid date. Please use yyyy-MM-dd, leave blank for today, or '#' to return to the menu.");
                    }

                    Console.WriteLine($"[4] Checking total balance across all investors at {date:yyyy-MM-dd}");
                    // Wait here until the user decides to return to the main menu
                    while (true)
                    {
                        Console.Write("Enter '#' to return to the main menu: ");
                        string? goBack = Console.ReadLine();
                        if ((goBack ?? string.Empty).Trim() == "#")
                            break;
                    }
                    EndCase4:
                    break;
                }
                case "5":
                {
                    DateTime date;
                    while (true)
                    {
                        Console.Write("Enter date (optional, format: yyyy-MM-dd, or '#' to return to the main menu): ");
                        string? input = Console.ReadLine();

                        if ((input ?? string.Empty).Trim() == "#")
                            goto EndCase5; // back to main menu

                        if (string.IsNullOrWhiteSpace(input)) { date = DateTime.Today; break; }
                        if (DateTime.TryParse(input, out date)) break;
                        Console.WriteLine("Invalid date. Please use yyyy-MM-dd, leave blank for today, or '#' to return to the menu.");
                    }

                    Console.WriteLine($"[5] Checking brokerage profile based on investors at {date:yyyy-MM-dd}");
                    // Wait here until the user decides to return to the main menu
                    while (true)
                    {
                        Console.Write("Enter '#' to return to the main menu: ");
                        string? goBack = Console.ReadLine();
                        if ((goBack ?? string.Empty).Trim() == "#")
                            break;
                    }
                    EndCase5:
                    break;
                }
                default:
                    Console.WriteLine("Invalid choice.");
                    break;
            }
        }
    }
}