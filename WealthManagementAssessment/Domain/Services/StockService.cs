using WealthManagementAssessment.Domain.Contracts.Services;
using WealthManagementAssessment.Domain.Entities;
using WealthManagementAssessment.Domain.Enums;

namespace WealthManagementAssessment.Domain.Services
{
    public class StockService : IStockService
    {
        public decimal CalculateStockBalanceByInvestments(List<Investment> investments)
        {
            if (investments.Count == 0)
                return 0m;

            decimal total = 0m;

            foreach (Investment inv in investments)
            {
                if (inv.InvestmentType != InvestmentTypeEnum.Stock)
                    continue;

                if (inv.Transactions.Count == 0 || inv.LatestQuote == null)
                    continue;

                decimal totalShares = inv.Transactions.Where(t => t.Type == TransactionTypeEnum.Shares).Sum(t => t.Value);
                decimal price = inv.LatestQuote.PricePerShare;

                total += totalShares * price;
            }

            return total;
        }
    }
}
