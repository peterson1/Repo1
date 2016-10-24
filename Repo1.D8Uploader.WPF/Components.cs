using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Autofac.Builder;
using Repo1.Core.ns11.Configuration;
using Repo1.D8Uploader.Lib45.Configuration;
using Repo1.D8Uploader.Lib45.RestClients;
using Repo1.D8Uploader.WPF.RestClients;
using Repo1.WPF45.SDK.Clients;

namespace Repo1.D8Uploader.WPF
{
    internal static class Components
    {
        internal static IContainer GetContainer(UploaderCfg cfg)
        {
            var b = new ContainerBuilder();

            b.RegisterInstance(cfg).As<UploaderCfg, DownloaderCfg, RestServerCredentials>();
            b.Solo<MainWindowVM>();
            b.Solo<UploaderClient2>();
            b.Solo<DeleterClientBase, DeleterClient2>();
            b.Multi<DownloaderClient2>();

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
