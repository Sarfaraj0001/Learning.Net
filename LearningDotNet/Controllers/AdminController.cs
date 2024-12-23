using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LearningDotNet.Models;

namespace LearningDotNet.Controllers
{
    public class AdminController : Controller
    {
        LearningDotNetEntities db = new LearningDotNetEntities();
        // GET: Admin

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AdminLogin(Admin obj)
        {
            var data = db.Admins.FirstOrDefault(u => u.UserName == obj.UserName && u.Password == obj.Password);
            if (data == null)
            {
                TempData["msg"] = "UserName or Password does not match";
                return RedirectToAction("Index");
            }
            TempData["msg"] = "Login Successful";
            Session["AdminId"] = data.id;    
            return RedirectToAction("Deshbord");
        }
        public ActionResult AdminLogout()
        {
            // Clear the session
            Session["AdminId"] = null;
            TempData["msg"] = "You have been logged out successfully!";
            return RedirectToAction("Index");
        }

        public ActionResult Deshbord()
        {
          
            return View();
        }

        public ActionResult Users(Register obj)
        {
            if(Session["AdminId"] != null)
            {
                var data = db.Registers.ToList();
                return View(data);
            }
            return RedirectToAction("Index");
        }

        public ActionResult Pending(Register obj)
        {
            if (Session["AdminId"] != null)
            {
                var data = db.Registers.Where(u => u.Status== "Pending").ToList();

                return View(data);
            }
            return RedirectToAction("Index");
        }

        public ActionResult ActivUsers(Register obj)
        {
            if (Session["AdminId"] != null)
            {
                var data = db.Registers.Where(u => u.Status == "Active").ToList();

                return View(data);
            }
            return RedirectToAction("Index");
        }

        public ActionResult DeActiveUsers(Register obj)
        {
            if (Session["AdminId"] != null)
            {
                var data = db.Registers.Where(u => u.Status == "Deactive").ToList();

                return View(data);
            }
            return RedirectToAction("Index");
        }

        public ActionResult UserStatus(int Id, string Status)
        {
            var data = db.Registers.Where(x => x.id == Id).FirstOrDefault();
            if(data == null)
            {
                TempData["msg"] = "id did not match!";
                return RedirectToAction("Users");
            }
            if(Status == "Active")
            {
                data.Status = "Active";
                db.Entry(data).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("ActivUsers");
            }
            else
            {
                data.Status = "Deactive";
                db.Entry(data).State = EntityState.Modified;
                db.SaveChanges();
                Session["UserId"] = null;
                Session["UserName"] = null;
                Session["UserStatus"] = null;
                return RedirectToAction("DeActiveUsers");
            }       
        }

        //public ActionResult DeActiveUser(int Id)
        //{
        //    var data = db.Registers.Where(x => x.id == Id).FirstOrDefault();

        //    if(data != null) 
        //    { 
        //    data.Status = "Deactive";
        //    db.Entry(data).State = EntityState.Modified;
        //    db.SaveChanges();
        //        Session["UserId"] = null;
        //        Session["UserName"] = null;
        //        Session["UserStatus"] = null;
        //    }
        //    return RedirectToAction("Users");
        //}



        public ActionResult Profile()
        {
            var data = db.Users.ToList();

            // Convert binary image data to Base64 string for display
            foreach (var user in data)
            {
                if (user.ProfileImage != null)
                {
                    //user.ProfileImageBase64 = Convert.ToBase64String(user.ProfileImage);
                }
            }

            return View(data);
        }

        public ActionResult GetAllUsers()
        {
          
            var data = db.Users.ToList();
                return View(data);
           
        }


        //getinng data from database without image
        public ActionResult GetUserDetailsForEdit(int userId)
        {
            var data = db.Users.Find(userId);
            return View(data);
        }


        //updating data into database without image

        [HttpPost]
        public ActionResult UpdateUserDetails(User user)
        {
            // Retrieve the existing user from the database
            var existingUser = db.Users.Find(user.UserId);

            if (existingUser != null)
            {
                // Update the user details, but keep the existing ProfileImage
                existingUser.FullName = user.FullName;
                existingUser.Email = user.Email;
                existingUser.Phone = user.Phone;
                existingUser.Mobile = user.Mobile;
                existingUser.Address = user.Address;

                // Do not modify ProfileImage, it should remain the same

                // Mark the entity as modified
                db.Entry(existingUser).State = EntityState.Modified;

                // Save changes to the database
                db.SaveChanges();

                return RedirectToAction("Index", "Admin");
            }
            else
            {
                // Handle case when the user is not found
                ModelState.AddModelError("", "User not found.");
                return View(user); // Return the same view with error
            }
        }
   
        //getinng image from database
        public ActionResult GetUserImageForEdit(int userId)
        {
            var data = db.Users.Find(userId);
            return View(data);
        }

        //Updating only image in database 
        //[HttpPost]
        //public ActionResult UpdateUserImage(int userId, HttpPostedFileBase profileImage)
        //{
        //    if (profileImage != null && profileImage.ContentLength > 0)
        //    {
        //        // Retrieve the existing user from the database
        //        var existingUser = db.Users.Find(userId);
        //        if (existingUser != null)
        //        {
        //            // Convert the uploaded file to binary data
        //            using (var memoryStream = new MemoryStream())
        //            {
        //                profileImage.InputStream.CopyTo(memoryStream);
        //                existingUser.ProfileImage = memoryStream.ToArray(); // Store as binary data
        //            }

        //            // Update only the ProfileImage field
        //            db.Entry(existingUser).Property(u => u.ProfileImage).IsModified = true;

        //            // Save changes to the database
        //            db.SaveChanges();

        //            return RedirectToAction("Index", "Admin");
        //        }
        //        else
        //        {
        //            // Handle case when the user is not found
        //            ModelState.AddModelError("", "User not found.");
        //        }
        //    }
        //    else
        //    {
        //        // Handle case when no file is selected or file is invalid
        //        ModelState.AddModelError("", "Please select a valid image file.");
        //    }

        //    // Return the same view in case of failure
        //    return View();
        //}

        //deleting only image from database 
        [HttpPost]
        public ActionResult DeleteProfileImage(int userId)
        {
            // Retrieve the existing user from the database
            var existingUser = db.Users.Find(userId);

            if (existingUser != null)
            {
                // Set the ProfileImage field to null
                existingUser.ProfileImage = null;

                // Mark the entity as modified
                db.Entry(existingUser).State = EntityState.Modified;

                // Save changes to the database
                db.SaveChanges();

                return RedirectToAction("Index", "Admin"); // Redirect to Admin Index after deletion
            }
            else
            {
                // Handle case when the user is not found
                ModelState.AddModelError("", "User not found.");
                return View(); // Return the view with an error
            }
        }

    }
}