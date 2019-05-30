using Microsoft.Extensions.DependencyInjection;

using WebApiContrib.Core.Formatter.Protobuf;

namespace SampleWebApi.Extensions.Formatters
{
    /// <summary>
    /// 
    /// </summary>
    public static class Protobuf
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="setupAction"></param>
        /// <returns></returns>
        public static IMvcBuilder ConfigureProtobuf(this IMvcBuilder setupAction)
        {
            return setupAction.AddProtobufFormatters();
        }
    }
}
