using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Playground.Common
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
            public static string AdminUserName = "vojaadmin@playground.com";
            public static string AdminPass = "pass*123";
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

            public static string GameCategoryPictureRoot = "/Images/Game/";
            public static string GameCategoryPicturePrefix = "game_category_pic";
            public static System.Drawing.Imaging.ImageFormat GameCategoryImageFormat = System.Drawing.Imaging.ImageFormat.Jpeg;
            public static string GameCategoryPictureExtension = "jpg";
            public static int GameCategoryImageMaxSize = 200;

            public static string PlayerPictureRoot = "/Images/Player/";
            public static string PlayerPicturePrefix = "player_pic";
            public static System.Drawing.Imaging.ImageFormat PlayerImageFormat = System.Drawing.Imaging.ImageFormat.Jpeg;
            public static string PlayerPictureExtension = "jpg";
            public static int PlayerImageMaxSize = 200;

            public static string TeamPictureRoot = "/Images/Team/";
            public static string TeamPicturePrefix = "team_pic";
            public static System.Drawing.Imaging.ImageFormat TeamImageFormat = System.Drawing.Imaging.ImageFormat.Jpeg;
            public static string TeamPictureExtension = "jpg";
            public static int TeamImageMaxSize = 200;

            public static string DefaultProfileMale = "/Images/Default/default_profile_male.jpg";
            public static string DefaultProfileFemale = "/Images/Default/default_profile_female.jpg";
            public static string DefaultGameImage = "";
                
        }
    }
}