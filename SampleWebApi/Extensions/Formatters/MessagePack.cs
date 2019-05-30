using Microsoft.Extensions.DependencyInjection;

using WebApiContrib.Core.Formatter.MessagePack;

namespace SampleWebApi.Extensions.Formatters
{
    /// <summary>
    /// 
    /// </summary>
    public static class MessagePack
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="setupAction"></param>
        /// <returns></returns>
        public static IMvcBuilder ConfigureMessagePack(this IMvcBuilder setupAction)
        {
            return setupAction.AddMessagePackFormatters();
        }
    }
}
