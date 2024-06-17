
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Web;
using System.Web.Mvc;
using System.Web.Razor.Parser.SyntaxTree;
using System.Web.UI.WebControls;
using System.Web.WebPages.Html;
using Website_clothe.Models;
using SelectPdf;
using System.Drawing.Printing;
using System.Data;

namespace Website_clothe.Areas.Admin.Controllers
{
    public class ImportproductController : BaseController
    {
        Server_clothesEntities5 db = new Server_clothesEntities5();
        public ActionResult Index()
        {
            if (Session["AdminUsername"] == null)
            {
                return View("/Areas/Admin/Views/AdminUser/Login.cshtml");
            }
            var model = db.Phieunhaps.ToList();
            return View(model);
        }
        public ActionResult Create()
        {
            if (Session["AdminUsername"] == null)
            {
                return View("/Areas/Admin/Views/AdminUser/Login.cshtml");
            }
            var nhacungcap = db.Nhacungcaps.ToList();
            ViewBag.Nhacungcap = nhacungcap.Select(c => new System.Web.Mvc.SelectListItem
            {
                Value = c.Manhacungcap.ToString(),
                Text = c.Tennhacungcap
            });
            
            var sanpham = (from c in db.Sanphams join q in db.Bienthesanphams on c.Masanpham equals q.ID_Sanpham select new sanphamFsanphambienthe
            {

                Masan_phambienthe = q.ID_Bienthesanpham,
                Ten_sanpham = c.Tensanpham
            }).ToList();

            ViewBag.sanphams = sanpham;


            Nhacungcap model = new Nhacungcap();
            return View(model);
        }
        [HttpPost]
        public ActionResult getinfoprovider (int idpro)
        {
            var nhacungcap = db.Nhacungcaps.FirstOrDefault(x => x.Manhacungcap == idpro);
            return Json(new
            {
                manhacungcap = nhacungcap.Manhacungcap,
                tennhacungcap = nhacungcap.Tennhacungcap,
                diachi = nhacungcap.Diachi,
                soDT = nhacungcap.SoDT

            });
        }
        [HttpPost]
        public ActionResult getsizeandcolor (int idpro)
        {
            var bienthe = db.Bienthesanphams.FirstOrDefault(x => x.ID_Bienthesanpham == idpro);
            var price = db.Sanphams.Where(c => c.Masanpham == bienthe.ID_Sanpham).Select(c => c.Gianhap).First();
            var size = (from c in db.Sizes
                       where c.ID_Size == bienthe.SizeID
                       join y in db.Loais on c.ID_Loai equals y.ID_Loai
                       select new
                       {
                           masize = c.ID_Size,
                           tensize = y.Tenloai + " - " + c.TenSize
                       }).First();
            var color = (from c in db.MauSacs
                        where  c.ID_Mau == bienthe.ID_Mau 
                        join d in db.Loais on c.ID_Loai equals d.ID_Loai
                        select new
                        {
                            Ma_mau = c.ID_Mau,
                            Tenmau = c.Tenmau + " - " + d.Tenloai
                        }).First();
            
            var result = new {size = size,color = color,price = price};
            return Json(result);
        }
        [HttpPost]
        public ActionResult Create(ImportDataModel data)
        {
            var idNhacungcap = data.idnhacungcap;
            var Tongtien = data.totalbill;
            var listctpn = data.detailbill;
            var textnote = data.note;
            var time = data.datatime;
            var databillimport = new Phieunhap
            {
                ID_Nhacungcap = idNhacungcap,
                NguoiLapPhieu = "Admin",
                Tongtien = Tongtien,
            };
            if(textnote != null)
            {
                databillimport.GhiChu = textnote;
            }
            if(time != null)
            {
                databillimport.Ngaytao = time;
            }
            else
            {
                databillimport.Ngaytao = DateTime.Now;
            }
            db.Phieunhaps.Add(databillimport);
            db.SaveChanges();

            foreach (var item in listctpn)
            {
                var dtbill = new Chitietphieunhap
                {
                    ID_Bienthesanpham = item.idsp,
                    Soluongnhap = item.quality,
                    Thanhtiennhap = item.quality * item.prices,
                    ID_Phieunhaphang = databillimport.ID_Phieunhap
                };
                db.Chitietphieunhaps.Add(dtbill);
                var bienthesp = db.Bienthesanphams.Where(x => x.ID_Bienthesanpham == item.idsp).FirstOrDefault();
                bienthesp.Soluongton = bienthesp.Soluongton + item.quality;
                UpdateModel(bienthesp);
            }
            db.SaveChanges();
            return Json(new { redirectTo = Url.Action("Index", "Importproduct") });
        }
        public ActionResult Detail (int id)
        {
            var data = db.Phieunhaps.FirstOrDefault(x => x.ID_Phieunhap == id);
            ViewBag.Nhacungcap = db.Nhacungcaps.FirstOrDefault(x => x.Manhacungcap == data.ID_Nhacungcap);
            
            ViewBag.Phieunhap = data;
            
            var data2 = (from x in db.Phieunhaps
                        where x.ID_Phieunhap == id
                        join y in db.Chitietphieunhaps on x.ID_Phieunhap equals y.ID_Phieunhaphang
                        join z in db.Bienthesanphams on y.ID_Bienthesanpham equals z.ID_Bienthesanpham
                        join c in db.Sanphams on z.ID_Sanpham equals c.Masanpham
                        join a in db.Sizes on z.SizeID equals a.ID_Size
                        join b in db.MauSacs on z.ID_Mau equals b.ID_Mau
                        select new
                        {
                            idsp = z.ID_Sanpham,
                            namesp = c.Tensanpham,
                            size = a.TenSize,
                            color = b.Tenmau,
                            quality = y.Soluongnhap,
                            prices = c.Gianhap
                        }).ToList();
            List<detailbillimp> bill = new List<detailbillimp>();
            foreach (var item in data2)
            {
                var dt = new detailbillimp
                {
                    idsp = (int)item.idsp,
                    namepd = item.namesp,
                    sizes = item.size,
                    color = item.color,
                    quality = item.quality,
                    prices = (int)item.prices,
                };
                bill.Add(dt);
            }
            var model = bill;
            return View(model);
        }
        public ActionResult Delete (int id)
        {
            var varianbill = db.Chitietphieunhaps.Where(x => x.ID_Phieunhaphang== id).ToList();
            db.Chitietphieunhaps.RemoveRange(varianbill);
            var importbill = db.Phieunhaps.FirstOrDefault(x => x.ID_Phieunhap == id);
            db.Phieunhaps.Remove(importbill);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        [HttpGet]
        public ActionResult Edit (int id)
        {
            var data = db.Phieunhaps.FirstOrDefault(x => x.ID_Phieunhap == id);
            var nhacungcap = db.Nhacungcaps.ToList();
            ViewBag.Nhacungcap = nhacungcap.Select(c => new System.Web.Mvc.SelectListItem
            {
                Value = c.Manhacungcap.ToString(),
                Text = c.Tennhacungcap
            });
            var chitietphieu = db.Chitietphieunhaps.Where(x => x.ID_Phieunhaphang == id);
            var bienthe = chitietphieu.Select(x => x.ID_Bienthesanpham).ToList();
            ViewBag.Phieunhap = data;
            List<detailbillimp> bill = new List<detailbillimp>();

            foreach (var item in bienthe)
            {
                var chitietvarian = db.Bienthesanphams.FirstOrDefault(x => x.ID_Bienthesanpham == item);
                var Color = db.MauSacs.FirstOrDefault(x => x.ID_Mau == chitietvarian.ID_Mau);
                var Sizes = db.Sizes.FirstOrDefault(x => x.ID_Size == chitietvarian.SizeID);
                var sanpham = db.Sanphams.FirstOrDefault(x => x.Masanpham == chitietvarian.ID_Sanpham);
                //var type = db.Loais.FirstOrDefault(x => x.ID_Loai == sanpham.ID_Loai);
                var dt = new detailbillimp
                {
                    idsp = (int)chitietvarian.ID_Sanpham,
                    namepd = sanpham.Tensanpham.ToString(),
                    sizes = Sizes.TenSize,
                    color = Color.Tenmau,
                    quality = chitietvarian.Soluongton,
                    prices = (int)sanpham.Gianhap,
                };
                bill.Add(dt);
            }
            ViewBag.Bill = bill;
            Nhacungcap model = db.Nhacungcaps.FirstOrDefault(x => x.Manhacungcap == data.ID_Nhacungcap);
            return View(model);
        }
        [HttpPost]
        public ActionResult Edit(int provide,string note ,string Date,int code )
        {
            var data = db.Phieunhaps.FirstOrDefault(x => x.ID_Phieunhap == code);
            data.ID_Nhacungcap = provide;
            data.Ngaytao = DateTime.Parse(Date);
            data.GhiChu = note;
            UpdateModel(data);
            db.SaveChanges();
            return Json(new { redirectTo = Url.Action("Index", "Importproduct") });
        }
        public ActionResult Generatepdf(int idbill)
        {
            var contentorder = (from x in db.Phieunhaps
                               where x.ID_Phieunhap == idbill
                               join y in db.Nhacungcaps on x.ID_Nhacungcap equals y.Manhacungcap
                               select new
                               {
                                   ID_Phieunhap = idbill,
                                   Nhacungcap = y.Tennhacungcap,
                                   diachi = y.Diachi,
                                   soDT = y.SoDT,
                                   Nguoilapphieu = x.NguoiLapPhieu,
                                   tongtien = x.Tongtien,
                                   Ghichu = x.GhiChu,
                                   ngaytao = x.Ngaytao
                                   
                               }).First();
            var detailorder = (from x in db.Chitietphieunhaps
                              where x.ID_Phieunhaphang == idbill
                              join y in db.Bienthesanphams on x.ID_Bienthesanpham equals y.ID_Bienthesanpham
                              join z in db.Sanphams on y.ID_Sanpham equals z.Masanpham
                              join a in db.Sizes on y.SizeID equals a.ID_Size
                              join b in db.MauSacs on y.ID_Mau equals b.ID_Mau
                              select new
                              {
                                  Mabienthe = y.ID_Bienthesanpham,
                                  tensanpham = z.Tensanpham,
                                  size = a.TenSize,
                                  Mau = b.Tenmau,
                                  Soluong = x.Soluongnhap,
                                  gianhap = z.Gianhap,
                                  Thanhtiennhap = x.Soluongnhap * z.Gianhap
                              }).ToList();

            billofimport billofimport_ = new billofimport();
            billofimport_.ID_phieunhap = contentorder.ID_Phieunhap;
            billofimport_.tennhacungcap = contentorder.Nhacungcap;
            billofimport_.ngaynhap = contentorder.ngaytao;
            billofimport_.diachi = contentorder.diachi;
            billofimport_.sodt = contentorder.soDT;
            billofimport_.tongtiennhap = (int) contentorder.tongtien;
            billofimport_.nguoilapphieu = contentorder.Nguoilapphieu;
            billofimport_.ghichu = contentorder.Ghichu;
            foreach(var item in detailorder)
            {
                var query = new detailbillimp
                {
                    idsp = item.Mabienthe,
                    namepd = item.tensanpham,
                    sizes = item.size,
                    color = item.Mau,
                    quality = item.Soluong,
                    prices = (int)item.gianhap,
                };
                billofimport_.ctbill.Add(query);
            }




            // instantiate a html to pdf converter object
            HtmlToPdf converter = new HtmlToPdf();

            // set converter options
            converter.Options.PdfPageSize = PdfPageSize.A4;
            converter.Options.PdfPageOrientation = PdfPageOrientation.Portrait;
            converter.Options.MarginLeft = 10;
            converter.Options.MarginRight = 10;
            converter.Options.MarginTop = 20;
            converter.Options.MarginBottom = 20;

            var htmlPdf = base.RenderPartialToString("~/Areas/Admin/Views/Importproduct/PartialViewPdfResult.cshtml", billofimport_, ControllerContext);


            // create a new pdf document converting an html string
            PdfDocument doc = converter.ConvertHtmlString(htmlPdf);
            string fileName = string.Format("{0}.pdf", DateTime.Now.Ticks);
            string pathFile = string.Format("{0}/{1}", Server.MapPath("~/Resource/Pdf"), fileName);
            doc.Save(pathFile);

            // save pdf document
            

            // close pdf document
            doc.Close();

            return Json(fileName,JsonRequestBehavior.AllowGet);
        }
        public ActionResult PartialViewPdfResult()
        {
            var model = new billofimport();
            return PartialView(model);
        }
    }
    
    public class sanphamFsanphambienthe
    {
        public int Masan_phambienthe { get; set; }
        public string Ten_sanpham { get; set; }
    }
    
    public class ImportDataModel
    {
        public int idnhacungcap { get; set; }
        public int totalbill { get; set; }
        public List<detailbillimp> detailbill { get; set; }
        public string note { get; set; }
        public DateTime datatime { get; set; }
    }
}