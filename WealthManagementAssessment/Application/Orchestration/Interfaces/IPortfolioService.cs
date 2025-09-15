using WealthManagementAssessment.Domain.Entities;
using WealthManagementAssessment.Domain.Enums;

namespace WealthManagementAssessment.Application.Orchestration.Interfaces;

public interface IPortfolioService
{
    IReadOnlyList<Investment> GetInvestmentsByInvestor(string investorId);

    void LoadTransactionsByInvestments(List<Investment> investments, DateTime valuationDate, InvestmentTypeEnum? investmentType = null);

    void LoadQuotesByStockAsset(List<Investment> stockInvestments, DateTime valuationDate);
    IReadOnlyList<Investment> GetInvestmentsByInvestorAndInvestmentType(string investorId, InvestmentTypeEnum investmentType);
}