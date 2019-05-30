using Microsoft.EntityFrameworkCore;

namespace SampleWebApi.Database
{
    using SampleModel;

    /// <summary>
    /// 
    /// </summary>
    public class Context : DbContext
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        public Context(DbContextOptions<Context> options) : base(options) { }

        /// <summary>
        /// 
        /// </summary>
        public DbSet<ValueItem> Values { get; set; }
    }
}
