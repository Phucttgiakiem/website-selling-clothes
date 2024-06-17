using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Website_clothe.Models;

namespace Website_clothe.Areas.Admin.Controllers
{
    public class businessprovideController : Controller
    {
        public Server_clothesEntities5 db = new Server_clothesEntities5();
        // GET: Admin/businessprovide
        public ActionResult Index()
        {
            if (Session["AdminUsername"] == null)
            {
                return View("/Areas/Admin/Views/AdminUser/Login.cshtml");
            }
            var model = db.Nhacungcaps.ToList(); 
            return View(model);
        }
        [HttpGet]
        public ActionResult Edit(int id)
        {
            var resuilt = db.Nhacungcaps.Where(x => x.Manhacungcap == id).FirstOrDefault();
            return View(resuilt);
        }
        [HttpPost]
        public ActionResult Edit(string tennhacungcap,string area,string numberphone,int id_business)
        {
            var resuilt = db.Nhacungcaps.Where(x => x.Manhacungcap == id_business).FirstOrDefault();
            resuilt.Tennhacungcap = tennhacungcap;
            resuilt.Diachi = area;
            resuilt.SoDT = numberphone;
            UpdateModel(resuilt);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        [HttpGet]
        public ActionResult Create()
        {
            if (Session["AdminUsername"] == null)
            {
                return View("Index", "AdminUser");
            }
            return View();
        }
        [HttpPost]
        public ActionResult Create(string namebusiness,string area,string numberphone)
        {
            var resuilt = db.Nhacungcaps.Where(x => x.Tennhacungcap == namebusiness 
            && x.Diachi == area && x.SoDT == numberphone);

            if (resuilt.Any())
            {
                ViewBag.error = "Nhà cung cấp đã tồn tại trên hệ thống";
                return View();
            }
            var data = new Nhacungcap
            {
                Tennhacungcap = namebusiness,
                Diachi = area,
                SoDT = numberphone
            };
            db.Nhacungcaps.Add(data);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult Delete(int id) {
            var provide = db.Nhacungcaps.FirstOrDefault(x => x.Manhacungcap == id);
            if (provide != null)
            {
                var sanpham = db.Sanphams.Where(x => x.ID_Nhacungcap == provide.Manhacungcap).ToList();
                if (sanpham.Any())
                {
                    var sanphamIDs = sanpham.Select(x => x.Masanpham).ToList();
                    var bienthesp = db.Bienthesanphams.Where(x => sanphamIDs.Contains((int)x.ID_Sanpham)).ToList();
                    if (bienthesp.Any())
                    {
                        var bientheIDs = bienthesp.Select(x => x.ID_Bienthesanpham).ToList();
                        var donmuact = db.Chitietdonhangs.Where(x => bientheIDs.Contains((int)x.ID_Sanphambienthe)).ToList();
                        if (donmuact.Any())
                        {
                            var donDatIDs = donmuact.Where(x => x.ID_Donhang.HasValue).Select(x => x.ID_Donhang.Value).ToList();
                            var dondat = db.Donmuahangs.Where(x => donDatIDs.Contains(x.ID_Donhang)).ToList();
                            if (dondat.Any())
                            {
                                db.Donmuahangs.RemoveRange(dondat);
                                db.SaveChanges();
                            }
                            db.Chitietdonhangs.RemoveRange(donmuact);
                            db.SaveChanges();
                        }
                        var phieunhapct = db.Chitietphieunhaps.Where(x => bientheIDs.Contains((int)x.ID_Chitietphieunhap)).ToList();
                        if (phieunhapct.Any())
                        {
                            var phieunhapIDs = phieunhapct.Where(x => x.ID_Phieunhaphang.HasValue).Select(x => x.ID_Phieunhaphang.Value).ToList();
                            var phieunhap = db.Phieunhaps.Where(x => phieunhapIDs.Contains(x.ID_Phieunhap)).ToList();
                            if (phieunhap.Any())
                            {
                                db.Phieunhaps.RemoveRange(phieunhap);
                                db.SaveChanges();
                            }
                        }
                        db.Bienthesanphams.RemoveRange(bienthesp);
                        db.SaveChanges();
                    }
                    var hinhsp = db.HinhanhSanphams.Where(x => sanphamIDs.Contains((int)x.Masanpham)).ToList();
                    if (hinhsp.Any())
                    {
                        foreach (var item in hinhsp)
                        {
                            deletefile_image(item.TenAnh);

                        }
                        db.HinhanhSanphams.RemoveRange(hinhsp);
                        db.SaveChanges();
                    }
                }
                db.Nhacungcaps.Remove(provide);
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }
        public ActionResult Detail (int id)
        {
            var resuilt = db.Nhacungcaps.Where(x => x.Manhacungcap == id).FirstOrDefault();
            return View(resuilt);
        }
        public void deletefile_image(string name_image)
        {
            try
            {
                string folderPath = Server.MapPath("~/Content/img/");
                string filePath = Path.Combine(folderPath, name_image);
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }
            catch (Exception e)
            {

            }
            return;
        }
    }
}