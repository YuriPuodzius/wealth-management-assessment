using WealthManagementAssessment.Domain.Enums;
namespace WealthManagementAssessment.Domain.Entities
{
    public class Transaction
    {
        public string InvestmentId { get; set; }
        public TransactionTypeEnum Type { get; set; }
        public DateTime Date { get; set; }
        public decimal Value { get; set; }   
    }

}
