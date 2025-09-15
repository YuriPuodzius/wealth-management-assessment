using WealthManagementAssessment.Domain.Entities;

namespace WealthManagementAssessment.Domain.Contracts.Repositories;

public interface ITransactionDataSource
{
    IReadOnlyDictionary<string, IReadOnlyList<Transaction>> GetTransactionsByInvestmentIds(IEnumerable<string> investmentIds, DateTime valuationDate);
}