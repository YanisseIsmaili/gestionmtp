using System;
using Yprotect.Model;

namespace Yprotect.Services
{
    public static class UserSession
    {
        public static BDUtilisateur? CurrentUser { get; private set; }
        
        public static bool IsAdmin => CurrentUser?.Role == "Admin";
        
        public static void SetCurrentUser(BDUtilisateur user)
        {
            CurrentUser = user;
        }
        
        public static void Logout()
        {
            CurrentUser = null;
        }
    }
}
