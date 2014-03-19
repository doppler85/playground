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
using Playground.Business.Contracts;
using Playground.Business;

namespace Playground.Web
{
    public class IocConfig
    {
        public static void RegisterIoc(HttpConfiguration config)
        {
            string path = HttpRuntime.AppDomainAppPath;

            var unityContainer = new UnityContainer();
            
            unityContainer.RegisterType<IPlaygroundBusiness, PlaygroundBusiness>();

            unityContainer.RegisterType<IUserBusiness, UserBusiness>();

            unityContainer.RegisterType<IAutomaticConfirmationBusiness, AutomaticConfirmationBusiness>();

            unityContainer.RegisterType<ICompetitionTypeBusiness, CompetitionTypeBusiness>();

            unityContainer.RegisterType<IGameCategoryBusiness, GameCategoryBusiness>();

            unityContainer.RegisterType<IGameBusiness, GameBusiness>();

            unityContainer.RegisterType<ICompetitorBusiness, CompetitorBusiness>();

            unityContainer.RegisterType<IMatchBusiness, MatchBusiness>();

            unityContainer.RegisterType<IRepositoryProvider, RepositoryProvider>();

            unityContainer.RegisterType<IPlaygroundUow, PlaygroundUow>();

            unityContainer.RegisterInstance<RepositoryFactories>(new RepositoryFactories(), new ContainerControlledLifetimeManager());

            unityContainer.RegisterInstance<IHttpControllerActivator>(new UnityHttpControllerActivator(unityContainer));

            config.DependencyResolver = new IoCContainer(unityContainer);
        }
    }
}