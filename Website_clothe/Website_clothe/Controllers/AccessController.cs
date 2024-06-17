using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.ModelBinding;
using System.Web.Mvc;
using BCrypt.Net;
using Website_clothe.Models;

namespace Website_clothe.Controllers
{
    public class AccessController : Controller
    {
        
        Server_clothesEntities5 db = new Server_clothesEntities5();
        [HttpGet]
        public ActionResult Login()
        {
            if (Session["username"] == null )
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index","Home");
            }
        }
        [HttpPost]
        public ActionResult Login (User_and_account user)
        {
           
            if (Session["username"] == null)
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
                        if (BCrypt.Net.BCrypt.Verify(user.password, u.Password))
                        {
                            Session["Manguoidung"] = u.ID_Nguoidung;
                            Session["username"] = user.username;
                            Session["password"] = user.password;

                            if (user.Remember)
                            {
                                // Lưu tên đăng nhập vào cookie
                                HttpCookie usernameCookie = new HttpCookie("Username");
                                usernameCookie.Value = user.username;
                                usernameCookie.Expires = DateTime.Now.AddDays(7); // Cookie hết hạn sau 7 ngày
                                Response.Cookies.Add(usernameCookie);

                                // Lưu mật khẩu vào cookie
                                HttpCookie passwordCookie = new HttpCookie("Password");
                                passwordCookie.Value = user.password;
                                passwordCookie.Expires = DateTime.Now.AddDays(7); // Cookie hết hạn sau 7 ngày
                                Response.Cookies.Add(passwordCookie);
                            }

                               
                            return RedirectToAction("Index", "Home");
                        }
                    }
                }
            }
            ModelState.AddModelError("", "Tên đăng nhập hoặc mật khẩu không đúng");
            return View(user);
        }
        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }
        

        [HttpPost]
        public ActionResult Register(register_user ruser)
        {
            if(!ModelState.IsValid)
            {
                return View(ruser);
            }
            else
            {
                var u = db.Nguoidungs.Where(x => x.Username == ruser.username ||
                                                x.Password == ruser.Password ||
                                                x.Email == ruser.Email ||
                                                x.SoDT == ruser.Sodt).FirstOrDefault();
                if (u != null)
                {
                    if (u.Username == ruser.username)
                    {
                        ModelState.AddModelError("", "Tên đăng nhập đã có người sử dụng");
                    }
                    if (u.Password == ruser.Password)
                    {
                        ModelState.AddModelError("", "Mật khẩu đã có người sử dụng");
                    }
                    if (u.Email == ruser.Email)
                    {
                        ModelState.AddModelError("", "Email đã có người sử dụng");
                    }
                    if (u.SoDT == ruser.Sodt)
                    {
                        ModelState.AddModelError("", "Số điện thoại đã có người sử dụng");
                    }
                    return View(ruser);
                }
                else
                {
                    string hashpass = BCrypt.Net.BCrypt.HashPassword(ruser.Password);
                    var user = new Nguoidung
                    {
                        Tennguoidung = ruser.Tennguoidung,
                        Username = ruser.username,
                        SoDT = ruser.Sodt,
                        Email = ruser.Email,
                        Quyentruycap = 0,
                        Password = hashpass
                    };
                    db.Nguoidungs.Add(user);
                    db.SaveChanges();
                    Session["Manguoidung"] = user.ID_Nguoidung;
                    Session["username"] = ruser.username;
                    Session["password"] = ruser.Password;
                    return RedirectToAction("Index", "Home");
                }
            }
        }
        [HttpGet]
        public ActionResult ForgetPass()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ForgetPass(Forgetpassword forgetp)
        {
            if (!ModelState.IsValid)
            {
                return View(forgetp);
            }
            else
            {
                var u = db.Nguoidungs.SingleOrDefault(x => x.Username == forgetp.username);
                if (u != null)
                {
                    if(BCrypt.Net.BCrypt.Verify(forgetp.password, u.Password))
                    {
                        ModelState.AddModelError("", "Mật khẩu đã được sử dụng");
                        return View(forgetp);
                    }
                    string hasspass = BCrypt.Net.BCrypt.HashPassword(forgetp.password);
                    u.Password = hasspass;
                    db.SaveChanges();
                    return RedirectToAction("Login", "Access");
                }
                ModelState.AddModelError("", "Tên đăng nhập không tồn tại vui lòng nhập lại");
                return View(forgetp);
            }
        }
        public ActionResult Logout()
        {
                Session.Remove("Manguoidung");
                Session.Remove("username");
                Session.Remove("password");
            
            return RedirectToAction("Index", "Home");
        }
        public ActionResult MyInfomation()
        {
            return View();
        }
        public ActionResult Partial_InfoUser()
        {
            int id_user = (int)Session["Manguoidung"];
            Nguoidung model = null;
            if(id_user != 0)
            {
                model = db.Nguoidungs.FirstOrDefault(x => x.ID_Nguoidung== id_user);
            }
          
            return PartialView(model);
        }
        [HttpPost]
        public ActionResult Updatemyinfo(Nguoidung model)
        {
            var code = new { Success = false, Code = -1 };
            if (ModelState.IsValid)
            {
                int id_user = (int)Session["Manguoidung"];
                var data = db.Nguoidungs.FirstOrDefault(x => x.ID_Nguoidung == id_user);
                data.Tennguoidung = model.Tennguoidung;
                data.Ngaysinh = model.Ngaysinh;
                data.Diachi= model.Diachi;
                data.Email= model.Email;
                data.SoDT= model.SoDT;
                data.Username= model.Username;
                UpdateModel(data);
                db.SaveChanges();
                code = new { Success = true, Code = 1 };
                return Json(code);
            }
            return Json(code);
        }
        [HttpPost]
        public ActionResult UploadImageUser(HttpPostedFileBase formData)
        {
            var code = new { Success = false };
            if (formData != null)
            {
                string fileExtension = Path.GetExtension(formData.FileName);
                if (fileExtension != null && (fileExtension.Equals(".jpg", StringComparison.OrdinalIgnoreCase) ||
                                              fileExtension.Equals(".jpeg", StringComparison.OrdinalIgnoreCase) ||
                                              fileExtension.Equals(".png", StringComparison.OrdinalIgnoreCase) ||
                                              fileExtension.Equals(".gif", StringComparison.OrdinalIgnoreCase)))
                {
                
                    string extension = Path.GetExtension(fileExtension);
                    string uniqueFileName = Path.GetFileNameWithoutExtension(fileExtension) + "_" + Guid.NewGuid().ToString() + extension;
                    string path = Path.Combine(Server.MapPath("~/Content/img"), uniqueFileName);
                    formData.SaveAs(path);
                    int manguoidung = (int)Session["Manguoidung"];
                    var data = db.Nguoidungs.FirstOrDefault(x => x.ID_Nguoidung == manguoidung);
                    if (data != null)
                    {
                        data.HinhAnh = uniqueFileName;
                        UpdateModel(data);
                        db.SaveChanges();
                        code = new { Success = true };
                        return Json(code);
                    }
                }
            }
            return Json(code);
        }
    }
}