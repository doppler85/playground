using Playground.Data.Contracts;
using Playground.Model;
using Playground.Web.Filters;
using Playground.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebMatrix.WebData;

namespace Playground.Web.Controllers
{
    [Authorize]
    [InitializeSimpleMembership]
    public class AccountController : ApiBaseController
    {
        public AccountController(IPlaygroundUow uow)
        {
            this.Uow = uow;
        }

        //
        // POST: api/account/login

        /*
        [HttpPost]
        [AllowAnonymous]
        [System.Web.Mvc.ValidateAntiForgeryToken]
        public HttpResponseMessage Login(LoginModel model, string returnUrl)
        {
            if (ModelState.IsValid && WebSecurity.Login(model.UserName, model.Password, persistCookie: model.RememberMe))
            {
                var response = Request.CreateResponse(HttpStatusCode.OK, model);
                return response;
            }
            else
            {
                var response = Request.CreateResponse(HttpStatusCode.OK, "Invalid credentilas");
                return response;
            }
        }
        */

        [HttpPost]
        [AllowAnonymous]
        // TODO: uncoment this when antiforgery token mechanism is in place
        //[ValidateHttpAntiForgeryTokeAttribute]
        public HttpResponseMessage Login(LoginModel user)
        {

            if (ModelState.IsValid && WebSecurity.Login(user.Email, user.Password, true))
            {
                var response = Request.CreateResponse(HttpStatusCode.OK, "Sucess");
                return response;
            }
            else
            {
                var response = Request.CreateResponse(HttpStatusCode.OK, "Invalid credentilas");
                return response;
            }
        }
    }
}
