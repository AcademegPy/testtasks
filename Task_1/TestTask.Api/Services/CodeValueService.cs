using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using TestTask.Api.Data;
using TestTask.Api.Entities;
using TestTask.Api.ParameterFilters;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace TestTask.Api.Services
{
    public class CodeValueService
    {
        private readonly ILogger<CodeValueService> _logger;
        private readonly ApplicationDbContext _context;

        public CodeValueService(ILogger<CodeValueService> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<PagedList<CodeValue>> GetCodeValuesAsync(QueryParameters query)
        {
            IQueryable<CodeValue> codeValues = _context.CodeValues;

            codeValues = Search(query, codeValues);

            codeValues = Sort(query, codeValues);

            var paged = await PagedList<CodeValue>.CreateAsync(codeValues, query.Page, query.PageSize);

            return paged;
        }

        private static IQueryable<CodeValue> Search(QueryParameters query, IQueryable<CodeValue> codeValues)
        {
            if (!string.IsNullOrWhiteSpace(query.SearchValue) | int.TryParse(query.SearchCode, out var code))
            {
                codeValues = codeValues.Where(x => x.Code == code || x.Value.Contains(query.SearchValue!));
            }

            return codeValues;
        }

        private static IQueryable<CodeValue> Sort(QueryParameters query, IQueryable<CodeValue> codeValues)
        {
            Expression<Func<CodeValue, object>> selector = query.SortColumn?.ToLower() switch
            {
                "code" => codeValue => codeValue.Code,
                "value" => codeValue => codeValue.Value,
                _ => codeValue => codeValue.Id
            };

            if (query.SortOrder?.ToLower() == "desc")
            {
                codeValues = codeValues.OrderByDescending(selector);
            }
            else
            {
                codeValues = codeValues.OrderBy(selector);
            }

            return codeValues;
        }

        public async Task<CodeValuesResponse> SaveCodeValuesAsync(IEnumerable<Dictionary<string, string>> codeValueJsons)
        {
            var extractResult = GetCodeValues(codeValueJsons);

            if (extractResult.Exception is not null)
                return new CodeValuesResponse(null, extractResult.Exception);

            try
            {
                await TruncateTableAndRestartSeqAsync();

                await AddCodeValuesAsync(extractResult.CodeValues!);
            }
            catch (Exception ex) 
            {
                _logger.LogError("Ошибка сохранения значений CodeValue {Exception}", ex.Message);
                return new CodeValuesResponse(null, new Exception("Ошибка сохранения значений CodeValue"));
            }


            return new CodeValuesResponse(await _context.CodeValues.ToListAsync(), null);
        }

        private async Task AddCodeValuesAsync(IEnumerable<CodeValuesDto> codeValues)
        {
            await _context.CodeValues.AddRangeAsync(codeValues.OrderBy(cvd => cvd.Code)
                .Select(cvd => new CodeValue { Code = cvd.Code, Value = cvd.Value }));

            await _context.SaveChangesAsync();
        }

        private async Task TruncateTableAndRestartSeqAsync()
        {
            var tableName = _context.CodeValues.EntityType.GetTableName();
            var idName = nameof(CodeValue.Id);

            var sql = $"""
                        TRUNCATE TABLE "{tableName}";
                        ALTER SEQUENCE "{tableName}_{idName}_seq" RESTART WITH 1;
                       """;

            await _context.Database.ExecuteSqlRawAsync(sql);
            _logger.LogDebug("Table {Table} is cleared", tableName);
        }

        private static ExtractResult GetCodeValues(IEnumerable<Dictionary<string, string>> codeValues)
        {
            try
            {
                var result = codeValues.SelectMany(dict => dict.Select(kvp => new CodeValuesDto
                {
                    Code = int.Parse(kvp.Key),
                    Value = kvp.Value
                })).ToList();

                return new ExtractResult(result, null);
            }
            catch (FormatException ex)
            {
                return new ExtractResult(null, ex);
            }
        }
    }

    public record ExtractResult(IEnumerable<CodeValuesDto>? CodeValues, Exception? Exception);
}
