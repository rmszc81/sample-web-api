using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Serilog;

namespace SampleWebApi.Controllers
{
    using Database;
    using SampleModel;
    using Helpers;
    

    /// <summary>
    /// 
    /// </summary>
    [Route("api/values")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly Context _context;
        private ILogger<ValuesController> _logger;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="diInterface"></param>
        /// <param name="logger"></param>
        public ValuesController(Context context, IInterface diInterface, ILogger<ValuesController> logger)
        {
            _context = context;
            _logger = logger;

            if (!_context.Values.Any())
            {
                _context.Values.Add(new ValueItem { Name = diInterface.Method() });
                _context.Values.Add(new ValueItem { Name = "Item1" });
                _context.Values.Add(new ValueItem { Name = "Item2" });
                _context.Values.Add(new ValueItem { Name = "Item3" });
                _context.SaveChanges();
            }

            _logger.LogDebug("Controller constructor was called.");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ValueItem>>> GetValueItems()
        {
            _logger.LogDebug("GetValueItems was called.");

            return await _context.Values.ToListAsync();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<ValueItem>> GetValueItem(long id)
        {
            _logger.LogDebug($"GetValueItem '{id}' was called.");

            var ValueItem = await _context.Values.FindAsync(id);

            if (ValueItem == null)
            {
                return NotFound();
            }

            return ValueItem;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<ValueItem>> PostValueItem([FromBody]ValueItem item)
        {
            _logger.LogDebug("PostValueItem was called.");

            _context.Values.Add(item);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetValueItem), new { id = item.Id }, item);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutValueItem(long id, ValueItem item)
        {
            _logger.LogDebug($"PutValueItem '{id}' was called.");

            if (id != item.Id)
            {
                return BadRequest();
            }

            _context.Entry(item).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteValueItem(long id)
        {
            _logger.LogDebug($"DeleteValueItem '{id}' was called.");

            var ValueItem = await _context.Values.FindAsync(id);

            if (ValueItem == null)
            {
                return NotFound();
            }

            _context.Values.Remove(ValueItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
