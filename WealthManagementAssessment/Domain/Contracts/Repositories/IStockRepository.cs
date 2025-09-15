using WealthManagementAssessment.Domain.Entities;

namespace WealthManagementAssessment.Domain.Contracts.Repositories;

public interface IStockRepository
{
    List<Investment> GetStockTransactionsByInvestments(List<Investment> investments);
}