using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Microsoft.Practices.Unity;
using Playground.Data.Helpers;
using Playground.Data.Contracts;
using Playground.Data;
using System.Web.Http.Dispatcher;

namespace Playground.Web.App_Start
{
    public class IocConfig
    {
        public static void RegisterIoc(HttpConfiguration config)
        {
            var unityContainer = new UnityContainer();

            unityContainer.RegisterType<IRepositoryProvider, RepositoryProvider>();

            unityContainer.RegisterType<IPlaygroundUow, PlaygroundUow>();

            // RepositoryFactories repositoryFactories = new RepositoryFactories();
            
            // unityContainer.RegisterInstance<RepositoryFactories>(repositoryFactories);

            unityContainer.RegisterInstance<RepositoryFactories>(new RepositoryFactories(), new ContainerControlledLifetimeManager());

            unityContainer.RegisterInstance<IHttpControllerActivator>(new UnityHttpControllerActivator(unityContainer));

            // unityContainer.RegisterType<RepositoryFactories, RepositoryFactories>();

            // unityContainer.RegisterType<IPla

            config.DependencyResolver = new IoCContainer(unityContainer);
        }
    }
}