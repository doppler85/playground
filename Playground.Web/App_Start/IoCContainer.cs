using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Dependencies;

namespace Playground.Web
{
    public class IoCContainer : ScopeContainer, IDependencyResolver
    {
        public IoCContainer(IUnityContainer container)
            : base(container)
        {
        }
 
        public IDependencyScope BeginScope()
        {
            var child = container.CreateChildContainer();
            return new ScopeContainer(child);
        }
    }
}
