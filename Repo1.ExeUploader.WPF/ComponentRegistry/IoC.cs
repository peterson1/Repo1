using Autofac;
using Autofac.Builder;
using Repo1.Core.ns12.Clients;
using Repo1.Core.ns12.Models;
using Repo1.ExeUploader.WPF.Clients;
using Repo1.ExeUploader.WPF.Configuration;

namespace Repo1.ExeUploader.WPF.ComponentRegistry
{
    internal static class IoC
    {
        internal static IContainer GetContainer(UploaderCfg cfg)
        {
            var b = new ContainerBuilder();

            b.RegisterInstance(cfg).As<UploaderCfg, RestServerCredentials>();
            b.Solo<MainWindowVM>();
            b.Multi<IRestClient, UploaderClient1>();

            return b.Build();
        }

        private static IRegistrationBuilder<T, ConcreteReflectionActivatorData, SingleRegistrationStyle> Solo<T>(this ContainerBuilder buildr) 
            => buildr.RegisterType<T>().AsSelf().SingleInstance();

        private static IRegistrationBuilder<TConcrete, ConcreteReflectionActivatorData, SingleRegistrationStyle> Solo<TInterface, TConcrete>(this ContainerBuilder buildr) where TConcrete : TInterface
            => buildr.RegisterType<TConcrete>().As<TInterface>().SingleInstance();

        private static IRegistrationBuilder<T, ConcreteReflectionActivatorData, SingleRegistrationStyle> Multi<T>(this ContainerBuilder buildr)
            => buildr.RegisterType<T>().AsSelf();

        private static IRegistrationBuilder<TConcrete, ConcreteReflectionActivatorData, SingleRegistrationStyle> Multi<TInterface, TConcrete>(this ContainerBuilder buildr) where TConcrete : TInterface
            => buildr.RegisterType<TConcrete>().As<TInterface>();
    }
}
