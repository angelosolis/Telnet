using TelNet.Contracts;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace TelNet.Controllers
{
    public class LoginController : BaseController
    {
        [AllowAnonymous] //
        public ActionResult Login()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult Login(userCredentials u)
        {

            if (string.IsNullOrEmpty(u.username) || string.IsNullOrEmpty(u.password))
            {
                ModelState.AddModelError("", "Please input username and password");
                return View(u);
            }

            // Check if the user exists in userRepo
            var user = _userRepo.Table.FirstOrDefault(m => m.username == u.username);
            if (user != null)
            {
                // Verify the password
                if (user.password == u.password)
                {
                        // Set authentication cookie
                        FormsAuthentication.SetAuthCookie(u.username, false);

                        // Store user ID in session
                        Session["UserId"] = user.userId;

                        return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "User's status information not found");
                }
            }
            else
            {
                ModelState.AddModelError("", "Incorrect password");
            }
            return View(u);

        }
    }
}
        