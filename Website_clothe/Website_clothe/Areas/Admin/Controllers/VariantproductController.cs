using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;
using Website_clothe.Models;

namespace Website_clothe.Areas.Admin.Controllers
{
    public class VariantproductController : Controller
    {
        public Server_clothesEntities5 db = new Server_clothesEntities5();
        public ActionResult Index()
        {
            if (Session["AdminUsername"] == null)
            {
                return View("/Areas/Admin/Views/AdminUser/Login.cshtml");
            }
            var model = db.Bienthesanphams.ToList();
            ViewBag.Sanpham = db.Sanphams.ToList();
            return View(model);
        }
        [HttpGet]
        public ActionResult Create()
        {
            if (Session["AdminUsername"] == null)
            {
                return View("/Areas/Admin/Views/AdminUser/Login.cshtml");
            }
            var  sanpham = db.Sanphams.ToList();
            var sizes = (from x in db.Sizes join y in db.Loais on x.ID_Loai equals y.ID_Loai select new
            {
                Masize = x.ID_Size,
                Tensize = x.TenSize + " - " + y.Tenloai,
            }).ToList();

            var maus = (from c in db.MauSacs join q in db.Loais on c.ID_Loai equals q.ID_Loai select new
            {
                Mamau = c.ID_Mau,
                Ten_mau = c.Tenmau + " - " + q.Tenloai
            }).ToList();

            var sanphamlist = sanpham.Select(c => new SelectListItem
            {
                Value = c.Masanpham.ToString(),
                Text = c.Tensanpham
            });

            ViewBag.sanpham = sanphamlist;

            var sizeslist = sizes.Select(c => new SelectListItem
            {
                Value = c.Masize.ToString(),
                Text = c.Tensize
            });
            ViewBag.sizes = sizeslist;

            var mauslist = maus.Select(c => new SelectListItem
            {
                Value = c.Mamau.ToString(),
                Text = c.Ten_mau
            });
            ViewBag.maus = mauslist;
            Bienthesanpham model = new Bienthesanpham();
            return View(model);
        }
        [HttpPost]
        public ActionResult Create(Bienthesanpham variant)
        {
            if(!ModelState.IsValid)
            {
                var bienthe = db.Bienthesanphams.Where(x => x.ID_Sanpham == variant.ID_Sanpham &&
                x.ID_Mau == variant.ID_Mau && x.SizeID == variant.SizeID);
                if(bienthe.Any())
                {
                    ModelState.AddModelError("", "Sản phẩm đã tồn tại trên hệ thống");
                }
                var sanpham = db.Sanphams.ToList();
                var sizes = (from x in db.Sizes
                             join y in db.Loais on x.ID_Loai equals y.ID_Loai
                             select new
                             {
                                 Masize = x.ID_Size,
                                 Tensize = x.TenSize + " - " + y.Tenloai,
                             }).ToList();

                var maus = (from c in db.MauSacs
                            join q in db.Loais on c.ID_Loai equals q.ID_Loai
                            select new
                            {
                                Mamau = c.ID_Mau,
                                Ten_mau = c.Tenmau + " - " + q.Tenloai
                            }).ToList();

                var sanphamlist = sanpham.Select(c => new SelectListItem
                {
                    Value = c.Masanpham.ToString(),
                    Text = c.Tensanpham
                });

                ViewBag.sanpham = sanphamlist;

                var sizeslist = sizes.Select(c => new SelectListItem
                {
                    Value = c.Masize.ToString(),
                    Text = c.Tensize
                });
                ViewBag.sizes = sizeslist;

                var mauslist = maus.Select(c => new SelectListItem
                {
                    Value = c.Mamau.ToString(),
                    Text = c.Ten_mau
                });
                ViewBag.maus = mauslist;
                return View(variant);
            }
            var data = new Bienthesanpham
            {
                ID_Sanpham = variant.ID_Sanpham,
                ID_Mau = variant.ID_Mau,
                SizeID = variant.SizeID,
                Soluongton = variant.Soluongton,
            };
            db.Bienthesanphams.Add(data);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        [HttpGet]
        public ActionResult Edit (int id)
        {
            var model = db.Bienthesanphams.Where(x => x.ID_Bienthesanpham == id).FirstOrDefault();
            var sanpham = db.Sanphams.ToList();
            var sizes = (from x in db.Sizes
                         join y in db.Loais on x.ID_Loai equals y.ID_Loai
                         select new
                         {
                             Masize = x.ID_Size,
                             Tensize = x.TenSize + " - " + y.Tenloai,
                         }).ToList();

            var maus = (from c in db.MauSacs
                        join q in db.Loais on c.ID_Loai equals q.ID_Loai
                        select new
                        {
                            Mamau = c.ID_Mau,
                            Ten_mau = c.Tenmau + " - " + q.Tenloai
                        }).ToList();

            var sanphamlist = sanpham.Select(c => new SelectListItem
            {
                Value = c.Masanpham.ToString(),
                Text = c.Tensanpham
            });

            ViewBag.sanpham = sanphamlist;

            var sizeslist = sizes.Select(c => new SelectListItem
            {
                Value = c.Masize.ToString(),
                Text = c.Tensize
            });
            ViewBag.sizes = sizeslist;

            var mauslist = maus.Select(c => new SelectListItem
            {
                Value = c.Mamau.ToString(),
                Text = c.Ten_mau
            });
            ViewBag.maus = mauslist;
            return View(model);
        }
        [HttpPost]
        public ActionResult Edit (Bienthesanpham variant)
        {
            if(!ModelState.IsValid)
            {
                var sanpham = db.Sanphams.ToList();
                var sizes = (from x in db.Sizes
                             join y in db.Loais on x.ID_Loai equals y.ID_Loai
                             select new
                             {
                                 Masize = x.ID_Size,
                                 Tensize = x.TenSize + " - " + y.Tenloai,
                             }).ToList();

                var maus = (from c in db.MauSacs
                            join q in db.Loais on c.ID_Loai equals q.ID_Loai
                            select new
                            {
                                Mamau = c.ID_Mau,
                                Ten_mau = c.Tenmau + " - " + q.Tenloai
                            }).ToList();

                var sanphamlist = sanpham.Select(c => new SelectListItem
                {
                    Value = c.Masanpham.ToString(),
                    Text = c.Tensanpham
                });

                ViewBag.sanpham = sanphamlist;

                var sizeslist = sizes.Select(c => new SelectListItem
                {
                    Value = c.Masize.ToString(),
                    Text = c.Tensize
                });
                ViewBag.sizes = sizeslist;

                var mauslist = maus.Select(c => new SelectListItem
                {
                    Value = c.Mamau.ToString(),
                    Text = c.Ten_mau
                });
                ViewBag.maus = mauslist;
                return View(variant);
            }
            else
            {
                var data = db.Bienthesanphams.FirstOrDefault(x => x.ID_Bienthesanpham == variant.ID_Bienthesanpham);
                data.ID_Mau = variant.ID_Mau;
                data.ID_Sanpham = variant.ID_Sanpham;
                data.SizeID = variant.SizeID; 
                data.Soluongton = variant.Soluongton;
                UpdateModel(data);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
        }
        [HttpGet]
        public ActionResult Delete (int id)
        {
            var data = db.Bienthesanphams.FirstOrDefault(x => x.ID_Bienthesanpham == id);
            if(data != null)
            {
                
                var chiTietDonHangs = db.Chitietdonhangs.Where(x => x.ID_Sanphambienthe == data.ID_Bienthesanpham).ToList();
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

                var chiTietPhieuNhaps = db.Chitietphieunhaps.Where(x => x.ID_Bienthesanpham == data.ID_Bienthesanpham).ToList();
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
                db.Bienthesanphams.Remove(data);
            }
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        [HttpGet]
        public ActionResult Detail (int id)
        {
            var model = db.Bienthesanphams.FirstOrDefault(x => x.ID_Bienthesanpham == id);
            ViewBag.Sanpham = db.Sanphams.FirstOrDefault(x => x.Masanpham == model.ID_Sanpham);
            ViewBag.Size = db.Sizes.FirstOrDefault(x => x.ID_Size == model.SizeID);
            ViewBag.Mau = db.MauSacs.FirstOrDefault(x => x.ID_Mau == model.ID_Mau);
            return View(model);
        }
    }
}