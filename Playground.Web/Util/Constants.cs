using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Playground.Web.Util
{
    public static class Constants
    {
        public static class RoleNames
        {
            public static string Admin = "Admin";
            public static string User = "User";
        }

        public static class AdminUser
        {
            public static string AdminUserName = "admin@playground.com";
            public static string AdminPass = "admin123!";
        }

        public static class Images
        {
            public static string ProfilePictureRoot = "/Images/Profile/";
            public static string ProfilePicturePrefix = "profile_pic";
            public static System.Drawing.Imaging.ImageFormat ProfileImageFormat = System.Drawing.Imaging.ImageFormat.Jpeg;
            public static string ProfilePictureExtension = "jpg";
            public static int ProfileImageMaxSize = 400;

            public static string GamePictureRoot = "/Images/Game/";
            public static string GamePicturePrefix = "game_pic";
            public static System.Drawing.Imaging.ImageFormat GameImageFormat = System.Drawing.Imaging.ImageFormat.Jpeg;
            public static string GamePictureExtension = "jpg";
            public static int GameImageMaxSize = 200;

            public static string DefaultMalaProfileImage = "";
            public static string DefaultFemaleProfileImage = "";
            public static string DefaultGameImage = "";
                
        }
    }
}