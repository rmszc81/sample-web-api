using Microsoft.Extensions.DependencyInjection;

namespace SampleWebApi.Extensions.Formatters
{
    /// <summary>
    /// 
    /// </summary>
    public static class Xml
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="setupAction"></param>
        /// <returns></returns>
        public static IMvcBuilder ConfigureXml(this IMvcBuilder setupAction)
        {
            return setupAction.AddXmlSerializerFormatters();
        }
    }
}
