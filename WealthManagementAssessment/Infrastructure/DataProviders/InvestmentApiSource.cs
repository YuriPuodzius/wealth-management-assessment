using WealthManagementAssessment.Domain.Contracts;
using WealthManagementAssessment.Domain.Contracts.Repositories;
using WealthManagementAssessment.Domain.Entities;

namespace WealthManagementAssessment.Infrastructure.DataProviders;

public class InvestmentApiSource : IInvestmentDataSource, ITransactionDataSource, IQuoteDataSource
{
    public IReadOnlyList<Investment> GetInvestmentsByInvestorId(string investorId)
    {
        throw new NotImplementedException();
    }

    public IReadOnlyDictionary<string, IReadOnlyList<Investment>> GetInvestmentsByInvestorIds(IEnumerable<string> investorIds)
    {
        throw new NotImplementedException();
    }

    public IReadOnlyDictionary<string, IReadOnlyList<Transaction>> GetTransactionsByInvestmentIds(IEnumerable<string> investmentIds, DateTime valuationDate)
    {
        throw new NotImplementedException();
    }

    public IReadOnlyDictionary<string, IReadOnlyList<Quote>> GetQuoteByIsins(IEnumerable<string> isins, DateTime valuationDate)
    {
        throw new NotImplementedException();
    }
}