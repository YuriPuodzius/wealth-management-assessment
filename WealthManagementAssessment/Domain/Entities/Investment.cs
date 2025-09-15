using CsvHelper.Configuration.Attributes;
using WealthManagementAssessment.Domain.Enums;

namespace WealthManagementAssessment.Domain.Entities
{
    public class Investment
    {
        public string InvestorId { get; set; }

        public string InvestmentId { get; set; }

        public InvestmentTypeEnum InvestmentType { get; set; }

        public string ISIN { get; set; }

        public string City { get; set; }

        public string FondsInvestor { get; set; }

        public List<Transaction> Transactions { get; set; } = new List<Transaction>(); 
        
        [Ignore] // CsvHelper vai ignorar essa propriedade no binding..
        public Quote? LatestQuote { get; set; }
    }
}
