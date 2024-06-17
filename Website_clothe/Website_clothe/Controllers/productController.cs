using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Website_clothe.Models;

namespace Website_clothe.Controllers
{
    public class productController : Controller
    {
        private Server_clothesEntities5 db = new Server_clothesEntities5();


        
        public ActionResult Index(int id)
        {
            var model = db.Sanphams
                .Where(sp => sp.Chitietdanhmuc == id && db.Bienthesanphams.Any(bt => bt.ID_Sanpham == sp.Masanpham))
                .ToList();

            var firstImagesForEachProduct = db.Sanphams
                    .Where(x => x.Chitietdanhmuc == id)
                    .Select(p => new
                    {
                        ProductId = p.Masanpham,
                        FirstImageId = p.HinhanhSanphams.OrderBy(h => h.ID_Image).Select(h => h.ID_Image).FirstOrDefault(),
                        FirstImageUrl = p.HinhanhSanphams.OrderBy(h => h.ID_Image).Select(h => h.TenAnh).FirstOrDefault()
                    })
                    .ToList();
            var hinhanhSanphams = firstImagesForEachProduct
                                    .Select(p => new HinhanhSanpham
                                    {
                                        ID_Image = p.FirstImageId,
                                        TenAnh = p.FirstImageUrl,
                                        Masanpham = p.ProductId
                                    })
                                    .ToList();

            
            List<int> masanpham = model.Select(x => x.Masanpham).ToList();
            ViewBag.Hinhanh = hinhanhSanphams.Where(x => masanpham.Contains((int)x.Masanpham)).ToList();
            return View(model);
        }

        public ActionResult Detailproduct(int idsp)
        {
           
            List<MauSac> colorproduct = new List<MauSac>();
            var model = db.Sanphams.Where(x => x.Masanpham == idsp).FirstOrDefault();
            var item = db.Bienthesanphams.Where(x => x.ID_Sanpham == model.Masanpham).ToList();
            var sizetotal = db.Sizes.ToList();
            
            var colortotal = db.MauSacs.ToList();
            
            foreach(var query in item)
            {
                
                foreach(var query2 in colortotal)
                {
                    if(query2.ID_Mau == query.ID_Mau && query2.ID_Loai != null && !colorproduct.Any(c => c.Tenmau == query2.Tenmau))
                    {

                        var coloritem = new MauSac
                        {
                            ID_Mau = query2.ID_Mau,
                            Tenmau = query2.Tenmau,
                            ID_Loai= query2.ID_Loai,
                        };
                        colorproduct.Add(coloritem);
                    }
                }
            }
            ViewBag.Mausanpham = colorproduct;
           
            ViewBag.hinhsanpham = db.HinhanhSanphams.Where(x => model.Masanpham == x.Masanpham).ToList();
            return View(model);
        }
        public ActionResult Findproductwithbrand(int id)
        {
            var model = db.Sanphams.Where(x => x.Mathuonghieu == id && db.Bienthesanphams.Any(bt => bt.ID_Sanpham == x.Masanpham)).ToList();
            //Lấy hình ảnh sản phẩm
            var firstImagesForEachProduct = db.Sanphams
                    .Select(p => new
                    {
                        ProductId = p.Masanpham,
                        FirstImageId = p.HinhanhSanphams.OrderBy(h => h.ID_Image).Select(h => h.ID_Image).FirstOrDefault(),
                        FirstImageUrl = p.HinhanhSanphams.OrderBy(h => h.ID_Image).Select(h => h.TenAnh).FirstOrDefault()
                    })
                    .ToList();
            var hinhanhSanphams = firstImagesForEachProduct
                                    .Select(p => new HinhanhSanpham
                                    {
                                        ID_Image = p.FirstImageId,
                                        TenAnh = p.FirstImageUrl,
                                        Masanpham = p.ProductId
                                    })
                                    .ToList();
            List<int> masanpham = model.Select(x => x.Masanpham).ToList();
            ViewBag.Hinhanh = hinhanhSanphams.Where(x => masanpham.Contains((int)x.Masanpham)).ToList();
            return View("~/Views/product/Index.cshtml", model);
        }
        public ActionResult Findproductwithname(string names)
        {
            var model = db.Sanphams.Where(p => p.Tensanpham.Contains(names) && db.Bienthesanphams.Any(bt => bt.ID_Sanpham == p.Masanpham)).ToList();
            //Lấy hình ảnh sản phẩm
            var firstImagesForEachProduct = db.Sanphams
                    .Select(p => new
                    {
                        ProductId = p.Masanpham,
                        FirstImageId = p.HinhanhSanphams.OrderBy(h => h.ID_Image).Select(h => h.ID_Image).FirstOrDefault(),
                        FirstImageUrl = p.HinhanhSanphams.OrderBy(h => h.ID_Image).Select(h => h.TenAnh).FirstOrDefault()
                    })
                    .ToList();
            var hinhanhSanphams = firstImagesForEachProduct
                                    .Select(p => new HinhanhSanpham
                                    {
                                        ID_Image = p.FirstImageId,
                                        TenAnh = p.FirstImageUrl,
                                        Masanpham = p.ProductId
                                    })
                                    .ToList();
            List<int> masanpham = model.Select(x => x.Masanpham).ToList();
            ViewBag.Hinhanh = hinhanhSanphams.Where(x => masanpham.Contains((int)x.Masanpham)).ToList();
            return View("~/Views/product/Index.cshtml", model);
        }
        public ActionResult Findproductwithcatagory(string danhmuc)
        {
            int danhmuctong = int.Parse(danhmuc.Split('-')[0]);
            string ctdanhmuc = danhmuc.Split('-')[1];
            var item = db.Chitietdanhmucs.Where(x => x.Madanhmuc == danhmuctong && x.Tenchitietdanhmuc == ctdanhmuc).Select(x => x.ID_Chitietdanhmuc).FirstOrDefault();

            //var sanphamdetail = db.Bienthesanphams.Where(x => x.ID_Mau != null && x.SizeID != null && x.ID_Sanpham != null).Select(x => x.ID_Sanpham).ToList();
            var model = db.Sanphams.Where(x => x.Chitietdanhmuc == item && db.Bienthesanphams.Any(bt => bt.ID_Sanpham == x.Masanpham)).ToList();

            //Lấy hình ảnh sản phẩm
            var firstImagesForEachProduct = db.Sanphams
                    .Where(x => x.Chitietdanhmuc == item)
                    .Select(p => new
                    {
                        ProductId = p.Masanpham,
                        FirstImageId = p.HinhanhSanphams.OrderBy(h => h.ID_Image).Select(h => h.ID_Image).FirstOrDefault(),
                        FirstImageUrl = p.HinhanhSanphams.OrderBy(h => h.ID_Image).Select(h => h.TenAnh).FirstOrDefault()
                    })
                    .ToList();
            var hinhanhSanphams = firstImagesForEachProduct
                                    .Select(p => new HinhanhSanpham
                                    {
                                        ID_Image = p.FirstImageId,
                                        TenAnh = p.FirstImageUrl,
                                        Masanpham = p.ProductId
                                    })
                                    .ToList();
            List<int> masanpham = model.Select(x => x.Masanpham).ToList();
            ViewBag.Hinhanh = hinhanhSanphams.Where(x => masanpham.Contains((int)x.Masanpham)).ToList();




            return View("~/Views/product/Index.cshtml", model);
        }
        [HttpPost]
        public ActionResult FilterProducts(selectedConditions data)
        {
            List<Sanpham> model = new List<Sanpham>();
            var itemsub = db.Sanphams.Where(x => db.Bienthesanphams.Any(bt => bt.ID_Sanpham == x.Masanpham)).ToList();
            if (data.minPrice != data.maxPrice)
            {
                
                model = itemsub.Where(x => x.Giaban >= data.minPrice && x.Giaban <= data.maxPrice).ToList();
            }
            else
            {
                model = itemsub;
            }
            if (data.sizes != null)
            {
                string size = data.sizes;
                int maloai = int.Parse(size.Split('-')[0]);
                string masize = size.Split('-')[1];

                var sizeid = db.Sizes.Where(x => x.ID_Loai == maloai && x.TenSize == masize).Select(x => x.ID_Size).First();

                var bienthesp = db.Bienthesanphams.Where(x => x.SizeID == sizeid).Select(x => x.ID_Sanpham).ToList();

                model = model.Where(x => bienthesp.Contains(x.Masanpham)).ToList();
               
            }

            var firstImagesForEachProduct = db.Sanphams
                    .Select(p => new
                    {
                        ProductId = p.Masanpham,
                        FirstImageId = p.HinhanhSanphams.OrderBy(h => h.ID_Image).Select(h => h.ID_Image).FirstOrDefault(),
                        FirstImageUrl = p.HinhanhSanphams.OrderBy(h => h.ID_Image).Select(h => h.TenAnh).FirstOrDefault()
                    })
                    .ToList();
            var hinhanhSanphams = firstImagesForEachProduct
                                    .Select(p => new HinhanhSanpham
                                    {
                                        ID_Image = p.FirstImageId,
                                        TenAnh = p.FirstImageUrl,
                                        Masanpham = p.ProductId
                                    })
                                    .ToList();
            List<int> masanpham = model.Select(x => x.Masanpham).ToList();
            var hinh = hinhanhSanphams.Where(x => masanpham.Contains((int)x.Masanpham)).ToList();

            //tao danh sách sản phẩm có chứa hình
            List<Sanphamcondition> dt = new List<Sanphamcondition>();
            foreach(var item in model)
            {
                foreach(var item2 in hinh)
                {
                    if(item2.Masanpham == item.Masanpham)
                    {
                        var sanpham = new Sanphamcondition
                        {
                            masanpham = item.Masanpham,
                            tenhinhanh = item2.TenAnh,
                            tensanpham = item.Tensanpham,
                            giaban = int.Parse(item.Giaban + "")
                        };
                        dt.Add(sanpham);
                        break;
                    }
                }
            }
            var model2 = dt;
            return PartialView("Productwithcdtion", model2);
        }
        [HttpPost]
        public ActionResult checksize (int idsp,string mau)
        {
            var sanpham = db.Sanphams.Where(x => x.Masanpham == idsp).Select(x => x.ID_Loai).First();
            int idmau = db.MauSacs.Where(x => x.Tenmau == mau && x.ID_Loai == sanpham).Select(x => x.ID_Mau).First();
           

            var result = db.Bienthesanphams.Where(x => x.ID_Sanpham == idsp && x.ID_Mau == idmau).Join(db.Sizes,
                bienthe => bienthe.SizeID,
                size => size.ID_Size,
                (bienthe, size) => new sizeandsoluongton
                {
                    SizeID = (int)bienthe.SizeID,
                    Soluongton = bienthe.Soluongton,
                    TenSize = size.TenSize
                })
                .ToList();
            return Json(result,JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult checkcolor (int idsp,string size)
        {
            var sanpham = db.Sanphams.Where(x => x.Masanpham == idsp).Select(x => x.ID_Loai).First();
            int idsize = db.Sizes.Where(x => x.TenSize == size && x.ID_Loai == sanpham).Select(x => x.ID_Size).First();
            var bienthesanpham = db.Bienthesanphams.Where(x => x.ID_Sanpham == idsp && x.SizeID == idsize).Select(x => x.ID_Mau).ToList();
            var mau = db.MauSacs.Where(x => bienthesanpham.Contains(x.ID_Mau)).Select(x => x.Tenmau).ToList();
            return Json(mau, JsonRequestBehavior.AllowGet);
        }
    }
    public class selectedConditions
    {
        public string sizes { get; set; } = "";
        public int minPrice { get; set; }
        public int maxPrice { get; set; }
    
    }
    public class sizeandsoluongton
    {
        public int SizeID { get; set; }
        public int Soluongton { get; set; }
        public string TenSize { get; set; }
    }
}