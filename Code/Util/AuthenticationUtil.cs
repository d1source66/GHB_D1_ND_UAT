using GHB_D1.Code.BAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GHB_D1.Code.Util;
using GHB_D1.Models;
using GHB_D1.Services;

namespace GHB_D1.Code.Util
{
    public static class AuthenticationUtil
    {
        private static Dictionary<string, int> loginAttempts = new Dictionary<string, int>();
        private static Dictionary<string, DateTime> lockedAccounts = new Dictionary<string, DateTime>();
        
        
        
        

        // Validate user credentials
        public static bool ValidateUser(string username, string password)
        {
            // Your authentication logic here
            return true; // or false based on authentication result
        }

        // Increment login attempts and return current attempts
        public static int IncrementLoginAttempts(string username)
        {
            if (loginAttempts.ContainsKey(username))
            {
                loginAttempts[username]++;
            }
            else
            {
                loginAttempts.Add(username, 1);
            }

            return loginAttempts[username];
        }

        // Reset login attempts
        public static void ResetLoginAttempts(string username)
        {
            if (loginAttempts.ContainsKey(username))
            {
                loginAttempts[username] = 0;
            }
        }

        // Lock account
        public static void LockAccount(string username)
        {
            lockedAccounts[username] = DateTime.Now;
        }

        // Check if account is locked
        public static bool IsAccountLocked(string username)
        {
            if (lockedAccounts.ContainsKey(username))
            {
                DateTime lockTime = lockedAccounts[username];
                int numberOfMinutes = AdministratorBAL.GetUSER_ATTRIB(0,"Locked_Time_Limit");
                if ((DateTime.Now - lockTime).TotalMinutes > numberOfMinutes)
                {
                    // Unlock account if locked for more than 15 minutes
                    UnlockAccount(username);
                    return false;
                }
                return true;
            }
            return false;
        }
        public static bool IsAccountLocked2(string username, string password)
        {
            
            if (lockedAccounts.ContainsKey(username))
            {
                bool resultLock = false;
                resultLock = GetUserLock(username, password);
                if (!resultLock)
                {
                    UnlockAccount(username);
                    return false;
                }
                return true;
            }

            return false;
        }
        // Unlock account
        public static void UnlockAccount(string username)
        {
            if (lockedAccounts.ContainsKey(username))
            {
                lockedAccounts.Remove(username);
            }
        }
        
        private static bool GetUserLock(string username,string password)
        {
            bool result = false;
            AccountService obj = new AccountService();
            result = obj.GetUserLock(username, password);
            return result;
        }
        public static void IsLockAccount(string username, string password)
        {
            AccountService obj = new AccountService();
            obj.EditUserLock(username,password);
        }

    }
}