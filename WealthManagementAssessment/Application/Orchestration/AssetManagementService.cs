using System.Diagnostics;
using WealthManagementAssessment.Application.Models;
using WealthManagementAssessment.Application.Orchestration.Interfaces;
using WealthManagementAssessment.Domain.Contracts.Services;
using WealthManagementAssessment.Domain.Entities;
using WealthManagementAssessment.Domain.Enums;

namespace WealthManagementAssessment.Application.Orchestration
{
    public class AssetManagementService : IAssetManagementService
    {
        private readonly IPortfolioService _portfolioService;
        private readonly IRealEstateService _realEstateService;
        private readonly IStockService _stockService;
        private readonly IFondsService _fondsService;

        // Simple in-memory cache: key = FondsInvestorId + valuationDate (yyyyMMdd)
        private readonly Dictionary<string, List<Investment>> _fondsPortfolioCache = new(StringComparer.OrdinalIgnoreCase);
        
        public AssetManagementService(
            IPortfolioService portfolioService,
            IRealEstateService realEstateService,
            IStockService stockService,
            IFondsService fondsService)
        {
            _portfolioService = portfolioService;
            _realEstateService = realEstateService;
            _stockService = stockService;
            _fondsService = fondsService;
        }

        public bool InvestorExists(string investorId)
        {
            if (string.IsNullOrWhiteSpace(investorId))
                return false;

            Stopwatch swInv = Stopwatch.StartNew();
            IReadOnlyList<Investment> investments = _portfolioService.GetInvestmentsByInvestor(investorId);
            swInv.Stop();
            Console.WriteLine($"⏱️ PortfolioService.GetInvestmentsByInvestor executed in {swInv.Elapsed.TotalSeconds:F2} seconds ({swInv.Elapsed.TotalMinutes:F2} minutes).");
            return investments.Count > 0;
        }

        public bool HasTransactionsForValuationDate(string investorId, DateTime valuationDate)
        {
            List<Investment> investorInvestments = _portfolioService.GetInvestmentsByInvestor(investorId).ToList();
            if (investorInvestments.Count == 0)
                return false;

            // Only hydrate the principal investor to keep this check fast
            Stopwatch swTx = Stopwatch.StartNew();
            _portfolioService.LoadTransactionsByInvestments(investorInvestments, valuationDate);
            swTx.Stop();
            Console.WriteLine($"⏱️ PortfolioService.LoadTransactionsByInvestments executed in {swTx.Elapsed.TotalSeconds:F2} seconds ({swTx.Elapsed.TotalMinutes:F2} minutes).");

            return investorInvestments.Any(inv => inv.Transactions.Count > 0);
        }
        
        public InvestorBalanceResult GetTotalBalanceByInvestor(string investorId, DateTime valuationDate)
        {
            Stopwatch swInv = Stopwatch.StartNew();
            List<Investment> investorInvestments = _portfolioService.GetInvestmentsByInvestor(investorId).ToList();
            swInv.Stop();
            Console.WriteLine($"⏱️ PortfolioService.GetInvestmentsByInvestor executed in {swInv.Elapsed.TotalSeconds:F2} seconds ({swInv.Elapsed.TotalMinutes:F2} minutes).");
            
            Stopwatch swTx = Stopwatch.StartNew();
            _portfolioService.LoadTransactionsByInvestments(investorInvestments, valuationDate);
            swTx.Stop();
            Console.WriteLine($"⏱️ PortfolioService.LoadTransactionsByInvestments executed in {swTx.Elapsed.TotalSeconds:F2} seconds ({swTx.Elapsed.TotalMinutes:F2} minutes).");
            
            List<Investment> stocks = investorInvestments.Where(i => i.InvestmentType == InvestmentTypeEnum.Stock).ToList();

            if (stocks.Count > 0)
            {
                //medidor aqui inicio
                _portfolioService.LoadQuotesByStockAsset(stocks, valuationDate);
                //fim
            }
            
            Stopwatch swRe = Stopwatch.StartNew();
            decimal realStateBalanceTotal = _realEstateService.CalculateRealEstateBalanceByInvestments(investorInvestments);
            swRe.Stop();
            Console.WriteLine($"⏱️ RealEstateService.CalculateRealEstateBalanceByInvestments executed in {swRe.Elapsed.TotalSeconds:F2} seconds ({swRe.Elapsed.TotalMinutes:F2} minutes).");

            Stopwatch swSt = Stopwatch.StartNew();
            decimal stockBalanceTotal = _stockService.CalculateStockBalanceByInvestments(investorInvestments);
            swSt.Stop();
            Console.WriteLine($"⏱️ StockService.CalculateStockBalanceByInvestments executed in {swSt.Elapsed.TotalSeconds:F2} seconds ({swSt.Elapsed.TotalMinutes:F2} minutes).");
            
            Stopwatch swHydrateFonds = Stopwatch.StartNew();
            HydrateFondsInvestorsPortfolios(investorInvestments, investorId, valuationDate);
            swHydrateFonds.Stop();
            Console.WriteLine($"⏱️ HydrateFondsInvestorsPortfolios executed in {swHydrateFonds.Elapsed.TotalSeconds:F2} seconds ({swHydrateFonds.Elapsed.TotalMinutes:F2} minutes).");
            
            Stopwatch swFondsCalc = Stopwatch.StartNew();
            decimal fondBalanceTotal = _fondsService.CalculateFondBalanceByInvestments(investorInvestments);
            swFondsCalc.Stop();
            Console.WriteLine($"⏱️ FondsService.CalculateFondBalanceByInvestments executed in {swFondsCalc.Elapsed.TotalSeconds:F2} seconds ({swFondsCalc.Elapsed.TotalMinutes:F2} minutes).");

            // Console.WriteLine($"Your Fond wallet is : {balance} Euros.");
            // Console.WriteLine($"Your total wallet is : {realEstateAsset + stockAsset + fondAsset} Euros.");

            InvestorBalanceResult result = new InvestorBalanceResult
            {
                RealEstateBalance = realStateBalanceTotal,
                StockBalance = stockBalanceTotal,
                FondsBalance = fondBalanceTotal
            };
            return result;
        }

        public InvestorBalanceResult GetTotalBalanceByInvestorAndInvestmentType(string investorId, DateTime valuationDate, InvestmentTypeEnum investmentType)
        {
            List<Investment> investorInvestments = _portfolioService.GetInvestmentsByInvestor(investorId).ToList();
            _portfolioService.LoadTransactionsByInvestments(investorInvestments, valuationDate, investmentType);
            
            switch (investmentType)
            {
                case InvestmentTypeEnum.RealEstate:
                {
                    decimal re = _realEstateService.CalculateRealEstateBalanceByInvestments(investorInvestments);
                    return new InvestorBalanceResult { RealEstateBalance = re };
                }
                case InvestmentTypeEnum.Stock:
                {
                    List<Investment> stocks = investorInvestments.Where(i => i.InvestmentType == InvestmentTypeEnum.Stock).ToList();
                    if (stocks.Count > 0)
                        _portfolioService.LoadQuotesByStockAsset(stocks, valuationDate);
                    decimal st = _stockService.CalculateStockBalanceByInvestments(investorInvestments);
                    return new InvestorBalanceResult { StockBalance = st };
                }
                case InvestmentTypeEnum.Fonds:
                {
                    HydrateFondsInvestorsPortfolios(investorInvestments, investorId, valuationDate);
                    decimal fd = _fondsService.CalculateFondBalanceByInvestments(investorInvestments);
                    return new InvestorBalanceResult { FondsBalance = fd };
                }
                default:
                    throw new NotImplementedException($"Calculation for investment type '{investmentType}' is not implemented.");
            }
        }

        public InvestorProfileEnum GetProfileByInvestor(string ownerId, DateTime valuationDate)
        {
            throw new NotImplementedException();
        }
        
        private void HydrateFondsInvestorsPortfolios(List<Investment> investorInvestments, string investorId, DateTime valuationDate)
        {
            // --- Load and hydrate portfolios for all Fonds investors referenced by the principal investor ---
            List<Investment> fonds = investorInvestments.Where(i => i.InvestmentType == InvestmentTypeEnum.Fonds).ToList();

            if (fonds.Count == 0)
                return;

            // Medição (2): custo para extrair/normalizar/deduplicar os IDs de FondsInvestor
            Stopwatch swIds = Stopwatch.StartNew();
            List<string> fondsInvestorIds = fonds
                .Select(f => f.FondsInvestor!.Trim())
                .Where(id => !string.IsNullOrWhiteSpace(id))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                // avoid re-loading the principal investor in case of malformed data
                .Where(id => !string.Equals(id, investorId, StringComparison.OrdinalIgnoreCase))
                .ToList();
            swIds.Stop();
            Console.WriteLine($"   ↳ FondsInvestorIds construídos em {swIds.Elapsed.TotalSeconds:F2} segundos ({swIds.Elapsed.TotalMinutes:F2} minutos). Quantidade: {fondsInvestorIds.Count}.");

            if (fondsInvestorIds.Count == 0)
                return;

            List<Investment> aggregatedFondsInvestments = new List<Investment>();

            // Contadores de cache para entender ganhos entre o 1º run e os demais (sem prints por iteração)
            int cacheHits = 0, cacheMisses = 0;
            Stopwatch swLoop = Stopwatch.StartNew();
            foreach (string fondsInvestorId in fondsInvestorIds)
            {
                string cacheKey = FondsCacheKey(fondsInvestorId, valuationDate);

                if (!_fondsPortfolioCache.TryGetValue(cacheKey, out List<Investment>? subInvestments))
                {
                    cacheMisses++;
                    // Load investments of the Fonds' of sub-investor
                    subInvestments = _portfolioService.GetInvestmentsByInvestor(fondsInvestorId).ToList();

                    if (subInvestments.Count > 0)
                    {
                        // Hydrate with transactions up to valuationDate
                        _portfolioService.LoadTransactionsByInvestments(subInvestments, valuationDate);

                        // Load quotes for any stocks within the fonds' portfolio
                        List<Investment> subStocks = subInvestments.Where(i => i.InvestmentType == InvestmentTypeEnum.Stock).ToList();

                        if (subStocks.Count > 0)
                            _portfolioService.LoadQuotesByStockAsset(subStocks, valuationDate);
                    }

                    // Memoize even empty result to avoid repeated I/O for missing portfolios
                    _fondsPortfolioCache[cacheKey] = subInvestments;
                }
                else
                {
                    cacheHits++;
                }

                if (subInvestments.Count > 0)
                    aggregatedFondsInvestments.AddRange(subInvestments);
            }
            swLoop.Stop();
            Console.WriteLine($"   ↳ Hidratação de sub-carteiras concluída em {swLoop.Elapsed.TotalSeconds:F2} segundos ({swLoop.Elapsed.TotalMinutes:F2} minutos). Processados: {fondsInvestorIds.Count}, cacheHits={cacheHits}, cacheMisses={cacheMisses}.");

            if (aggregatedFondsInvestments.Count > 0)
            {
                // Add the Fonds investors' portfolios to the main list so that FondsService
                // can correctly find and calculate them by their InvestorId
                investorInvestments.AddRange(aggregatedFondsInvestments);
            }
        }
        
        private static string FondsCacheKey(string investorId, DateTime valuationDate) => $"{investorId.Trim().ToUpperInvariant()}|{valuationDate:yyyyMMdd}";
    }
}