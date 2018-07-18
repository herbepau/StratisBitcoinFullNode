using Microsoft.AspNetCore.Mvc;
using Stratis.Bitcoin.Statistics.Interfaces;

namespace Stratis.Bitcoin.Controllers
{
    [Route("api/[controller]")]
    public class StatisticsController : Controller
    {
        private readonly IStatisticsService statisticsService;        

        public StatisticsController(IStatisticsService statisticsService) => this.statisticsService = statisticsService;

        [HttpGet]
        [Route("groups")]
        public JsonResult GetGroupsAsync()
        {
            var groups = this.statisticsService.Groups;

            return new JsonResult(groups);
        }
    }
}
