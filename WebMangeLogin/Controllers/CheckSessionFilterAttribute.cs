using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace WebManageLogin.Controllers
{
    public class CheckSessionFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var session = context.HttpContext.Session;
            if (session.GetString("LoggedInUsername_Login") == null)
            {
                var controller = context.Controller as Controller;
                if (controller != null)
                {
                    controller.TempData["SessionError"] = "Not found session";
                }


                context.Result = new RedirectToActionResult("Login", "Login", null);
            }
        }
      
    }
}





