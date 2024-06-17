using Antlr.Runtime.Tree;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Website_clothe.Models;

namespace Website_clothe.Areas.Admin.Controllers
{
    public class TypeproductController : Controller
    {
        public Server_clothesEntities5 db = new Server_clothesEntities5();
        // GET: Admin/Typeproduct
        public ActionResult Index()
        {
            if (Session["AdminUsername"] == null)
            {
                return View("/Areas/Admin/Views/AdminUser/Login.cshtml");
            }
            var result = db.Loais.ToList();
            return View(result);
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
        public ActionResult Create(string tentheloai)
        {
            var query = db.Loais.Where(x => x.Tenloai== tentheloai);
            if (query.Any())
            {
                ViewBag.error = "Tên thể loại đã tồn tại";
                return View();
            }
            var data = new Loai
            {
                Tenloai = tentheloai
            };
            db.Loais.Add(data);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        [HttpGet]
        public ActionResult Edit(int id) {
            var result = db.Loais.Where(x => x.ID_Loai== id).FirstOrDefault();
            return View(result);
        }
        [HttpPost]
        public ActionResult Edit(string tentheloai,int maloai) {
            var result = db.Loais.First(x => x.ID_Loai == maloai);
            result.Tenloai= tentheloai;
            UpdateModel(result);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult Delete(int id)
        {
            var Loaisp = db.Loais.FirstOrDefault(x => x.ID_Loai == id);
            if (Loaisp == null)
            {
                var sanpham = db.Sanphams.Where(x => x.ID_Loai == id).ToList();
                if(sanpham != null &&  sanpham.Count() > 0)
                {
                    var sanphamIDs = sanpham.Select(x => x.Masanpham).ToList();
                    var Bienthesp = db.Bienthesanphams.Where(x => sanphamIDs.Contains((int)x.ID_Sanpham)).ToList();
                    if (Bienthesp.Any())
                    {
                        var bientheIDs = Bienthesp.Select(x => x.ID_Bienthesanpham).ToList();
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
                        db.Bienthesanphams.RemoveRange(Bienthesp);
                        db.SaveChanges();
                    }
                    var Hinhanhsp = db.HinhanhSanphams.Where(x => sanphamIDs.Contains((int)x.Masanpham)).ToList();
                    if (Hinhanhsp.Any())
                    {
                        foreach (var item in Hinhanhsp)
                        {
                            deletefile_image(item.TenAnh);

                        }
                        db.HinhanhSanphams.RemoveRange(Hinhanhsp);
                        db.SaveChanges();
                    }
                    db.Sanphams.RemoveRange(sanpham);
                    db.SaveChanges();
                }
                var Size = db.Sizes.Where(x => x.ID_Loai == id).ToList();
                if(Size != null && Size.Count() > 0)
                {
                    db.Sizes.RemoveRange(Size);
                    db.SaveChanges();
                }
                var Color = db.MauSacs.Where(x => x.ID_Loai == id).ToList();
                if(Color != null && Color.Count() > 0)
                {
                    db.MauSacs.RemoveRange(Color);
                    db.SaveChanges();
                }
                db.Loais.Remove(Loaisp);
                db.SaveChanges();
            }
            
            return RedirectToAction("Index");
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