using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using LibraryManagementSystem;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace LibraryManagementSystem.Controllers
{
    public class UsersController : Controller
    {
        private LlibraryMmanagementSystemEntities db = new LlibraryMmanagementSystemEntities();


        // GET: Users/Create

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(User user)
        {
            using (var context = new LlibraryMmanagementSystemEntities())
            {
                
                StringBuilder hash = new StringBuilder();
                MD5CryptoServiceProvider md5provider = new MD5CryptoServiceProvider();
                byte[] bytes = md5provider.ComputeHash(new UTF8Encoding().GetBytes(user.Password));

                for (int i = 0; i < bytes.Length; i++)
                {
                    hash.Append(bytes[i].ToString("x2"));
                }
                user.Password = hash.ToString();
                bool isValid = context.Users.Any(u => u.UserName == user.UserName && u.Password == user.Password);
                if (isValid)
                {
                    FormsAuthentication.SetAuthCookie(user.UserName, false);
                    sendMail(user);
                    return RedirectToAction("Index", "Books");
                }
                else {
                    ModelState.AddModelError("", "Invalid UserName or Password.");
                }
            }
            return RedirectToAction("Login");
        }

        // GET: Users/Create
        public ActionResult Signup()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Signup(User user)
        {
            using (var context = new LlibraryMmanagementSystemEntities())
            {

                StringBuilder hash = new StringBuilder();
                MD5CryptoServiceProvider md5provider = new MD5CryptoServiceProvider();
                byte[] bytes = md5provider.ComputeHash(new UTF8Encoding().GetBytes(user.Password));

                for (int i = 0; i < bytes.Length; i++)
                {
                    hash.Append(bytes[i].ToString("x2"));
                }
                user.Password = hash.ToString();
                context.Users.Add(user);
                context.SaveChanges();
            }
            return RedirectToAction("Login");
        }


        public static void sendMail(User user)
        {
            try
            {
                string to = user.UserName;
            string from = "navu0503@gmail.com";
            MailMessage message = new MailMessage(from, to);
            message.Subject = "Using the new SMTP client.";
            message.Body = @"<h2>Congratulation, if you got it!</h2>";
            message.IsBodyHtml = true;
                    
            SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
            client.EnableSsl = true;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = true;
            client.Credentials = new System.Net.NetworkCredential("navu0503@gmail.com", "Nn@@0503");


            // Credentials are necessary if the server requires the client
            // to authenticate before it will send email on the client's behalf.

                client.Send(message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception caught in CreateTestMessage2(): {0}",
                    ex.ToString());
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
