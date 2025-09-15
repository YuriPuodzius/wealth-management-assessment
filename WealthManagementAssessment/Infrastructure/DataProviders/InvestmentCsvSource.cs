using WealthManagementAssessment.Domain.Enums;
using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Extensions.Options;
using WealthManagementAssessment.Application.Configuration;
using WealthManagementAssessment.Domain.Contracts;
using WealthManagementAssessment.Domain.Contracts.Repositories;
using WealthManagementAssessment.Domain.Entities;

namespace WealthManagementAssessment.Infrastructure.DataProviders;

public class InvestmentCsvSource : IInvestmentDataSource, ITransactionDataSource, IQuoteDataSource
{
    private readonly AppConfig _appConfig;

    private static readonly CsvConfiguration CsvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
    {
        Delimiter = ";",
        HasHeaderRecord = true,
        TrimOptions = TrimOptions.Trim,
    };

    public InvestmentCsvSource(IOptions<AppConfig> appConfig)
    {
        _appConfig = appConfig.Value;
    }
    public IReadOnlyList<Investment> GetInvestmentsByInvestorId(string investorId)
    {
        List<Investment> investments;

        using (var reader = new StreamReader(Path.Combine(_appConfig.ProjectDirectory, _appConfig.DataBindings.CsvConfig.InvestmentsPath)))
        using (var csv = new CsvReader(reader, CsvConfig))
        {
            csv.Context.TypeConverterOptionsCache
                .GetOptions<InvestmentTypeEnum>()
                .EnumIgnoreCase = true;
            investments = csv.GetRecords<Investment>()
                .Where(inv => string.Equals(inv.InvestorId?.Trim(), investorId?.Trim(), StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        return investments;
    }

    public IReadOnlyDictionary<string, IReadOnlyList<Investment>> GetInvestmentsByInvestorIds(IEnumerable<string> investorIds)
    {
        HashSet<string> investorIdSet = new HashSet<string>(investorIds.Select(id => id.Trim()), StringComparer.OrdinalIgnoreCase);
        List<Investment> investments;

        using (var reader = new StreamReader(Path.Combine(_appConfig.ProjectDirectory, _appConfig.DataBindings.CsvConfig.InvestmentsPath)))
        using (var csv = new CsvReader(reader, CsvConfig))
        {
            csv.Context.TypeConverterOptionsCache
                .GetOptions<InvestmentTypeEnum>()
                .EnumIgnoreCase = true;
            investments = csv.GetRecords<Investment>()
                .Where(inv => investorIdSet.Contains(inv.InvestorId?.Trim() ?? string.Empty))
                .ToList();
        }

        var grouped = investments
            .GroupBy(inv => inv.InvestorId)
            .ToDictionary(g => g.Key, g => (IReadOnlyList<Investment>)g.ToList());

        return grouped;
    }

    public IReadOnlyDictionary<string, IReadOnlyList<Transaction>> GetTransactionsByInvestmentIds(IEnumerable<string> investmentIds, DateTime valuationDate)
    {
        HashSet<string> investmentIdSet = new HashSet<string>(investmentIds.Select(id => id.Trim()), StringComparer.OrdinalIgnoreCase);
        List<Transaction> transactions;

        using (StreamReader reader = new StreamReader(Path.Combine(_appConfig.ProjectDirectory, _appConfig.DataBindings.CsvConfig.TransactionsPath)))
        using (CsvReader csv = new CsvReader(reader, CsvConfig))
        {
            csv.Context.TypeConverterOptionsCache
                .GetOptions<TransactionTypeEnum>()
                .EnumIgnoreCase = true;
            transactions = csv.GetRecords<Transaction>()
                .Where(tx => tx.Date <= valuationDate && investmentIdSet.Contains((tx.InvestmentId ?? string.Empty).Trim()))
                .ToList();
        }
        
        // IEnumerable<IGrouping<string, Transaction>> groupsByInvestmentId = transactions.GroupBy(tx => tx.InvestmentId, StringComparer.OrdinalIgnoreCase);
        // Dictionary<string, IReadOnlyList<Transaction>> transactionsByInvestmentId = new Dictionary<string, IReadOnlyList<Transaction>>(StringComparer.OrdinalIgnoreCase);
        //
        // foreach (IGrouping<string, Transaction> group in groupsByInvestmentId)
        // {
        //     List<Transaction> orderedTransactions = group.OrderBy(tx => tx.Date).ToList();
        //     transactionsByInvestmentId[group.Key] = orderedTransactions;
        // }
        
        // Group transactions by InvestmentId and build a dictionary
        Dictionary<string, IReadOnlyList<Transaction>> transactionsByInvestmentId = transactions
            .GroupBy(tx => tx.InvestmentId, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(g => g.Key, g=> (IReadOnlyList<Transaction>)g.OrderBy(tx => tx.Date).ToList());
        
        return transactionsByInvestmentId;
    }

    public IReadOnlyDictionary<string, IReadOnlyList<Quote>> GetQuoteByIsins(IEnumerable<string> isins, DateTime valuationDate)
    {
        HashSet<string> isinSet = new HashSet<string>(isins.Select(s => s.Trim()), StringComparer.OrdinalIgnoreCase);
        List<Quote> quotes;

        using (var reader = new StreamReader(Path.Combine(_appConfig.ProjectDirectory, _appConfig.DataBindings.CsvConfig.QuotesPath)))
        using (var csv = new CsvReader(reader, CsvConfig))
        {
            quotes = csv.GetRecords<Quote>().Where(q => isinSet.Contains(q.ISIN.Trim()) && q.Date <= valuationDate).ToList();
        }

        Dictionary<string, IReadOnlyList<Quote>> quotesByIsin = quotes.GroupBy(q => q.ISIN, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(g => g.Key, g 
                    => (IReadOnlyList<Quote>) g.OrderByDescending(q => q.Date).ToList()
                );

        return quotesByIsin;
    }
}