using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Security;
using System.Net.Http;
using System.Security.Cryptography;

using LIMS.Util;
using LIMS.Entities;
using LIMS.MVCFoundation.Core;

namespace LIMS.MVCFoundation.Helpers
{
    public static class SecurityHelper
    {
        private static MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();

        public static void CreateTicket(HttpResponseBase response, UserEntity user)
        {
            FormsAuthenticationTicket ticket;
            string userData = CustomPrincipal.MakeTicketUserData(user.Id);

            ticket = new FormsAuthenticationTicket(
                1, 
                user.Name, 
                DateTime.Now, 
                DateTime.Now.AddMinutes(30), 
                false, 
                userData, 
                FormsAuthentication.FormsCookiePath);

            string authTicket = FormsAuthentication.Encrypt(ticket);

            //将加密后的票据保存为cookie  
            HttpCookie cookie = response.Cookies[FormsAuthentication.FormsCookieName];
            if (cookie == null)
            {
                cookie = new HttpCookie(FormsAuthentication.FormsCookieName, authTicket);
            }
            else
            {
                cookie.Value = authTicket;
            }

            cookie.Path = FormsAuthentication.FormsCookiePath;
            cookie.HttpOnly = false;
            cookie.Secure = FormsAuthentication.RequireSSL;
            cookie.Path = FormsAuthentication.FormsCookiePath;
            if (ticket.IsPersistent)
            {
                cookie.Expires = ticket.Expiration;
            }

            response.Cookies.Remove(FormsAuthentication.FormsCookieName);
            response.Cookies.Add(cookie);
        }

        public static bool ValidatePassword(string password, string hashValue)
        {
            var temp = HashPassword(password);

            return string.Compare(temp, hashValue) == 0;
        }

        public static string HashPassword(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return string.Empty;
            }

            var bytes = Encoding.UTF8.GetBytes(text + "_LIMS");
            var hashValue = md5.ComputeHash(bytes);

            return Convert.ToBase64String(hashValue);
        }
    }
}
