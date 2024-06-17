using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Website_clothe.Areas.Admin.Controllers
{
    public class BaseController : Controller
    {
        // GET: Admin/Base
        
        public string RenderPartialToString(string viewName,object model,ControllerContext controllerContext)
        {
            if(string.IsNullOrEmpty(viewName))
            {
                viewName = controllerContext.RouteData.GetRequiredString("action");
            }
            ViewDataDictionary ViewData = new ViewDataDictionary();
            TempDataDictionary TeampData = new TempDataDictionary();
            ViewData.Model = model;
            using (StringWriter sw = new StringWriter())
            {
                ViewEngineResult viewResult = ViewEngines.Engines.FindPartialView(controllerContext, viewName);
                ViewContext viewContext = new ViewContext(controllerContext, viewResult.View,ViewData,TeampData,sw);
                viewResult.View.Render(viewContext, sw);
                return sw.GetStringBuilder().ToString();
            }
            
        }
    }
}