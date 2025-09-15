using WealthManagementAssessment.Domain.Entities;

namespace WealthManagementAssessment.Domain.Contracts.Services
{
    public interface IStockService
    {
        decimal CalculateStockBalanceByInvestments(List<Investment> investments);
    }
}
