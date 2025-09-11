using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GHB_D1.Controllers
{
    public class ErrorController : BaseController
    {
        // GET: Error
        public ActionResult Index(Exception error)
        {
            ViewBag.ErrorMessage = error.Message;            
            return View("Error");
        }       

        public ActionResult NotFound()
        {
            Response.StatusCode = 404;            
            return View("NotFound");
        }

        public ActionResult InternalServerError()
        {
            Response.StatusCode = 500;
            return View("InternalServerError");
        }
    }
}