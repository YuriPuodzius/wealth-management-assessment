using WealthManagementAssessment.Domain.Contracts.Services;
using WealthManagementAssessment.Domain.Entities;
using WealthManagementAssessment.Domain.Enums;

namespace WealthManagementAssessment.Domain.Services
{
    public class FondsService : IFondsService
    {
        private readonly IRealEstateService _realEstateService;
        private readonly IStockService _stockService;

        public FondsService(IRealEstateService realEstateService, IStockService stockService)
        {
            _realEstateService = realEstateService;
            _stockService = stockService;
        }
        public decimal CalculateFondBalanceByInvestments(List<Investment> investments)
        {
            if (investments.Count == 0)
                return 0m;

            // Index all investments by their owner (InvestorId) to resolve Fonds sub-investor portfolios quickly
            Dictionary<string, List<Investment>> byInvestor = investments
                .GroupBy(i => i.InvestorId, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(g => g.Key, g => g.ToList(), StringComparer.OrdinalIgnoreCase);

            decimal total = 0m;

            foreach (Investment fond in investments.Where(i => i.InvestmentType == InvestmentTypeEnum.Fonds))
            {
                // Consider only Percentage transactions for Fonds ownership
                decimal ownership = fond.Transactions
                    .Where(t => t.Type == TransactionTypeEnum.Percentage)
                    .Sum(t => t.Value);

                if (ownership == 0m)
                    continue;

                string fondsInvestorId = fond.FondsInvestor?.Trim() ?? string.Empty;
                if (string.IsNullOrEmpty(fondsInvestorId))
                    continue;

                if (!byInvestor.TryGetValue(fondsInvestorId, out List<Investment>? subInvestments) || subInvestments.Count == 0)
                    continue;

                // Delegate calculation to existing services to respect DRY and single-responsibility
                List<Investment> subStocks = subInvestments.Where(i => i.InvestmentType == InvestmentTypeEnum.Stock).ToList();
                List<Investment> subRealEstate = subInvestments.Where(i => i.InvestmentType == InvestmentTypeEnum.RealEstate).ToList();

                decimal subInvestmentsTotal = 0m;
                
                if (subStocks.Count > 0)
                    subInvestmentsTotal += _stockService.CalculateStockBalanceByInvestments(subStocks);

                if (subRealEstate.Count > 0)
                    subInvestmentsTotal += _realEstateService.CalculateRealEstateBalanceByInvestments(subRealEstate);

                total += ownership * subInvestmentsTotal;
            }

            return total;
        }
    }
}
