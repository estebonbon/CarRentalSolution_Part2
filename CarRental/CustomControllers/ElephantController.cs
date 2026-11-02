using CarRental.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CarRental.CustomControllers
{
    /// <summary>
    /// The Elephant Controller has a good memory to help
    /// persist the Index Sort, Filter and Paging parameters
    /// into a URL stored in ViewData.
    ///
    /// WARNING: Depends on the following Utilities:
    /// - CookieHelper
    /// - MaintainURL
    /// </summary>
    public class ElephantController : CognizantController  //Retrieve controller and action names
    {
        // This is the list of Actions that will add
        // the ReturnURL to ViewData
        internal string[] ActionWithURL =
        [
            "Details",
            "Create",
            "Edit",
            "Delete",
            "Add",
            "Update",
            "Remove"
        ];
        //Get a return url for these

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (ActionWithURL.Contains(ActionName()))
            {
                ViewData["returnURL"] = MaintainURL.ReturnURL(
                    HttpContext,
                    ControllerName()
                );
            }
            else if (ActionName() == "Index")
            {
                // Clear the sort/filter/paging URL Cookie for Controller
                CookieHelper.CookieSet(
                    HttpContext,
                    ControllerName() + "URL",
                    "",
                    -1 //How exactly does the -1 make the url cleared, I guess it gets rid of the cookie immediatly meaning the program wont preserve any preferences
                );
            }

            base.OnActionExecuting(context);
        }

        public override Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (ActionWithURL.Contains(ActionName()))
            {
                ViewData["returnURL"] = MaintainURL.ReturnURL(
                    HttpContext,
                    ControllerName()
                );
            }
            else if (ActionName() == "Index")
            {
                // Clear the sort/filter/paging URL Cookie for Controller
                CookieHelper.CookieSet(
                    HttpContext,
                    ControllerName() + "URL",
                    "",
                    -1
                );
            }

            return base.OnActionExecutionAsync(context, next);
        }
    }
}