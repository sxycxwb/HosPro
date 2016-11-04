using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HosPro.Models;
using Newtonsoft.Json;

namespace HosPro.Code
{
    public class HandlerLoginAttribute : AuthorizeAttribute
    {
        public bool Ignore = true;
        public HandlerLoginAttribute(bool ignore = true)
        {
            Ignore = ignore;
        }
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (Ignore == false)
            {
                return;
            }
            var loginUser = ToObject<LoginUser>(WebHelper.GetCookie("loginUser521"));
            if (loginUser == null)
            {
                filterContext.HttpContext.Response.Write("<script>window.location.href = '/Home/Login';</script>");
                return;
            }
        }

        public  T ToObject<T>(string Json)
        {
            return Json == null ? default(T) : JsonConvert.DeserializeObject<T>(Json);
        }
    }
}