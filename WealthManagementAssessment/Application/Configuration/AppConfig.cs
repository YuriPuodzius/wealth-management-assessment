using WealthManagementAssessment.Domain.Enums;

namespace WealthManagementAssessment.Application.Configuration
{
    public class AppConfig
    {
        public DataBindingsConfig DataBindings { get; set; } = new();
        public string ProjectDirectory => Directory.GetParent(AppContext.BaseDirectory)!.Parent!.Parent!.Parent!.FullName;
    }

    public class DataBindingsConfig
    {
        public InvestmentDataSourceTypeEnum InvestmentDataSourceType { get; set; } = InvestmentDataSourceTypeEnum.Csv;
        public CsvConfig CsvConfig { get; set; } = new();
        public JsonConfig JsonConfig { get; set; } = new();
        public ApiConfig ApiConfig { get; set; } = new();
    }

    public class CsvConfig
    {
        public string InvestmentsPath { get; set; } = string.Empty;
        public string TransactionsPath { get; set; } = string.Empty;
        public string QuotesPath { get; set; } = string.Empty;
    }

    public class JsonConfig
    {
        public string InvestmentsPath { get; set; } = string.Empty;
        public string TransactionsPath { get; set; } = string.Empty;
        public string QuotesPath { get; set; } = string.Empty;
    }

    public class ApiConfig
    {
        public string BaseUrl { get; set; } = string.Empty;
        public string InvestmentsEndpoint { get; set; } = string.Empty;
        public string TransactionsEndpoint { get; set; } = string.Empty;
        public string QuotesEndpoint { get; set; } = string.Empty;
        public string ApiKey { get; set; } = string.Empty;
    }
}
