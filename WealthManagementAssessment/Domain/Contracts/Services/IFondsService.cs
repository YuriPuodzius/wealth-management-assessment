using WealthManagementAssessment.Domain.Entities;

namespace WealthManagementAssessment.Domain.Contracts.Services
{
    public interface IFondsService
    {
        decimal CalculateFondBalanceByInvestments(List<Investment> investments);
    }
}