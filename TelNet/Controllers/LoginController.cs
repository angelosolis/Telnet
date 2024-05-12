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
    [OutputCache(NoStore = true, Duration = 0)]
    public class LoginController : BaseController
    {
        [AllowAnonymous] 
        public ActionResult Login()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult CreateApplication()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult Login(userCredentials u)
        {
            
            if (string.IsNullOrEmpty(u.username) || string.IsNullOrEmpty(u.eid) || string.IsNullOrEmpty(u.password))
            {
                TempData["ErrorMessage"] = "Please input username, EID, and password";
                return View(u);
            }

            // Check if the user exists in userCredentials table based on both username and EID
            var user = _userRepo.Table.FirstOrDefault(m => m.username == u.username && m.eid == u.eid);
            if (user != null)
            {
                if (user.password == u.password)
                {
                    FormsAuthentication.SetAuthCookie(u.username, false);

                    var userRoles = (from ur in _db.userRoles
                                     join r in _db.roles on ur.roleId equals r.id
                                     where ur.userId == user.userId
                                     select r.roleName).ToList();
                    
                    Session["CurrentlyLoggedIn"] = user.username;

                    if(userRoles.Contains("LALU"))
                    {
                        return RedirectToAction("CRM", "Home");
                    }
                    else if(userRoles.Contains("TU"))
                    { 
                        return RedirectToAction("HRMDashboard", "HRM"); 
                    }
                    else if (userRoles.Contains("CAU"))
                    {
                        return RedirectToAction("RatePlans", "SDFICO");
                    }
                    else if (userRoles.Contains("MCU"))
                    {
                        return RedirectToAction("AccountStanding", "CCAS");
                    }
                    else if (userRoles.Contains("IAS"))
                    {
                        return RedirectToAction("Index", "DocumentManagement");
                    }

                }
                else
                {
                    TempData["ErrorMessage"] = "Incorrect password";
                }
            }
            else
            {
                TempData["ErrorMessage"] = "User not found";
            }

            return View(u);
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Login");
        }
    }
}
        