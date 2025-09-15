using WealthManagementAssessment.Application.Orchestration.Interfaces;
using WealthManagementAssessment.Domain.Contracts.Repositories;
using WealthManagementAssessment.Domain.Entities;
using WealthManagementAssessment.Domain.Enums;

namespace WealthManagementAssessment.Application.Orchestration;

public class PortfolioService : IPortfolioService
{
    private readonly IPortfolioRepository _portfolioRepository;

    public PortfolioService(IPortfolioRepository portfolioRepository)
    {
        _portfolioRepository = portfolioRepository;
    }

    public IReadOnlyList<Investment> GetInvestmentsByInvestor(string investorId)
    {
        IReadOnlyList<Investment> investments = _portfolioRepository.GetInvestmentsByInvestorId(investorId);
        return investments;
    }

    public void LoadTransactionsByInvestments(List<Investment> investments, 
        DateTime valuationDate, InvestmentTypeEnum? investmentType = null) 
        => _portfolioRepository.LoadTransactionsByInvestments(investments, valuationDate);
    
    public void LoadQuotesByStockAsset(List<Investment> stockInvestments, DateTime valuationDate)
        => _portfolioRepository.LoadQuotesByStockAsset(stockInvestments, valuationDate);
    
    public IReadOnlyList<Investment> GetInvestmentsByInvestorAndInvestmentType(string investorId,
        InvestmentTypeEnum investmentType)
    {
        throw new NotImplementedException();
    }
}