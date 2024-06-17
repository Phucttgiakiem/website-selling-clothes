using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Website_clothe.Models;

namespace Website_clothe.Areas.Admin.Controllers
{
    public class ColorproductController : Controller
    {
        public Server_clothesEntities5 db = new Server_clothesEntities5();
        public ActionResult Index()
        {
            if (Session["AdminUsername"] == null)
            {
                return View("/Areas/Admin/Views/AdminUser/Login.cshtml");
            }
            var result = db.MauSacs.ToList();
            ViewBag.Loai = db.Loais.ToList();
            return View(result);
        }
        [HttpGet]
        public ActionResult Create()
        {
            if (Session["AdminUsername"] == null)
            {
                return View("/Areas/Admin/Views/AdminUser/Login.cshtml");
            }
            ViewBag.Loai = db.Loais.ToList();
            return View();
        }
        [HttpPost]
        public ActionResult Create(string tenmau,int loaisp)
        {
            var existingMau = db.MauSacs.FirstOrDefault(p => p.Tenmau == tenmau && p.ID_Loai == loaisp);

            if (existingMau != null)
            {
                ViewBag.error = "Màu đã được tạo trên hệ thống";
                ViewBag.Loai = db.Loais.ToList();
                return View();
            }
            var data = new MauSac
            {
                Tenmau = tenmau,
                ID_Loai = loaisp
            };
            db.MauSacs.Add(data);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        [HttpGet]
        public ActionResult Edit (int id)
        {
            var model = db.MauSacs.FirstOrDefault(x => x.ID_Mau == id);
            ViewBag.Loai = db.Loais.ToList();
            return View(model);
        }
        [HttpPost]
        public ActionResult Edit (string tenmau,int loaisp,int mamau)
        {
            var resuil = db.MauSacs.Where(x => x.ID_Loai == loaisp && x.Tenmau == tenmau);
            if(resuil.Any())
            {
                ViewBag.error = "Sản phẩm đã tồn tại trên hệ thống";
                var model = db.MauSacs.FirstOrDefault(x => x.ID_Mau == mamau);
                ViewBag.Loai = db.Loais.ToList();
                return View(model);
            }
            var resuil1 = db.MauSacs.FirstOrDefault(x => x.ID_Mau == mamau);
            resuil1.Tenmau = tenmau;
            resuil1.ID_Loai = loaisp;
            UpdateModel(resuil1);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult Delete (int id)
        {
            
            var mauSac = db.MauSacs.FirstOrDefault(x => x.ID_Mau == id);
            if (mauSac != null)
            {
                var bientheIDs = db.Bienthesanphams.Where(x => x.ID_Mau == id).Select(x => x.ID_Bienthesanpham).ToList();

                var chiTietDonHangs = db.Chitietdonhangs.Where(x => bientheIDs.Contains((int)x.ID_Sanphambienthe)).ToList();
                if (chiTietDonHangs.Any())
                {
                    var donDatIDs = chiTietDonHangs.Where(x => x.ID_Donhang.HasValue).Select(x => x.ID_Donhang.Value).ToList();
                    var donDat = db.Donmuahangs.Where(x => donDatIDs.Contains(x.ID_Donhang)).ToList();
                    db.Chitietdonhangs.RemoveRange(chiTietDonHangs);
                    db.SaveChanges();
                    if (donDat.Any())
                    {
                        db.Donmuahangs.RemoveRange(donDat);
                        db.SaveChanges();
                    }
                    
                }

                var chiTietPhieuNhaps = db.Chitietphieunhaps.Where(x => bientheIDs.Contains((int)x.ID_Bienthesanpham)).ToList();
                if (chiTietPhieuNhaps.Any())
                {
                    var phieuNhapIDs = chiTietPhieuNhaps.Select(x => x.ID_Phieunhaphang).ToList();
                    var phieuNhap = db.Phieunhaps.Where(x => phieuNhapIDs.Contains(x.ID_Phieunhap)).ToList();
                    db.Chitietphieunhaps.RemoveRange(chiTietPhieuNhaps);
                    db.SaveChanges();
                    if (phieuNhap.Any())
                    {
                        db.Phieunhaps.RemoveRange(phieuNhap);
                        db.SaveChanges();
                    }
                    
                }

                db.Bienthesanphams.RemoveRange(db.Bienthesanphams.Where(x => bientheIDs.Contains(x.ID_Bienthesanpham)));
                db.SaveChanges();
                db.MauSacs.Remove(mauSac);
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}