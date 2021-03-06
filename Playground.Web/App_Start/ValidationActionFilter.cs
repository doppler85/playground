﻿using log4net;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Playground.Web
{
    public class ValidationActionFilter : ActionFilterAttribute 
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public override void OnActionExecuting(HttpActionContext context) 
        { 
            var modelState = context.ModelState; 
            if (!modelState.IsValid) 
            { 
                var errors = new JObject(); 
                foreach (var key in modelState.Keys) 
                { 
                    var state = modelState[key]; 
                    if (state.Errors.Any()) 
                    { 
                        errors[key] = state.Errors.First().ErrorMessage;
                        log.ErrorFormat("Error validating model. key: {0}, message: {1}", key, state.Errors.First().ErrorMessage);
                        log.Error("Exception: ", state.Errors.First().Exception);
                    } 
                } 
 
                context.Response = context.Request.CreateResponse<JObject>(HttpStatusCode.BadRequest, errors); 
            } 
        } 
    }
}