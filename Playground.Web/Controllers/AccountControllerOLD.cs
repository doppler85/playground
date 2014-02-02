//using Playground.Common;
//using Playground.Data.Contracts;
//using Playground.Model;
//using Playground.Web.Filters;
//using Playground.Web.Models;
//using Playground.Web.Util;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net;
//using System.Net.Http;
//using System.Web.Http;
//using System.Web.Security;
//using WebMatrix.WebData;

//namespace Playground.Web.Controllers
//{
//    [Authorize]
//    [InitializeSimpleMembership]
//    public class AccountControllerOLD : ApiBaseController
//    {
//        public AccountControllerOLD(IPlaygroundUow uow)
//        {
//            this.Uow = uow;
//        }

//        //
//        // POST: api/account/login

//        /*
//        [HttpPost]
//        [AllowAnonymous]
//        [System.Web.Mvc.ValidateAntiForgeryToken]
//        public HttpResponseMessage Login(LoginModel model, string returnUrl)
//        {
//            if (ModelState.IsValid && WebSecurity.Login(model.UserName, model.Password, persistCookie: model.RememberMe))
//            {
//                var response = Request.CreateResponse(HttpStatusCode.OK, model);
//                return response;
//            }
//            else
//            {
//                var response = Request.CreateResponse(HttpStatusCode.OK, "Invalid credentilas");
//                return response;
//            }
//        }
//        */

//        private UserProfile GetUserProfile(string userName)
//        {
//            UserProfile retVal = null;
//            using (UsersContext db = new UsersContext())
//            {
//                retVal = db.UserProfiles.FirstOrDefault(u => u.UserName.ToLower() == userName.ToLower());
//            }
//            return retVal;
//        }

//        private UserProfile GetUserProfile()
//        {
//            return GetUserProfile(User.Identity.Name);
//        }

//        [HttpPost]
//        [AllowAnonymous]
//        // TODO: uncoment this when antiforgery token mechanism is in place
//        //[ValidateHttpAntiForgeryTokeAttribute]
//        public HttpResponseMessage Login(LoginModel user)
//        {
//            if (ModelState.IsValid && WebSecurity.Login(user.Email, user.Password, user.RememberMe))
//            {
//                UserProfile userProfile = GetCurrentUser(user.Email);
//                var response = Request.CreateResponse(HttpStatusCode.OK, new {user = userProfile});
//                return response;
//            }
//            else
//            {
//                var response = Request.CreateResponse(HttpStatusCode.OK, "Invalid credentilas");
//                return response;
//            }
//        }

//        [HttpPost]
//        [ActionName("logout")]
//        // TODO: uncoment this when antiforgery token mechanism is in place
//        //[ValidateHttpAntiForgeryTokeAttribute]
//        public HttpResponseMessage LogOff()
//        {
//            WebSecurity.Logout();
//            return Request.CreateResponse(HttpStatusCode.OK);
//        }

//        [HttpGet]
//        [AllowAnonymous]
//        [ActionName("currentuser")]
//        public UserProfile GetCurrentUser()
//        {
//            return GetCurrentUser(User.Identity.Name);
//        }

//        private UserProfile GetCurrentUser(string userName)
//        {
//            UserProfile retVal = GetUserProfile(userName);
//            if (retVal != null)
//            {
//                Playground.Model.User playgroundUser = Uow.Users
//                    .GetAll()
//                    .FirstOrDefault(u => u.ExternalUserID == retVal.UserId);
//                if (playgroundUser != null)
//                {
//                    retVal.UserName = String.Format("{0} {1}", playgroundUser.FirstName, playgroundUser.LastName);
//                }
//            }

//            return retVal;
//        }

//        //
//        // POST: /Account/Register

//        [HttpPost]
//        [AllowAnonymous]
//        // TODO: uncoment this when antiforgery token mechanism is in place
//        //[ValidateHttpAntiForgeryTokeAttribute]
//        public HttpResponseMessage Register(RegisterModel registerModel)
//        {
//            if (ModelState.IsValid)
//            {
//                // Attempt to register the user
//                try
//                {
//                    WebSecurity.CreateUserAndAccount(registerModel.UserName, registerModel.Password);
//                    AddUserToRole(registerModel.UserName, Constants.RoleNames.User);
//                    WebSecurity.Login(registerModel.UserName, registerModel.Password);
//                    UserProfile userProfile = GetCurrentUser(registerModel.UserName);

//                    User user = new User()
//                    {
//                        EmailAddress = registerModel.UserName,
//                        ExternalUserID = userProfile.UserId
//                    };

//                    Uow.Users.Add(user);
//                    Uow.Commit();

//                    var response = Request.CreateResponse(HttpStatusCode.OK, new { user = userProfile });
//                    return response;
//                }
//                catch (MembershipCreateUserException e)
//                {
//                    var response = Request.CreateResponse(HttpStatusCode.OK, "Invalid credentilas");
//                    return response;
//                }
//            }
//            else
//            {
//                var response = Request.CreateResponse(HttpStatusCode.InternalServerError);
//                return response;
//            }
//        }

//        private void AddUserToRole(string userName, string roleName)
//        {
//            var roles = (SimpleRoleProvider)Roles.Provider;
//            if (!roles.IsUserInRole(userName, roleName))
//            {
//                roles.AddUsersToRoles(new string[] { userName }, new string[] { roleName });
//            }
//        }
//    }
//}
