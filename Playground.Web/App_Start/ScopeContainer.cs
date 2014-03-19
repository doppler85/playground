using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Dependencies;

namespace Playground.Web
{
    public class ScopeContainer : IDependencyScope
    {
        protected IUnityContainer container;

        public ScopeContainer(IUnityContainer container)
        {
            if (container == null)
            {
                throw new ArgumentNullException("container");
            }
            this.container = container;
        }

        public object GetService(Type serviceType)
        {
            if (container.IsRegistered(serviceType))
            {
                return container.Resolve(serviceType);
            }
            else
            {
                return null;
            }
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            if (container.IsRegistered(serviceType))
            {
                return container.ResolveAll(serviceType);
            }
            else
            {
                return new List<object>();
            }
        }

        public void Dispose()
        {
            container.Dispose();
        }
    }
}