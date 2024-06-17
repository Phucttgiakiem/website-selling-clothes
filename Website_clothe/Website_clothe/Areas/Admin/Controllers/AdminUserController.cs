using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Website_clothe.Models;

namespace Website_clothe.Areas.Admin.Controllers
{
    public class AdminUserController : Controller
    {
        Server_clothesEntities5 db = new Server_clothesEntities5();
        [HttpGet]
        public ActionResult Login()
        {
            if (Session["AdminUsername"] == null)
            {
                return View();
            }
            return RedirectToAction("Index", "Colorproduct");
        }
        [HttpPost]
        public ActionResult Login(User_and_account user)
        {
            if (Session["AdminUsername"] == null)
            {

                if (!ModelState.IsValid)
                {
                    return View(user);
                }
                else
                {
                    var u = db.Nguoidungs.Where(x => x.Username.Equals(user.username)).FirstOrDefault();
                    if (u != null)
                    {
                        if (u.Quyentruycap != 1)
                        {
                            ModelState.AddModelError("", "Tài khoản này không được quyền đăng nhập tại trang web này");
                            return View(user);
                        }
                        else
                        {
                            if (BCrypt.Net.BCrypt.Verify(user.password, u.Password))
                            {
                                Session["IDAdmin"] = u.ID_Nguoidung;
                                Session["AdminUsername"] = user.username;
                                Session["Adminpassword"] = user.password;

                                if (user.Remember)
                                {
                                    // Lưu tên đăng nhập vào cookie
                                    HttpCookie usernameCookie = new HttpCookie("AdminUsername");
                                    usernameCookie.Value = user.username;
                                    usernameCookie.Expires = DateTime.Now.AddDays(7); // Cookie hết hạn sau 7 ngày
                                    Response.Cookies.Add(usernameCookie);

                                    // Lưu mật khẩu vào cookie
                                    HttpCookie passwordCookie = new HttpCookie("Adminpassword");
                                    passwordCookie.Value = user.password;
                                    passwordCookie.Expires = DateTime.Now.AddDays(7); // Cookie hết hạn sau 7 ngày
                                    Response.Cookies.Add(passwordCookie);
                                }


                                return RedirectToAction("Index", "Colorproduct");
                            }
                        }
                    }
                    
                }
            }
            ModelState.AddModelError("", "Tên đăng nhập hoặc mật khẩu không đúng");
            return View(user);
        }
        [HttpGet]
        public ActionResult ForgotPass()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ForgotPass(Forgetpassword user)
        {
            if (!ModelState.IsValid)
            {
                return View(user);
            }
            else
            {

                var u = db.Nguoidungs.SingleOrDefault(x => x.Username == user.username);
                if (u != null)
                {
                    if(u.Quyentruycap != 1)
                    {
                        ModelState.AddModelError("", "Tài khoản này không được quyền sử dụng tại trang web này");
                        return View(user);
                    }
                    if (BCrypt.Net.BCrypt.Verify(user.password, u.Password))
                    {
                        ModelState.AddModelError("", "Mật khẩu đã được sử dụng");
                        return View(user);
                    }
                    string hasspass = BCrypt.Net.BCrypt.HashPassword(user.password);
                    u.Password = hasspass;
                    db.SaveChanges();
                    return RedirectToAction("Login", "AdminUser");
                }
                ModelState.AddModelError("", "Tên đăng nhập không tồn tại vui lòng nhập lại");
                return View(user);
            }
           
        }
        public ActionResult Logout()
        {
            if (Session["AdminUsername"] != null)
            {
                Session.Remove("IDAdmin");
                Session.Remove("AdminUsername");
                Session.Remove("Adminpassword");
            }
            return RedirectToAction("Login");
        }
    }
}