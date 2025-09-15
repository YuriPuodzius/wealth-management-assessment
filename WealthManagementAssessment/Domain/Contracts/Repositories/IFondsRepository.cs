using WealthManagementAssessment.Domain.Entities;

namespace WealthManagementAssessment.Domain.Contracts.Repositories;

public interface IFondsRepository
{
    List<Investment> GetFondTransactionsByInvestments(List<Investment> investments);
}