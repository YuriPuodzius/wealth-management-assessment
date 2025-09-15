using WealthManagementAssessment.Domain.Entities;

namespace WealthManagementAssessment.Domain.Contracts.Repositories;

public interface IInvestmentDataSource
{
    IReadOnlyList<Investment> GetInvestmentsByInvestorId(string investorId);
    IReadOnlyDictionary<string, IReadOnlyList<Transaction>> GetTransactionsByInvestmentIds(IEnumerable<string> investmentIds, DateTime valuationDate);
    IReadOnlyDictionary<string, IReadOnlyList<Investment>> GetInvestmentsByInvestorIds(IEnumerable<string> investorIds);
}