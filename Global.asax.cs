using GHB_D1.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;

namespace GHB_D1
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            MvcHandler.DisableMvcResponseHeader = true;
        }

    protected void Application_Error()
        {
            Exception exception = Server.GetLastError();
            Server.ClearError();

            var httpException = exception as HttpException;
            var routeData = new RouteData();
            routeData.Values["controller"] = "Error";
            routeData.Values["action"] = "Index";

            if (httpException != null)
            {
                switch (httpException.GetHttpCode())
                {
                    //case 400:
                    //    routeData.Values["action"] = "URLError";
                    //    break;
                    case 404:
                        routeData.Values["action"] = "NotFound";
                        break;
                    case 500:
                        routeData.Values["action"] = "InternalServerError";
                        break;
                }
            }

            routeData.Values["error"] = exception;
            Response.TrySkipIisCustomErrors = true;
            IController errorController = new ErrorController();
            errorController.Execute(new RequestContext(new HttpContextWrapper(Context), routeData));
        }
    }
}
