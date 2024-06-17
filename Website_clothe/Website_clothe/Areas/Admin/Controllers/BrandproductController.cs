using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Website_clothe.Models;

namespace Website_clothe.Areas.Admin.Controllers
{
    public class BrandproductController : Controller
    {
        // GET: Admin/Brandproduct
        public Server_clothesEntities5 db = new Server_clothesEntities5();
        public ActionResult Index()
        {
            if (Session["AdminUsername"] == null)
            {
                return View("/Areas/Admin/Views/AdminUser/Login.cshtml");
            }
            List<Thuonghieu> query = db.Thuonghieux.ToList();
            return View(query);
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
        public ActionResult Create(string tenthuonghieu)
        {
            var query = db.Thuonghieux.Where(x => x.Tenthuonghieu== tenthuonghieu);
            if (query.Any())
            {
                ViewBag.error = "Tên thương hiệu đã tồn tại";
                return View();
            }
            var data = new Thuonghieu
            {
                Tenthuonghieu = tenthuonghieu
            };
            db.Thuonghieux.Add(data);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        [HttpGet]
        public ActionResult Edit(int id) {
            var model = db.Thuonghieux.Where(x => x.Mathuonghieu == id).FirstOrDefault();
            return View(model);
        }
        [HttpPost]
        public ActionResult Edit(string tenthuonghieu,int mathuonghieu)
        {
            var query = db.Thuonghieux.First(x => x.Mathuonghieu == mathuonghieu);
            query.Tenthuonghieu = tenthuonghieu;
            UpdateModel(query);
            db.SaveChangesAsync();
            return RedirectToAction("Index"); 
        }
        [HttpGet]
        public ActionResult Delete(int id)
        {
            var brand = db.Thuonghieux.FirstOrDefault(x => x.Mathuonghieu == id);
            if (brand != null)
            {
                var sanpham = db.Sanphams.Where(x => x.Mathuonghieu == brand.Mathuonghieu).ToList();
                if (sanpham.Any())
                {
                    var sanphamIDs = sanpham.Select(x => x.Masanpham).ToList();
                    var bienthesp = db.Bienthesanphams.Where(x => sanphamIDs.Contains((int)x.ID_Sanpham)).ToList();
                    if(bienthesp.Any())
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
                        if(phieunhapct.Any())
                        {
                            var phieunhapIDs = phieunhapct.Where(x => x.ID_Phieunhaphang.HasValue).Select(x => x.ID_Phieunhaphang.Value).ToList();
                            var phieunhap = db.Phieunhaps.Where(x => phieunhapIDs.Contains(x.ID_Phieunhap)).ToList();
                            if(phieunhap.Any())
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
                    db.Sanphams.RemoveRange(sanpham); 
                    db.SaveChanges();
                }
                db.Thuonghieux.Remove(brand);
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