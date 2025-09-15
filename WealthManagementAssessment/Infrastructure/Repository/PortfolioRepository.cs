using System.Diagnostics;
using WealthManagementAssessment.Domain.Contracts;
using WealthManagementAssessment.Domain.Contracts.Repositories;
using WealthManagementAssessment.Domain.Entities;
using WealthManagementAssessment.Domain.Enums;

namespace WealthManagementAssessment.Infrastructure.Repository;

public class PortfolioRepository : IPortfolioRepository
{
    private readonly IInvestmentDataSource _investmentDataSource;
    private readonly ITransactionDataSource _transactionDataSource;
    private readonly IQuoteDataSource _quoteDataSource;

    public PortfolioRepository(IInvestmentDataSource investmentDataSource, ITransactionDataSource transactionDataSource,
        IQuoteDataSource quoteDataSource)
    {
        _investmentDataSource = investmentDataSource;
        _transactionDataSource = transactionDataSource;
        _quoteDataSource = quoteDataSource;
    }

    public IReadOnlyList<Investment> GetInvestmentsByInvestorId(string investorId)
    {
        Stopwatch stopwatch = Stopwatch.StartNew();
        IReadOnlyList<Investment> investments = _investmentDataSource.GetInvestmentsByInvestorId(investorId);
        stopwatch.Stop();
        return investments;
        
    }

    public void LoadTransactionsByInvestments(List<Investment> investments, DateTime valuationDate,
        InvestmentTypeEnum? investmentType = null)
    {
        Stopwatch stopwatch = Stopwatch.StartNew();
        IEnumerable<Investment> filtered = investmentType.HasValue
            ? investments.Where(i => i.InvestmentType == investmentType.Value)
            : investments;
        List<string> investmentIds = filtered.Select(i => i.InvestmentId).ToList();

        IReadOnlyDictionary<string, IReadOnlyList<Transaction>> transactionsByInvestmentIds =
            _transactionDataSource.GetTransactionsByInvestmentIds(investmentIds, valuationDate);

        // List<Transaction> allTransactions = new List<Transaction>();
        foreach (var inv in investments)
        {
            if (transactionsByInvestmentIds.TryGetValue(inv.InvestmentId, out var txs))
                inv.Transactions = txs.ToList();
                // allTransactions.AddRange(txs);
            else
                inv.Transactions = new List<Transaction>();
        }
        stopwatch.Stop();
        // return allTransactions;
    }

    public void LoadQuotesByStockAsset(List<Investment> stocks, DateTime valuationDate)
    {
        Stopwatch stopwatch = Stopwatch.StartNew();
        // Collect distinct ISINs
        List<string> isins = stocks.Select(i => i.ISIN?.Trim()).Where(s => !string.IsNullOrWhiteSpace(s))
            .Distinct(StringComparer.OrdinalIgnoreCase).ToList()!;

        // Fetch quotes grouped by ISIN up to valuationDate
        IReadOnlyDictionary<string, IReadOnlyList<Quote>> quotesByIsin = _quoteDataSource.GetQuoteByIsins(isins, valuationDate);

        foreach (Investment investment in stocks)
        {
            string isin = investment.ISIN?.Trim() ?? string.Empty;

            if (!quotesByIsin.TryGetValue(isin, out IReadOnlyList<Quote>? quotes) || quotes.Count == 0)
            {
                investment.LatestQuote = null;
                continue;
            }

            // Pick the latest quote <= valuationDate; if none, fallback to the most recent available
            Quote? latest = quotes
                .Where(q => q.Date <= valuationDate).OrderByDescending(q => q.Date).FirstOrDefault() 
                            ?? quotes.OrderByDescending(q => q.Date).FirstOrDefault();

            investment.LatestQuote = latest;
        }
        stopwatch.Stop();
    }

    public IReadOnlyList<Investment> GetInvestmentsByInvestorAndInvestmentType(string investorId,
        InvestmentTypeEnum investmentType)
    {
        throw new NotImplementedException();
    }
}