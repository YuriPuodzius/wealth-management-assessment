namespace WealthManagementAssessment.Application.Models;

public class InvestorBalanceResult
{
    public decimal RealEstateBalance { get; set; }
    public decimal StockBalance { get; set; }
    public decimal FondsBalance { get; set; }
    public decimal TotalBalance => RealEstateBalance + StockBalance + FondsBalance;
}