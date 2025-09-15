using WealthManagementAssessment.Domain.Entities;

namespace WealthManagementAssessment.Domain.Contracts.Repositories;

public interface IQuoteDataSource
{
    IReadOnlyDictionary<string, IReadOnlyList<Quote>> GetQuoteByIsins(IEnumerable<string> isins, DateTime valuationDate);
}