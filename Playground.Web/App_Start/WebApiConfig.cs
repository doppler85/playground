using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using Microsoft.Owin.Security.OAuth;
using Newtonsoft.Json.Serialization;

namespace Playground.Web
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.SuppressDefaultHostAuthentication();

            config.Filters.Add(new HostAuthenticationFilter(OAuthDefaults.AuthenticationType));

            config.MapHttpAttributeRoutes();

            config.Formatters.Remove(config.Formatters.XmlFormatter);

            var json = config.Formatters.JsonFormatter;
            json.SerializerSettings.ContractResolver =
                new CamelCasePropertyNamesContractResolver();

            config.Formatters.JsonFormatter.SerializerSettings.ReferenceLoopHandling
                = Newtonsoft.Json.ReferenceLoopHandling.Ignore;

            //config.Formatters.JsonFormatter.SerializerSettings.ReferenceLoopHandling 
            //    = Newtonsoft.Json.ReferenceLoopHandling.Serialize; 

            //config.Formatters.JsonFormatter.SerializerSettings.PreserveReferencesHandling 
            //    = Newtonsoft.Json.PreserveReferencesHandling.Objects; 

            //todo add validation action
            config.Filters.Add(new ValidationActionFilter());

            config.Routes.MapHttpRoute(
                name: "ControllerOnly",
                routeTemplate: "api/{controller}"
            );

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional },
                constraints: new { id = @"\d+" }
            );

            config.Routes.MapHttpRoute(
                name: "ControllerAction",
                routeTemplate: "api/{controller}/{action}"
            );

            config.Routes.MapHttpRoute(
                name: "ActionApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
