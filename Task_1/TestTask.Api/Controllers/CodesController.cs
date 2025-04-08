using Microsoft.AspNetCore.Mvc;
using TestTask.Api.Data;
using TestTask.Api.ParameterFilters;
using TestTask.Api.Services;

namespace TestTask.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CodesController : ControllerBase
    {
        private readonly CodeValueService _codeValueService;
        private readonly ILogger<CodesController> _logger;

        public CodesController(ILogger<CodesController> logger, CodeValueService codeValueService)
        {
            _logger = logger;
            _codeValueService = codeValueService;
        }

        [HttpGet]
        public async Task<IEnumerable<CodeValue>> Get([FromQuery]QueryParameters query)
        {
            _logger.LogInformation("Getting data with parameters {Query}", query.ToString());
            var paged = await _codeValueService.GetCodeValuesAsync(query);
            _logger.LogInformation("Received {Total} records", paged.TotalCount);
            return paged.Items;
        }

        [HttpPost]
        public async Task<ActionResult<IEnumerable<CodeValue>>> Post(IEnumerable<Dictionary<string, string>> codeValues)
        {
            if (codeValues == null || !codeValues.Any())
                return BadRequest("Invalid data");

            _logger.LogInformation("Saving data");
            var saveResult = await _codeValueService.SaveCodeValuesAsync(codeValues);

            if (saveResult.Exception is not null)
                return BadRequest(saveResult.Exception.Message);

            if (saveResult.CodeValues is not null)
            {
                _logger.LogInformation("Saved {Total} records", saveResult.CodeValues!.Count());
            }

            return CreatedAtAction(nameof(Get), saveResult.CodeValues);
        }
    }
}
