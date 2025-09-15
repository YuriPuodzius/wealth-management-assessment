using WealthManagementAssessment.Domain.Contracts.Services;
using WealthManagementAssessment.Domain.Entities;
using WealthManagementAssessment.Domain.Enums;

namespace WealthManagementAssessment.Domain.Services
{
    public class RealEstateService : IRealEstateService
    {
        public decimal CalculateRealEstateBalanceByInvestments(List<Investment> investorInvestiments)
        {
            if (investorInvestiments.Count == 0)
                return 0m;

            // Consider only Real Estate investments. Transactions are assumed pre-filtered (<= valuationDate) during hydration.
            IEnumerable<Investment> realEstates = investorInvestiments.Where(i => i.InvestmentType == InvestmentTypeEnum.RealEstate);

            decimal total = 0m;
            foreach (Investment inv in realEstates)
            {
                if (inv.Transactions.Count == 0)
                    continue;

                // For RealEstate, treat transaction Value as monetary contribution (positive buys, negative sells)
                decimal netAmount = inv.Transactions
                    .Where(t => t.Type == TransactionTypeEnum.Estate || t.Type == TransactionTypeEnum.Building)
                    .Sum(t => t.Value);
                total += netAmount;
            }

            return total;
        }
    }
}
