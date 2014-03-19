using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;

namespace Playground.Web
{
    public class UnityHttpControllerActivator : IHttpControllerActivator
    {
        private IUnityContainer _container;

        public UnityHttpControllerActivator(IUnityContainer container)
        {
            _container = container;
        }

        public IHttpController Create(HttpControllerContext controllerContext, Type controllerType)
        {
            return (IHttpController)_container.Resolve(controllerType);
        }

        public IHttpController Create(HttpRequestMessage request, HttpControllerDescriptor controllerDescriptor, Type controllerType)
        {
            return (IHttpController)_container.Resolve(controllerType);
        }
    }
}