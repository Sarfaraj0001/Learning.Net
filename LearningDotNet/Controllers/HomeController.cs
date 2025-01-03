﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LearningDotNet.Models;

namespace LearningDotNet.Controllers
{
    public class HomeController : Controller
    {
        LearningDotNetEntities db = new LearningDotNetEntities();
        // GET: Home
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }
        // POST: Home
        [HttpPost]
        public ActionResult UserRegister(Register1 obj)
        {
            Register obj1 = new Register();

            if (ModelState.IsValid)
            {               
                if (db.Registers.Any( u => u.Email == obj.Email))
                {
                    TempData["msg"] = "Email already exist";
                    return RedirectToAction("Index");
                }

                else if (db.Registers.Any(u => u.Username == obj.Username))
                {
                    TempData["msg"] = "Username already exist";
                    return RedirectToAction("Index");
                }

                else if (obj.Password != obj.ConfPassword)
                {
                    TempData["msg"] = "Password Not Match";
                    return RedirectToAction("Index");
                }
                else { 
                    obj1.Email = obj.Email;
                    obj1.Username = obj.Username;
                    obj1.Password = obj.Password;
                    obj1.Registered_On = DateTime.Now;
                    obj1.Status = obj.Status;               
                    obj1.Status = "Pending";
                    db.Registers.Add(obj1);
                    db.SaveChanges();
                    TempData["msg"] = "Registration Successfull";
                    return RedirectToAction("Index");
                }
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Login(Admin obj1, Register obj, string Identifier)
        {
            // Check if the ModelState is valid
            if (ModelState.IsValid)
            {
                // Check if the email exists in the database
                var userData = db.Registers.FirstOrDefault(u => u.Email == Identifier && u.Password == obj.Password);
                var adminData = db.Admins.FirstOrDefault(u => u.UserName == Identifier && u.Password == obj.Password);

                if (userData == null && adminData == null)
                {
                    TempData["msg"] = "Invalid Email or Password";
                    return RedirectToAction("Index");
                   
                }
                else if (adminData != null)
                {
                    TempData["msg"] = "Login successful!";
                    Session["AdminId"] = adminData.id;
                    //Session["UserName"] = userData.Username;
                    //Session["UserStatus"] = userData.Status;
                    return RedirectToAction("Dashboard", "Admin");
                }
                else if (userData != null && userData.Status == "Active")
                { 
                TempData["msg"] = "Login successful!";
                Session["UserId"] = userData.id;
                //Session["UserName"] = userData.Username;
                //Session["UserStatus"] = userData.Status;
                return RedirectToAction("Index");
                }
               
                else if (userData != null && userData.Status == "Deactive")
                {
                    TempData["msg"] = "Admin Rejected your login Approval!";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["msg"] = "Admin didn't Approved yet! Please contect your Admin";
                    return RedirectToAction("Index");
                }
            }
            TempData["msg"] = "Invalid login attempt";
            return RedirectToAction("Index");
        }

        public ActionResult Logout()
        {
            Session["UserId"] = null;
            Session["UserName"] = null;
            Session["UserStatus"] = null;

            TempData["msg"] = "You have been logged out successfully!";
            return RedirectToAction("Index");
        }

    }
}
public partial class Register1
{
    public int id { get; set; }
    public string Email { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string Status { get; set; }
    public string ConfPassword { get; set; }
}