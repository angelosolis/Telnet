using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using TelNet.Contracts;
using TelNet.Repositories;
using System.IO;
using System.Security.Cryptography;

namespace TelNet.Controllers
{
    [OutputCache(NoStore = true, Duration = 0)]
    [AllowAnonymous]
    public class HomeController : BaseController
    {

        public ActionResult Dashboard()
        {
            if (!User.IsInRole("CEO"))
            {
                return RedirectToAction("Unauthorized", "Error");
            }
            return View();
            
        }

        public ActionResult CRM()
        {
            
                var userApplications = _userApplicationsRepo.GetAll().OrderByDescending(a => a.applicationId).ToList();

                return View(userApplications);
        }


        [HttpPost]
        public ActionResult CreateApplication(userApplications model, HttpPostedFileBase idImage)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Check if idImage is not null and is an image
                    if (idImage != null && idImage.ContentLength > 0 && idImage.ContentType.StartsWith("image"))
                    {
                        // Generate a unique identifier for the application
                        string uniqueId = Guid.NewGuid().ToString();

                        // Get the file extension
                        string extension = Path.GetExtension(idImage.FileName);

                        // Generate the file name with the desired format
                        string fileName = $"ApplicationID-{uniqueId}{extension}";

                        // Define the file path to save the image
                        string filePath = Path.Combine(Server.MapPath("~/Content/images/"), fileName);

                        // Save the uploaded image file to the specified location
                        idImage.SaveAs(filePath);


                        // Mapping ViewModel to Entity
                        var application = new userApplications
                        {
                            fullName = model.fullName,
                            mothersMaidenName = model.mothersMaidenName,
                            birthdate = model.birthdate,
                            typeOfId = model.typeOfId,
                            idNumber = model.idNumber,
                            gender = model.gender,
                            civilStatus = model.civilStatus,
                            citizenship = model.citizenship,
                            homeOwnership = model.homeOwnership,
                            homeAddress = model.homeAddress,
                            employmentStatus = model.employmentStatus,
                            totalHouseholdIncome = model.totalHouseholdIncome,
                            occupation = model.occupation,
                            occupationRank = model.occupationRank,
                            businessName = model.businessName,
                            businessAddress = model.businessAddress,
                            officeTelephone = model.officeTelephone,
                            email = model.email,
                            plans = model.plans,
                            modeOfPayment = model.modeOfPayment,
                            image = "/Content/images/" + fileName,
                        };


                        // Save the application to the database
                        var repository = new BaseRepository<userApplications>();
                        repository.Create(application);

                        // Log the creation action
                        LogNotification("Create", application.applicationId);

                        // Return JSON indicating success
                        return Json(new { success = true, message = "Application successfully created." });
                    }
                }
                catch (Exception)
                {
                    // Log the exception or handle it appropriately
                    ModelState.AddModelError("", "An error occurred while processing your request.");
                    return Json(new { success = false, message = "An error occurred while processing your request." });
                }
            }

            // If ModelState is not valid, return JSON with validation errors
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
            return Json(new { success = false, message = "Validation failed", errors = errors });
        }

        [HttpGet]
        public ActionResult GetApplicationDetails(int applicationId)
        {
            try
            {
                // Retrieve application details from the database based on the applicationId
                var application = _db.userApplications
                                    .Where(a => a.applicationId == applicationId)
                                    .Select(a => new
                                    {
                                        a.fullName,
                                        a.mothersMaidenName,
                                        a.birthdate,
                                        a.typeOfId,
                                        a.idNumber,
                                        a.gender,
                                        a.civilStatus,
                                        a.citizenship,
                                        a.homeOwnership,
                                        a.homeAddress,
                                        a.employmentStatus,
                                        a.totalHouseholdIncome,
                                        a.occupation,
                                        a.occupationRank,
                                        a.businessName,
                                        a.businessAddress,
                                        a.officeTelephone,
                                        a.status,
                                        a.denialReason,
                                        a.email,
                                        a.modeOfPayment,
                                        a.plans,
                                        a.image,
                                    })
                                    .FirstOrDefault();

                if (application == null)
                {
                    // If application not found, return 404 Not Found status
                    return HttpNotFound();
                }

                // Return application details as JSON
                return Json(application, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                // Log or handle the exception as needed
                return Json(new { success = false, message = "An error occurred: " + ex.Message });
            }
        }


        [HttpPost]
        public ActionResult UpdateApplicationStatus(int applicationId, string status, string reason)
        {
            try
            {
                // Find the user application by its ID
                var application = _db.userApplications.Find(applicationId);

                if (application == null)
                {
                    return Json(new { success = false, message = "Application not found." });
                }
            
                // Create a variable to hold the old status for logging purposes
                string oldStatus = application.status;

                // Update the status and reason
                application.status = status;

                // Optionally, update the reason as well
                application.denialReason = reason;

                // Save changes to the database
                _db.SaveChanges();

                // Log the update action
                LogNotification("Update", applicationId);

                
                
                if (application.status == "Approved")
                {
                    // Generate account number
                    string accountNumber = GenerateRandomDigits(7);

                    // Generate a generic password (you can replace this with your actual password generation logic)
                    string accountPassword = GenerateRandomPassword();

                    // Generate telephone number (random 4 digits after '343-')
                    string telephoneNumber = $"343-{GenerateRandomDigits(4)}";

                    // Determine service availed based on the plan (replace this with your actual logic)
                    string serviceAvailed = application.plans;

                    // Save data to customerAccounts table
                    var customerAccount = new customerAccounts
                    {
                        applicationId = application.applicationId,
                        accountNumber = accountNumber,
                        accountPassword = accountPassword,
                        telephoneNumber = telephoneNumber,
                        serviceAvailed = serviceAvailed
                    };

                    ViewBag.accountNumber = accountNumber;
                    ViewBag.accountPassword = accountPassword;
                    ViewBag.telephoneNumber = telephoneNumber;
                    ViewBag.serviceAvailed = serviceAvailed;
                    ViewBag.name = application.fullName;

                    // Save the customer account to the database
                    _customerAccountsRepo.Create(customerAccount);
                    // Prepare the email body with the OTP code
                    string emailApproved = RenderViewToString("~/Views/Home/EmailBodyApproved.cshtml");
                    SendEmail("telex.hdcompany@gmail.com", "yjkjjngeixvrnvip", application.email, "Application Update", emailApproved);
                }
                else
                {
                    // Prepare the email body with the OTP code
                    string emailDenied = RenderViewToString("~/Views/Home/EmailBodyDenied.cshtml");
                    SendEmail("telex.hdcompany@gmail.com", "yjkjjngeixvrnvip", application.email, "Application Update", emailDenied);
                }




                return Json(new { success = true, message = "Application status updated successfully." });
            }
            catch (Exception)
            {
                // Log the exception or handle it appropriately
                return Json(new { success = false, message = "An error occurred while updating the application status." });
            }
        }

        public static string GenerateRandomPassword(int length = 8)
        {
            const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%^&*()_+-=[]{}|;:,.<>?";
            var randomChars = new char[length];
            using (var rng = new RNGCryptoServiceProvider())
            {
                byte[] uintBuffer = new byte[sizeof(uint)];
                for (int i = 0; i < length; i++)
                {
                    rng.GetBytes(uintBuffer);
                    uint num = BitConverter.ToUInt32(uintBuffer, 0);
                    randomChars[i] = validChars[(int)(num % (uint)validChars.Length)];
                }
            }
            return new string(randomChars);
        }

        // Generate random digits
        public static string GenerateRandomDigits(int length)
        {
            const string validChars = "0123456789";
            var randomChars = new char[length];
            using (var rng = new RNGCryptoServiceProvider())
            {
                byte[] uintBuffer = new byte[sizeof(uint)];
                for (int i = 0; i < length; i++)
                {
                    rng.GetBytes(uintBuffer);
                    uint num = BitConverter.ToUInt32(uintBuffer, 0);
                    randomChars[i] = validChars[(int)(num % (uint)validChars.Length)];
                }
            }
            return new string(randomChars);
        }
        public ActionResult EmailBodyDenied()
        {
            return View();
        }

        public ActionResult EmailBodyApproved()
        {
            return View();
        }
        // Helper method to render a view to string
        private string RenderViewToString(string viewName)
        {
            try
            {
                using (var sw = new StringWriter())
                {
                    var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                    var viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                    viewResult.View.Render(viewContext, sw);
                    return sw.GetStringBuilder().ToString();
                }
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as appropriate for your application
                // For example, you could log it to a file or database, or return a default error message
                // You could also re-throw the exception to propagate it to the calling code
                // Alternatively, you could return a specific error message or handle it differently based on the type of exception

                // For demonstration purposes, returning a default error message
                return "An error occurred while rendering the view: " + ex.Message;
            }
        }
        public ActionResult DataAnalysis()
        {
            // Assuming userApplications is a property or method that returns a collection of user applicants
            var userApplications = _userApplicationsRepo.GetAll(); // Assuming this retrieves user applications

            // Count the number of user applicants
            int userApplicantsCount = userApplications.Count();
            int deniedApplicantsCount = userApplications.Count(a => a.status == "Denied    ");
            int approvedApplicantsCount = userApplications.Count(a => a.status == "Approved  ");
            ViewBag.DeniedApplicantsCount = deniedApplicantsCount;
            ViewBag.ApprovedApplicantsCount = approvedApplicantsCount;
            ViewBag.UserApplicantsCount = userApplicantsCount;

            // Pass other data to the view if needed
            return View(userApplications);
        }

        // Helper method to send email
        private void SendEmail(string fromMail, string fromPassword, string toMail, string subject, string body)
        {
            MailMessage message = new MailMessage();
            message.From = new MailAddress(fromMail, "TeleX");
            message.Subject = subject;
            message.To.Add(new MailAddress(toMail));
            message.Body = body;
            message.IsBodyHtml = true;

            using (var smtpClient = new SmtpClient("smtp.gmail.com"))
            {
                smtpClient.Port = 587;
                smtpClient.Credentials = new NetworkCredential(fromMail, fromPassword);
                smtpClient.EnableSsl = true;

                smtpClient.Send(message);
            }
        }

        [HttpPost]
        public ActionResult DeleteApplicationStatus(int applicationId)
        {
            try
            {
                // Attempt to delete the application status from the database
                var result = _userApplicationsRepo.Delete(applicationId);

                if (result == ErrorCode.Success)
                {
                    // Log the deletion action
                    LogNotification("Delete", applicationId);

                    // Return success message
                    return Json(new { success = true });
                }
                else
                {
                    // Return error message
                    return Json(new { success = false, message = "Failed to delete application status." });
                }
            }
            catch (Exception ex)
            {
                // Log the exception or handle it appropriately
                return Json(new { success = false, message = "An error occurred: " + ex.Message });
            }
        }


        private void LogNotification(string actionType, int applicationId)
        {
            try
            {
                // Create a new notification log entry
                var notificationLog = new notificationLogs
                {
                    actionType = actionType,
                    applicationId = applicationId,
                    actionDate = DateTime.Now
                };

                // Optionally, include additional information based on the action type
                if (actionType == "Update")
                {
                }
                else if (actionType == "Delete")
                {
                }

                // Save the notification log entry to the database
                var repository = new BaseRepository<notificationLogs>();
                repository.Create(notificationLog);
            }
            catch (Exception ex)
            {
                // Log or handle the exception as needed
                Console.WriteLine($"Error logging notification: {ex.Message}");
            }
        }


        public ActionResult Notifications()
        {

                var notifications = _notificationRepo.GetAll();
                return View(notifications);

        }

        public ActionResult CustomerCaptureInformation()
        {

                var userApplicants = _userApplicationsRepo.GetAll();
                return View(userApplicants);

        }

        public ActionResult RegisteredCustomers()
        {
            var customers = _customerAccountsRepo.GetAll();
            return View(customers);
        }
        public ActionResult Provision()
        {
            return View();
        }

    }
}