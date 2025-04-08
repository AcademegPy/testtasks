using TestTask.Api.Data;

namespace TestTask.Api.Entities
{
    public record CodeValuesResponse(IEnumerable<CodeValue>? CodeValues, Exception? Exception);
}
