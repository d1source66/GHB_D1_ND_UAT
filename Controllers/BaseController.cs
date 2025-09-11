using GHB_D1.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GHB_D1.Controllers
{
    public class BaseController : Controller
    {
        private UserSessionManager sessionManager = new UserSessionManager();
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            
            var existingSessionId = sessionManager.GetSessionId(Convert.ToInt64(Session["UserId"]));
            if (!string.IsNullOrEmpty(existingSessionId))
            {
                // เคลียร์เซสชันเก่า

                var oldSessionId = System.Web.HttpContext.Current.Session.SessionID;
                if (oldSessionId != existingSessionId)
                {
                    Session.Clear();
                    Session.Abandon();
                    Session.RemoveAll();
                }

            }
            // ตรวจสอบว่าผู้ใช้งานได้ล็อกอินอยู่หรือไม่
            if (Session["UserId"] == null)
            {
                // ถ้าไม่ได้ล็อกอิน ให้เปลี่ยนเส้นทางไปยังหน้าล็อกอิน
                filterContext.Result = new RedirectResult("~/Account/Login");
            }
        }
    }
}