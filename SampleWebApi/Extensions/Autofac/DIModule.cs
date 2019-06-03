using Autofac;

namespace SampleWebApi.Extensions.Autofac
{
    using Helpers;

    /// <summary>
    /// 
    /// </summary>
    public class DIModule : Module
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ContainerBuilder"></param>
        protected override void Load(ContainerBuilder ContainerBuilder)
        {
            ContainerBuilder.RegisterType<Class>().As<IInterface>().InstancePerLifetimeScope();
        }
    }
}
