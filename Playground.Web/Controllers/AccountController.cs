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
using Playground.Model;

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

        private UserProfile GetUserProfile(string userName)
        {
            UserProfile retVal = null;
            using (UsersContext db = new UsersContext())
            {
                retVal = db.UserProfiles.FirstOrDefault(u => u.UserName.ToLower() == userName.ToLower());
            }
            return retVal;
        }

        private UserProfile GetUserProfile()
        {
            return GetUserProfile(User.Identity.Name);
        }

        [HttpPost]
        [AllowAnonymous]
        // TODO: uncoment this when antiforgery token mechanism is in place
        //[ValidateHttpAntiForgeryTokeAttribute]
        public HttpResponseMessage Login(LoginModel user)
        {
            if (ModelState.IsValid && WebSecurity.Login(user.Email, user.Password, user.RememberMe))
            {
                UserProfile userProfile = GetCurrentUser(user.Email);
                var response = Request.CreateResponse(HttpStatusCode.OK, new {user = userProfile});
                return response;
            }
            else
            {
                var response = Request.CreateResponse(HttpStatusCode.OK, "Invalid credentilas");
                return response;
            }
        }

        [HttpPost]
        [ActionName("logout")]
        // TODO: uncoment this when antiforgery token mechanism is in place
        //[ValidateHttpAntiForgeryTokeAttribute]
        public HttpResponseMessage LogOff()
        {
            WebSecurity.Logout();
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [HttpGet]
        [AllowAnonymous]
        [ActionName("currentuser")]
        public UserProfile GetCurrentUser()
        {
            return GetCurrentUser(User.Identity.Name);
        }

        private UserProfile GetCurrentUser(string userName)
        {
            UserProfile retVal = GetUserProfile(userName);
            if (retVal != null)
            {
                Playground.Model.User playgroundUser = Uow.Users.GetUserByExternalId(retVal.UserId);
                if (playgroundUser != null)
                {
                    retVal.UserName = String.Format("{0} {1}", playgroundUser.FirstName, playgroundUser.LastName);
                }
            }

            return retVal;
        }

    }
}
