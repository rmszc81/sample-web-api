using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace SampleWebApi.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Route("[controller]")]
    [Authorize]
    public class IdentityController : Controller
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Get()
        {
            return new JsonResult(from c in User.Claims select new { c.Type, c.Value });
        }
    }
}