using WealthManagementAssessment.Domain.Entities;

namespace WealthManagementAssessment.Domain.Contracts.Repositories;

public interface IRealEstateRepository
{
    List<Transaction> GetRealStateTransactionsByInvestments(List<Investment> investments);
}