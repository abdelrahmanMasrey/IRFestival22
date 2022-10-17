
using System.Net;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using IRFestival.Api.Options;
using Microsoft.FeatureManagement;

namespace IRFestival.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SettingsController : ControllerBase
    {
        private readonly AppSettingsOptions _options;
        private readonly IFeatureManagerSnapshot _featureManager;
        public SettingsController(IOptions<AppSettingsOptions> options, IFeatureManagerSnapshot featureManager)
        {
            _options = options.Value;
            _featureManager = featureManager;
        }

        [HttpGet("Features")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<ActionResult> Features()
        {
            string message = await _featureManager.IsEnabledAsync("BuyTickets")
                ? "this ticket sale has started go go go"
                : "You cannot buy finiiiii";
                return Ok(message);
        }

        [HttpGet]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(AppSettingsOptions))]
        public IActionResult Get()
        {
            return Ok(_options);
        }
    }
}
