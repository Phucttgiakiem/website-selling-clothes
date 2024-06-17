using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Website_clothe.Models;

namespace Website_clothe.Areas.Admin.Controllers
{
    public class ConditionController : Controller
    {
        Server_clothesEntities5 db = new Server_clothesEntities5();
        public ActionResult Index()
        {
            if (Session["AdminUsername"] == null)
            {
                return View("/Areas/Admin/Views/AdminUser/Login.cshtml");
            }
            var model = db.TrangThaiDHs.ToList();
            return View(model);
        }
        [HttpGet]
        public ActionResult Create()
        {
            if (Session["AdminUsername"] == null)
            {
                return View("/Areas/Admin/Views/AdminUser/Login.cshtml");
            }
            return View();
        }
        [HttpPost]
        public ActionResult Create(string tentrangthai)
        {
            
                var query = db.TrangThaiDHs.Where(x => x.TenTrangThai == tentrangthai);
            if (query.Any())
            {
                ViewBag.error = "Trạng thái đã có trên hệ thống";
                return View();
            }
                var data = new TrangThaiDH
                {
                    TenTrangThai = tentrangthai,
                };
                db.TrangThaiDHs.Add(data);
                db.SaveChanges();
                return RedirectToAction("Index");
            
           
        }
        [HttpGet]
        public ActionResult Edit(int id)
        {
            var model = db.TrangThaiDHs.FirstOrDefault(x => x.ID_TrangThai == id);
            return View(model);
        }
        [HttpPost]
        public ActionResult Edit(int id,string tentrangthai)
        {
            
           
                var data = db.TrangThaiDHs.FirstOrDefault(x => x.ID_TrangThai == id);
                data.TenTrangThai = tentrangthai;
                UpdateModel(data);
                db.SaveChanges();
                return RedirectToAction("Index");
            
         
        }
        public ActionResult Delete (int id)
        {
            var data = db.TrangThaiDHs.FirstOrDefault(x => x.ID_TrangThai == id);
            var donmuahang = db.Donmuahangs.Where(x => x.ID_Trangthai == id).ToList();
            List<int> madon = donmuahang.Select(x=> x.ID_Donhang).ToList();
            var chitietdon = db.Chitietdonhangs.Where(x => madon.Contains((int)x.ID_Donhang)).ToList();
            db.TrangThaiDHs.Remove(data);
            db.SaveChanges();

            if(donmuahang.Count() > 0)
            {
                db.Donmuahangs.RemoveRange(donmuahang);
                db.SaveChanges();
            }
            if(chitietdon.Count() > 0)
            {
                db.Chitietdonhangs.RemoveRange(chitietdon); 
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}