using carenirvana.bre.engine.Interfaces;
using carenirvana.bre.model.Impl;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace carenirvana.bre.ruleapi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RuleController(IRuleEngine ruleEngine, ILogger<RuleController> logger) : ControllerBase
    {
        private readonly ILogger<RuleController> _logger = logger;
        private readonly IRuleEngine _ruleEngine = ruleEngine;

        [HttpPost(Name = "RunRule")]
        public RuleOutput RunRule(string ruleName, [FromBody] object[] parameters)
        {
            return _ruleEngine.ExecuteRule(ruleName, parameters);
        }
    }
}
