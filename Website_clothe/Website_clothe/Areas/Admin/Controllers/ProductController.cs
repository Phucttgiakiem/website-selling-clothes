using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;


using Website_clothe.Models;
namespace Website_clothe.Areas.Admin.Controllers
{
    public class ProductController : Controller
    {
        public Server_clothesEntities5 db = new Server_clothesEntities5();
        public ActionResult Index()
        {
            if (Session["AdminUsername"] == null)
            {
                return View("/Areas/Admin/Views/AdminUser/Login.cshtml");
            }
            var model = db.Sanphams.ToList();
            return View(model);
        }
        [HttpGet]
        public ActionResult Create()
        {
            if (Session["AdminUsername"] == null)
            {
                return View("/Areas/Admin/Views/AdminUser/Login.cshtml");
            }
            ViewBag.Loai = new List<Loai>(db.Loais.ToList());
            ViewBag.Brand = new List<Thuonghieu>(db.Thuonghieux.ToList());
            ViewBag.Nhacungcap = new List<Nhacungcap>(db.Nhacungcaps.ToList());
            var danhMucWithChiTiet = db.Danhmucsanphams.Include("Chitietdanhmucs").ToList();
            List<SelectListItem> items = new List<SelectListItem>();

            foreach (var danhMuc in danhMucWithChiTiet)
            {
                var group = new SelectListGroup { Name = danhMuc.Tendanhmuc };

                foreach (var chiTiet in danhMuc.Chitietdanhmucs)
                {
                    items.Add(new SelectListItem
                    {
                        Value = chiTiet.ID_Chitietdanhmuc.ToString(),
                        Text = chiTiet.Tenchitietdanhmuc,
                        Group = group
                    });
                }
            }
            ViewBag.Items = items;
            var model = new Sanpham();
            return View(model);
        }
        [HttpPost]
         public ActionResult Create(Sanpham sanpham, IEnumerable<HttpPostedFileBase> images) { 
            if(!ModelState.IsValid)
            {
                ViewBag.Loai = new List<Loai>(db.Loais.ToList());
                ViewBag.Brand = new List<Thuonghieu>(db.Thuonghieux.ToList());
                ViewBag.Nhacungcap = new List<Nhacungcap>(db.Nhacungcaps.ToList());
                var danhMucWithChiTiet = db.Danhmucsanphams.Include("Chitietdanhmucs").ToList();
                List<SelectListItem> items = new List<SelectListItem>();

                foreach (var danhMuc in danhMucWithChiTiet)
                {
                    var group = new SelectListGroup { Name = danhMuc.Tendanhmuc };

                    foreach (var chiTiet in danhMuc.Chitietdanhmucs)
                    {
                        items.Add(new SelectListItem
                        {
                            Value = chiTiet.ID_Chitietdanhmuc.ToString(),
                            Text = chiTiet.Tenchitietdanhmuc,
                            Group = group
                        });
                    }
                }
                ViewBag.Items = items;
                return View(sanpham);
            }
            else
            {
                var result = db.Sanphams.Where(x => x.Tensanpham == sanpham.Tensanpham 
                    && x.Chitietdanhmuc == sanpham.Chitietdanhmuc && x.Mathuonghieu == sanpham.Mathuonghieu && x.ID_Nhacungcap == sanpham.ID_Nhacungcap);
                if (result.Any())
                {
                    ModelState.AddModelError("", "Sản phẩm đã tồn tại trên hệ thống");
                    ViewBag.Loai = new List<Loai>(db.Loais.ToList());
                    ViewBag.Brand = new List<Thuonghieu>(db.Thuonghieux.ToList());
                    ViewBag.Nhacungcap = new List<Nhacungcap>(db.Nhacungcaps.ToList());
                    var danhMucWithChiTiet = db.Danhmucsanphams.Include("Chitietdanhmucs").ToList();
                    List<SelectListItem> items = new List<SelectListItem>();

                    foreach (var danhMuc in danhMucWithChiTiet)
                    {
                        var group = new SelectListGroup { Name = danhMuc.Tendanhmuc };

                        foreach (var chiTiet in danhMuc.Chitietdanhmucs)
                        {
                            items.Add(new SelectListItem
                            {
                                Value = chiTiet.ID_Chitietdanhmuc.ToString(),
                                Text = chiTiet.Tenchitietdanhmuc,
                                Group = group
                            });
                        }
                    }
                    ViewBag.Items = items;
                    return View(sanpham);
                }
                else
                {
                    var data = new Sanpham
                    {
                        Tensanpham = sanpham.Tensanpham,
                        ID_Loai = sanpham.ID_Loai,
                        Mathuonghieu = sanpham.Mathuonghieu,
                        ID_Nhacungcap = sanpham.ID_Nhacungcap,
                        Mota = sanpham.Mota,
                        Ngaytao = DateTime.Today,
                        Ngaycapnhat = DateTime.Today,
                        Giaban = sanpham.Giaban,
                        Gianhap = sanpham.Gianhap,
                        Chitietdanhmuc = sanpham.Chitietdanhmuc
                    };
                    db.Sanphams.Add(data);
                    db.SaveChanges();
                    if (!check_file_has_empty(images))
                    {
                        if (!check_image_array(images))
                        {
                            ModelState.AddModelError("", "Ảnh bạn chọn có file không phải là file ảnh");
                            ViewBag.Loai = new List<Loai>(db.Loais.ToList());
                            ViewBag.Brand = new List<Thuonghieu>(db.Thuonghieux.ToList());
                            ViewBag.Nhacungcap = new List<Nhacungcap>(db.Nhacungcaps.ToList());
                            var danhMucWithChiTiet = db.Danhmucsanphams.Include("Chitietdanhmucs").ToList();
                            List<SelectListItem> items = new List<SelectListItem>();

                            foreach (var danhMuc in danhMucWithChiTiet)
                            {
                                var group = new SelectListGroup { Name = danhMuc.Tendanhmuc };

                                foreach (var chiTiet in danhMuc.Chitietdanhmucs)
                                {
                                    items.Add(new SelectListItem
                                    {
                                        Value = chiTiet.ID_Chitietdanhmuc.ToString(),
                                        Text = chiTiet.Tenchitietdanhmuc,
                                        Group = group
                                    });
                                }
                            }
                            ViewBag.Items = items;
                            return View(sanpham);
                        }
                        else
                        {
                            var kq = db.Sanphams.Where(x => x.Tensanpham == sanpham.Tensanpham).Select(x => x.Masanpham).FirstOrDefault();
                            foreach (var image in images)
                            {

                                string filename = Path.GetFileName(image.FileName);
                                string extension = Path.GetExtension(filename);
                                string uniqueFileName = Path.GetFileNameWithoutExtension(filename) + "_" + Guid.NewGuid().ToString() + extension;
                                string path = Path.Combine(Server.MapPath("~/Content/img"), uniqueFileName);
                                image.SaveAs(path);

                                var imgdb = new HinhanhSanpham
                                {
                                    TenAnh = uniqueFileName,
                                    Masanpham = kq
                                };
                                db.HinhanhSanphams.Add(imgdb);
                            }
                            db.SaveChanges();
                        }
                    }
                    
                }
            }
            return RedirectToAction("Create","Variantproduct");
        }
        [HttpGet]
        public ActionResult Edit (int id)
        {
            var model = db.Sanphams.Where(x => x.Masanpham == id).FirstOrDefault();
            ViewBag.Loai = new List<Loai>(db.Loais.ToList());
            ViewBag.Brand = new List<Thuonghieu>(db.Thuonghieux.ToList());
            ViewBag.Nhacungcap = new List<Nhacungcap>(db.Nhacungcaps.ToList());
       //     ViewBag.Hinhanh = db.HinhanhSanphams.Where(x => x.Masanpham == id).ToList();
            var danhMucWithChiTiet = db.Danhmucsanphams.Include("Chitietdanhmucs").ToList();
            List<SelectListItem> items = new List<SelectListItem>();

            foreach (var danhMuc in danhMucWithChiTiet)
            {
                var group = new SelectListGroup { Name = danhMuc.Tendanhmuc };

                foreach (var chiTiet in danhMuc.Chitietdanhmucs)
                {
                    items.Add(new SelectListItem
                    {
                        Value = chiTiet.ID_Chitietdanhmuc.ToString(),
                        Text = chiTiet.Tenchitietdanhmuc,
                        Group = group
                    });
                }
            }
            ViewBag.Items = items;
            return View(model);
        }
        [HttpPost]
        public ActionResult Edit(Sanpham sanpham)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Loai = new List<Loai>(db.Loais.ToList());
                ViewBag.Brand = new List<Thuonghieu>(db.Thuonghieux.ToList());
                ViewBag.Nhacungcap = new List<Nhacungcap>(db.Nhacungcaps.ToList());
                //ViewBag.Hinhanh = db.HinhanhSanphams.Where(x => x.Masanpham == sanpham.Masanpham).ToList();
                var danhMucWithChiTiet = db.Danhmucsanphams.Include("Chitietdanhmucs").ToList();
                List<SelectListItem> items = new List<SelectListItem>();

                foreach (var danhMuc in danhMucWithChiTiet)
                {
                    var group = new SelectListGroup { Name = danhMuc.Tendanhmuc };

                    foreach (var chiTiet in danhMuc.Chitietdanhmucs)
                    {
                        items.Add(new SelectListItem
                        {
                            Value = chiTiet.ID_Chitietdanhmuc.ToString(),
                            Text = chiTiet.Tenchitietdanhmuc,
                            Group = group
                        });
                    }
                }
                ViewBag.Items = items;
                return View(sanpham);
            }
            else
            {
                var result = db.Sanphams.FirstOrDefault(x => x.Masanpham == sanpham.Masanpham);
                result.Tensanpham = sanpham.Tensanpham;
                result.ID_Loai = sanpham.ID_Loai;
                result.Mathuonghieu = sanpham.Mathuonghieu;
                result.ID_Nhacungcap = sanpham.ID_Nhacungcap;
                result.Mota = sanpham.Mota;
                result.Ngaycapnhat = DateTime.Today;
                result.Giaban = sanpham.Giaban;
                result.Gianhap = sanpham.Gianhap;
                result.Chitietdanhmuc = sanpham.Chitietdanhmuc;
                UpdateModel(result);

               
                db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
        }
        public ActionResult Detail (int id)
        {
            var model = db.Sanphams.Where(x => x.Masanpham == id).FirstOrDefault();
            ViewBag.Hinhsp = db.HinhanhSanphams.Where(x => x.Masanpham == id).Select(x=>x.TenAnh).ToList();
            ViewBag.Danhmuc = db.Chitietdanhmucs.Where(x => x.ID_Chitietdanhmuc == model.Chitietdanhmuc).Select(x => x.Tenchitietdanhmuc).FirstOrDefault();
            ViewBag.Thuonghieu = db.Thuonghieux.Where(x => x.Mathuonghieu == model.Mathuonghieu).Select(x => x.Tenthuonghieu).FirstOrDefault();
            ViewBag.Nhacungcap = db.Nhacungcaps.Where(x => x.Manhacungcap == model.ID_Nhacungcap).Select(x => x.Tennhacungcap).FirstOrDefault();
            ViewBag.Loai = db.Loais.Where(x => x.ID_Loai == model.ID_Loai).Select(x => x.Tenloai).FirstOrDefault();
            return View(model);
        }
        public ActionResult Delete (int id)
        {
            var result = db.HinhanhSanphams.Where(x => x.Masanpham == id).ToList();
            if(result != null)
            {
                foreach(var item in result)
                {
                    deletefile_image(item.TenAnh);
                    
                }
                db.HinhanhSanphams.RemoveRange(result);
                db.SaveChanges();
            }
            
            var result2 = db.Bienthesanphams.Where(x => x.ID_Sanpham == id).ToList();
            var result3 = db.Sanphams.Where(x => x.Masanpham == id).First();
            List <int> idbienthe = result2.Select(x => x.ID_Bienthesanpham).ToList();
            var dondatchitiet = db.Chitietdonhangs.Where(x => idbienthe.Contains((int)x.ID_Sanphambienthe)).ToList();
            
            var donnhapchitiet = db.Chitietphieunhaps.Where(x => idbienthe.Contains((int)x.ID_Bienthesanpham)).ToList();
            

            if (result2 != null)
            {
                db.Bienthesanphams.RemoveRange(result2);
                db.SaveChanges();
            }
            if(result3 != null)
            {
                db.Sanphams.Remove(result3);
                db.SaveChanges();
            }
            if(dondatchitiet != null && dondatchitiet.Count() > 0)
            {
                List<int> madonhang = new List<int>((IEnumerable<int>)dondatchitiet.Select(x => x.ID_Donhang));
                if(madonhang.Count() > 0)
                {
                    var dondat = db.Donmuahangs.Where(x => madonhang.Contains(x.ID_Donhang));
                    if (dondat != null)
                    {

                        db.Donmuahangs.RemoveRange(dondat);
                        db.SaveChanges();
                    }
                }
                
                db.Chitietdonhangs.RemoveRange(dondatchitiet);
                db.SaveChanges();
            }
            
            if(donnhapchitiet != null&& donnhapchitiet.Count() > 0)
            {
                List<int> madonnhap = new List<int>((IEnumerable<int>)donnhapchitiet.Select(x => x.ID_Phieunhaphang));
                if(madonnhap.Count() > 0)
                {
                    var donnhap = db.Phieunhaps.Where(x => madonnhap.Contains(x.ID_Phieunhap));
                    if (donnhap != null)
                    {
                        db.Phieunhaps.RemoveRange(donnhap);
                        db.SaveChanges();
                    }
                }
                
                db.Chitietphieunhaps.RemoveRange(donnhapchitiet);
                db.SaveChanges();
            }
            


            
            return RedirectToAction("Index");
        }
        public bool check_file_has_empty (IEnumerable<HttpPostedFileBase> images)
        {
            foreach(var item in images)
            {
                if (item == null) return true;
            }
            return false;
        }
        public void deletefile_image (string name_image)
        {
            try
            {
                string folderPath = Server.MapPath("~/Content/img/");
                string filePath = Path.Combine(folderPath, name_image);
                if(System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }
            catch (Exception e) { 
                
            }
            return;
        }
        private bool check_image_array (IEnumerable<HttpPostedFileBase> images)
        {
            foreach(var item in images)
            {
                    string filename = Path.GetFileName(item.FileName);
                    string extension = Path.GetExtension(filename);
                    if (IsImageFile(extension) == "") return false;
            }
            return true;
        }
        private string IsImageFile(string file_extension)
        {
            switch (file_extension)
            {
                case ".jpeg": 
                case ".jpg":
                case ".png":
                case ".gif":
                case ".bmp":
                case ".tiff":
                case ".webp":
                    return file_extension;
            }
            return "";
        }
        
    }
}