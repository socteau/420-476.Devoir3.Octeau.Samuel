using _420_476.Devoir3.Samuel.Octeau.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace _420_476.Devoir3.Samuel.Octeau.Controllers
{
    public class HomeController : Controller
    {
        private static string errorMessage = "";
        public ActionResult Login()
        {
            Session["firstName"] = null;
            Session["lastName"] = null;
            ViewBag.errorMessage = errorMessage;            
            return View();
        }

        public ActionResult submitLogin(string username, string password)
        {
            using (NorthwindEntities context = new NorthwindEntities())
            {
                //verifyUserIdentity = SELECT @PWD = password FROM Users WHERE login = @login
                //                     SELECT PWDCOMPARE(@password, @PWD)
                try
                {
                    var result = (int)context.verifyUserIdentity(username, password).FirstOrDefault();
                    if (result == 1)
                    {
                        errorMessage = "";
                        User user = context.Users.Where(x => x.login == username).FirstOrDefault();
                        Session["firstName"] = user.firstName;
                        Session["lastName"] = user.lastName;
                        Response.Redirect("~/Products/Index");
                    }
                    else
                        errorMessage = "Mot de passe erroné";
                }
                catch
                {
                    errorMessage = "Login inexistant";
                }
            }
            return RedirectToAction("Login");
        }
    }
}