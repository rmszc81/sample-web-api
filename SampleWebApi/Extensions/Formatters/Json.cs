using Microsoft.Extensions.DependencyInjection;

using Newtonsoft.Json;

namespace SampleWebApi.Extensions.Formatters
{
    /// <summary>
    /// 
    /// </summary>
    public static class Json
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="setupAction"></param>
        /// <returns></returns>
        public static IMvcBuilder ConfigureJson(this IMvcBuilder setupAction)
        {
            return setupAction.AddJsonOptions(options => { options.SerializerSettings.Formatting = Formatting.Indented; });
        }
    }
}
