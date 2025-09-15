using WealthManagementAssessment.Domain.Entities;

namespace WealthManagementAssessment.Domain.Contracts.Services
{
    public interface IRealEstateService
    {
        decimal CalculateRealEstateBalanceByInvestments(List<Investment> investments);
    }
}
