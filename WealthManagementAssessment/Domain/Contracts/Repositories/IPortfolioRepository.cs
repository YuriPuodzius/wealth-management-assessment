using WealthManagementAssessment.Domain.Entities;
using WealthManagementAssessment.Domain.Enums;

namespace WealthManagementAssessment.Domain.Contracts.Repositories;

public interface IPortfolioRepository
{
    IReadOnlyList<Investment> GetInvestmentsByInvestorId(string investorId);
    void LoadTransactionsByInvestments(List<Investment> investments, DateTime valuationDate, InvestmentTypeEnum? investmentType = null);
    void LoadQuotesByStockAsset(List<Investment> stockInvestments, DateTime valuationDate);
    IReadOnlyList<Investment> GetInvestmentsByInvestorAndInvestmentType(string investorId, InvestmentTypeEnum investmentType);
}