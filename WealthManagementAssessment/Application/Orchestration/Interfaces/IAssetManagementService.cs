using WealthManagementAssessment.Application.Models;
using WealthManagementAssessment.Domain.Enums;

namespace WealthManagementAssessment.Application.Orchestration.Interfaces
{
    public interface IAssetManagementService
    {
        InvestorBalanceResult GetTotalBalanceByInvestor(string investorId, DateTime valuationDate);

        InvestorBalanceResult GetTotalBalanceByInvestorAndInvestmentType(string investorId, DateTime valuationDate, InvestmentTypeEnum investmentType);

        bool InvestorExists(string investorId);
        bool HasTransactionsForValuationDate(string investorId, DateTime valuationDate);
        InvestorProfileEnum GetProfileByInvestor(string investorId, DateTime valuationDate);
    }
}