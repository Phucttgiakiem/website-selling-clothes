using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Website_clothe.Models;

namespace Website_clothe.Areas.Admin.Controllers
{
    public class ManageruserController : Controller
    {
        public Server_clothesEntities5 db = new Server_clothesEntities5();
        public ActionResult Index()
        {
            if (Session["AdminUsername"] == null)
            {
                return View("/Areas/Admin/Views/AdminUser/Login.cshtml");
            }
            var model = db.Nguoidungs.ToList();
            return View(model);
        }
        public ActionResult Detail(int id)
        {
            var model = db.Nguoidungs.FirstOrDefault(x => x .ID_Nguoidung == id);
            return View(model);
        }
        [HttpGet]
        public ActionResult Edit(int id)
        {
            var model = db.Nguoidungs.FirstOrDefault(x => x.ID_Nguoidung == id);
            return View(model);
        }
        [HttpPost]
        public ActionResult Edit(Nguoidung model)
        {
            if(ModelState.IsValid)
            {
                 var user = db.Nguoidungs.FirstOrDefault(x => x.ID_Nguoidung == model.ID_Nguoidung);
                    user.Tennguoidung = model.Tennguoidung;
                    user.Ngaysinh = model.Ngaysinh;
                    user.Diachi = model.Diachi;
                    user.SoDT = model.SoDT;
                    user.Email = model.Email;
                    user.Username = model.Username;
                    UpdateModel(user);
                    db.SaveChanges();
                    return RedirectToAction("Index");
            }
            return View(model);
        }
    }
}