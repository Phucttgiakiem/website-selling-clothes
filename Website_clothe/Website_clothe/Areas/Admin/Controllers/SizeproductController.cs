using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Website_clothe.Models;

namespace Website_clothe.Areas.Admin.Controllers
{
    public class SizeproductController : Controller
    {
        public Server_clothesEntities5 db = new Server_clothesEntities5();
        public ActionResult Index()
        {
            if (Session["AdminUsername"] == null)
            {
                return View("/Areas/Admin/Views/AdminUser/Login.cshtml");
            }
            var model = db.Sizes.ToList();
            ViewBag.loai = db.Loais.ToList();
            return View(model);
        }
        [HttpGet]
        public ActionResult Create()
        {
            if (Session["AdminUsername"] == null)
            {
                return View("/Areas/Admin/Views/AdminUser/Login.cshtml");
            }
            ViewBag.loai = db.Loais.ToList();
            return View();
        }
        [HttpPost]
        public ActionResult Create(string tensize,int loaisp)
        {
            var result = db.Sizes.FirstOrDefault(x => x.ID_Loai == loaisp && x.TenSize == tensize);
            if(result != null)
            {
                ViewBag.error = "size đã tồn tại trên hệ thống";
                ViewBag.loai = db.Loais.ToList();
                return View();
            }
            var data = new Size
            {
                TenSize = tensize,
                ID_Loai = loaisp
            };
            db.Sizes.Add(data);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        [HttpGet]
        public ActionResult Edit(int id)
        {
            var model = db.Sizes.FirstOrDefault(x => x.ID_Size == id);
            ViewBag.Loai = db.Loais.ToList();
            return View(model);
        }
        [HttpPost]
        public ActionResult Edit(string tensize,int loaisp,int masize) 
        {
           // var result = db.Sizes.First(x => x.ID_Size == masize);
            var result = db.Sizes.Where(x => x.TenSize == tensize && x.ID_Loai == loaisp);
            if (result.Any())
            {
                ViewBag.error = "Size đã tồn tại trên hệ thống";
                var model = db.Sizes.FirstOrDefault(x => x.ID_Size == masize);
                ViewBag.Loai = db.Loais.ToList();
                return View(model);
            }
            var result1 = db.Sizes.FirstOrDefault(x => x.ID_Size == masize);
            result1.TenSize = tensize;
            result1.ID_Loai= loaisp;
            UpdateModel(result);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult Delete(int id)
        {
            var size = db.Sizes.FirstOrDefault(x => x.ID_Size == id);
            if (size != null)
            {
                var bientheIDs = db.Bienthesanphams.Where(x => x.SizeID == id).Select(x => x.ID_Bienthesanpham).ToList();

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
                db.Sizes.Remove(size);
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}