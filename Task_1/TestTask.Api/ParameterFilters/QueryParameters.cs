using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace TestTask.Api.ParameterFilters
{
    public class QueryParameters
    {
        public string? SearchCode { get; set; }
        public string? SearchValue { get; set; }
        public string? SortOrder { get; set; }
        public string? SortColumn { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }

        public override string ToString()
        {
            return $"SearchCode={SearchCode},SearchValue={SearchValue},SortOrder={SortOrder}," +
                $"SortColumn={SortColumn},Page={Page},PageSize={PageSize}";
        }
    }
}
