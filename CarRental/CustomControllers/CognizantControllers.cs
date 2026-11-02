using CarRental.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.CodeAnalysis.Differencing;
using System.Collections;
using static System.Net.Mime.MediaTypeNames;

namespace CarRental.CustomControllers
{
    /// <summary>
    /// Makes the controller "self aware" knowing its own name
    /// and what Action was called.
    /// </summary>

    public class CognizantController : Controller
    {
        internal string ControllerName()
        {
            return ControllerContext.RouteData.Values["controller"].ToString();
        }

        internal string ActionName()
        {
            return ControllerContext.RouteData.Values["action"].ToString();
        }

        /// <summary>
        /// This method uses a regular expression to split the input string
        /// at each capital letter and then adds a space before each capital letter.
        /// The Trim() method removes any leading or trailing spaces.
        /// </summary>
        /// <param name="input">String in camel case to split into words</param>
        /// <returns>The camel case String split into words</returns>
        internal static string SplitCamelCase(string input)
        {
            return System.Text.RegularExpressions.Regex
                .Replace(
                    input,
                    "([A-Z])",
                    " $1",
                    System.Text.RegularExpressions.RegexOptions.Compiled
                ).Trim();
        }

        /// <summary>
        /// This code is executed before any Action method is called
        /// and gives us a chance to add to the ViewData dictionary.
        /// Makes these values available to all Views.
        /// </summary>
        /// <param name="context"></param>
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            // Add the Controller and Action names to ViewData
            string ControllerFriendlyName = SplitCamelCase(ControllerName());

            ViewData["ControllerName"] = ControllerName();
            ViewData["ControllerFriendlyName"] = ControllerFriendlyName;
            ViewData["ActionName"] = ActionName();
            ViewData["ActionNameLowerCase"] = ActionName().ToLower();
            ViewData["Title"] = ControllerFriendlyName + " " + ActionName();

            base.OnActionExecuting(context);
        }

        /// <summary>
        /// Same as above but for async Actions
        /// </summary>
        /// <param name="context"></param>
        /// <param name="next"></param>
        /// <returns></returns>
        public override Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // Add the Controller and Action names to ViewData
            string ControllerFriendlyName = SplitCamelCase(ControllerName());

            ViewData["ControllerName"] = ControllerName();
            ViewData["ControllerFriendlyName"] = ControllerFriendlyName;
            ViewData["ActionName"] = ActionName();
            ViewData["ActionNameLowerCase"] = ActionName().ToLower();
            ViewData["Title"] = ControllerFriendlyName + " " + ActionName();

            return base.OnActionExecutionAsync(context, next);
        }
    }
}


///Delegates dont really make sense i dont understand why we are using them or whats the point
//What do you mean the program doesnt know what the next step is???? It was running perfectly fine without this code!

/*Imagine a TV remote 📺
The remote has a Play button.

When you press Play, the TV starts playing.

The remote doesn't need to know which movie you're watching. It just knows how to tell the TV to continue.

 So the whole point of next is to give your code a way to tell MVC: "I'm finished doing my work. You can continue with the current action.*/


/*1.The computer is running the game and is about to pass the ball to Player 15.
2. Granny interrupts because she wants to check that both teams have the same number of players.
3. Granny finishes checking.
4. next() tells the game to continue with the pass that was already planned.
The important distinction is:

next doesn't decide who gets the ball. The game already decided Player 15 was receiving it.

next simply lets the game continue from where Granny interrupted.
*/

/*Now, why does YOUR professor need Granny?
Your professor wants MVC to automatically collect information like:
ViewData["ControllerName"] = ControllerName();
ViewData["ActionName"] = ActionName();


So when you click Edit Vehicle, Granny writes down:
-Controller = Vehicle
- Action = Edit
Then MVC continues to Edit().
But here's the ridiculous part you're noticing
For your particular project, Granny isn't doing anything that requires stopping the game for a long time.

She's just writing down two names.

Your professor could put that information inside every controller action, but then you'd repeat the same code everywhere.

So Granny's real purpose is to do one common job automatically for every action, instead of making every player do it themselves.*/

/*For Step 9, the actual goal is:

Remember the Index URL so users can return to their previous filters, sorting, and page number.*/