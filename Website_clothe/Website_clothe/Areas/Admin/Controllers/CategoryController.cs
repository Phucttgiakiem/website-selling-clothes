using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Website_clothe.Models;
namespace Website_clothe.Areas.Admin.Controllers
{

    public class CategoryController : Controller
    {
       public Server_clothesEntities5 db = new Server_clothesEntities5();
        public ActionResult Index()
        {
            if (Session["AdminUsername"] == null)
            {
                return View("/Areas/Admin/Views/AdminUser/Login.cshtml");
            }
            List < Danhmucsanpham > ketqua = db.Danhmucsanphams.ToList();
            return View(ketqua);
        }
        public ActionResult Add()
        {
            if (Session["AdminUsername"] == null)
            {
                return View("Index", "AdminUser");
            }
            return View();
        }
        [HttpPost]
        public ActionResult Add(string tendanhmuc)
        {
            var query = db.Danhmucsanphams.Where(p => p.Tendanhmuc == tendanhmuc);
            if(query.Any())
            {
                ViewBag.error = "Tên danh mục đã tồn tại trên hệ thống";
                return View();
            }
            
                var kq = new Danhmucsanpham
                {
                    Tendanhmuc = tendanhmuc,
                    HinhAnh = ""
                };
                db.Danhmucsanphams.Add(kq);
                db.SaveChangesAsync();
            
            return RedirectToAction("Create","DetailCatagory");
        }
        [HttpGet]
        public ActionResult Edit(int id)
        {
            var kq = db.Danhmucsanphams.Where(p => p.Madanhmuc == id).FirstOrDefault();

            
            return View(kq);
        }
        [HttpPost]
        public ActionResult Edit(int id,string tendanhmuc)
        {
                var query = db.Danhmucsanphams.First(p => p.Madanhmuc == id);
                query.Tendanhmuc = tendanhmuc;
                query.HinhAnh = "";
                UpdateModel(query);
                db.SaveChangesAsync();
            
            return RedirectToAction("Index");
        }
        [HttpGet]
        public ActionResult Delete (int id)
        {
            var Danhmucsp = db.Danhmucsanphams.Where(x => x.Madanhmuc == id).FirstOrDefault();
            if(Danhmucsp != null)
            {
                var CtDanhmuc = db.Chitietdanhmucs.Where(x => x.Madanhmuc == Danhmucsp.Madanhmuc).ToList();
                if(CtDanhmuc != null)
                {
                    var CtDanhmucIDs = CtDanhmuc.Select(x => x.ID_Chitietdanhmuc).ToList();
                    var sanpham = db.Sanphams.Where(x => CtDanhmucIDs.Contains((int)x.Chitietdanhmuc)).ToList();
                    if(sanpham.Any())
                    {
                        var sanphamIDs = sanpham.Select(x => x.Masanpham).ToList();
                        var btsanpham = db.Bienthesanphams.Where(x => sanphamIDs.Contains((int)x.ID_Sanpham)).ToList();
                        if (btsanpham.Any())
                        {
                            var btsanphamIDs = btsanpham.Select(x => x.ID_Bienthesanpham).ToList();
                            var donmuact = db.Chitietdonhangs.Where(x => btsanphamIDs.Contains((int)x.ID_Sanphambienthe)).ToList();
                            if (donmuact.Any())
                            {
                                var donDatIDs = donmuact.Where(x => x.ID_Donhang.HasValue).Select(x => x.ID_Donhang.Value).ToList();
                                var dondat = db.Donmuahangs.Where(x => donDatIDs.Contains(x.ID_Donhang)).ToList();
                                db.Chitietdonhangs.RemoveRange(donmuact);
                                db.SaveChanges();
                                if (dondat.Any())
                                {
                                    db.Donmuahangs.RemoveRange(dondat);
                                    db.SaveChanges();
                                }
                                
                            }
                            var phieunhapct = db.Chitietphieunhaps.Where(x => btsanphamIDs.Contains((int)x.ID_Bienthesanpham)).ToList();
                            if (phieunhapct.Any())
                            {
                                var phieunhapIDs = phieunhapct.Where(x => x.ID_Phieunhaphang.HasValue).Select(x => x.ID_Phieunhaphang.Value).ToList();
                                var phieunhap = db.Phieunhaps.Where(x => phieunhapIDs.Contains(x.ID_Phieunhap)).ToList();
                                db.Chitietphieunhaps.RemoveRange(phieunhapct);
                                db.SaveChanges();
                                if (phieunhap.Any())
                                {
                                    db.Phieunhaps.RemoveRange(phieunhap);
                                    db.SaveChanges();
                                }
                            }
                            db.Bienthesanphams.RemoveRange(btsanpham);
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
                        db.Sanphams.RemoveRange(sanpham); 
                        db.SaveChanges();
                    }
                    db.Chitietdanhmucs.RemoveRange(CtDanhmuc);
                    db.SaveChanges();
                }
                db.Danhmucsanphams.Remove(Danhmucsp);
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