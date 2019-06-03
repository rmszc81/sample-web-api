using Autofac;
using Autofac.Extensions.DependencyInjection;

using Microsoft.Extensions.DependencyInjection;

namespace SampleWebApi.Extensions.Autofac
{
    /// <summary>
    /// 
    /// </summary>
    public class Container
    {
        /// <summary>
        /// 
        /// </summary>
        public IContainer Kernel { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        public Container(IServiceCollection services)
        {
            var containerBuilder = new ContainerBuilder();

            containerBuilder.Populate(services);

            containerBuilder.RegisterModule(new DIModule());

            Kernel = containerBuilder.Build();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TObject"></typeparam>
        /// <returns></returns>
        public TObject Resolve<TObject>()
        {
            return Kernel.Resolve<TObject>();
        }
    }
}
