using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ISODocument.Controllers
{
    public class Check_Authorize_Admin : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (HttpContext.Current.Session["DC_UType"] == null)
            {
                filterContext.Result = new RedirectResult("~/Home/Index");
            }
            else if (HttpContext.Current.Session["DC_UType"].ToString() == "0")
            {
                filterContext.Result = new RedirectResult("~/Home/Index");
            }
        }
    }
}