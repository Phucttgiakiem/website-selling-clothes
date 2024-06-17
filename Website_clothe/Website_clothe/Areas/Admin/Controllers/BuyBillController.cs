using SelectPdf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Website_clothe.Models;

namespace Website_clothe.Areas.Admin.Controllers
{
    public class BuyBillController : BaseController
    {
        Server_clothesEntities5 db = new Server_clothesEntities5();
        public ActionResult Index()
        {
            if (Session["AdminUsername"] == null)
            {
                return View("/Areas/Admin/Views/AdminUser/Login.cshtml");
            }
            var model = db.Donmuahangs.ToList();
            return View(model);
        }
        public ActionResult Detail(int id)
        {
                var data = db.Chitietdonhangs.Where(x => x.ID_Donhang == id).ToList();
                var model = db.Donmuahangs.FirstOrDefault(x => x.ID_Donhang == id);
            
            

                //lấy ra các biến thể sản phẩm mà mã của nó nằm trong chi tiết đơn hàng

                var idSanPhamBienthe = data.Select(ct => ct.ID_Sanphambienthe).ToList();
                var item2 = db.Bienthesanphams.Where(x => idSanPhamBienthe.Contains(x.ID_Bienthesanpham)).ToList();

                //lấy ra size mà mã nằm trong biến thể

                var idSizebienthe = item2.Select(bt => bt.SizeID).ToList();
                var item3 = db.Sizes.Where(x => idSizebienthe.Contains(x.ID_Size)).ToList();

                //lấy ra màu mà mã nằm trong biến thể

                var idMaubienthe = item2.Select(bt => bt.ID_Mau).ToList();
                var item4 = db.MauSacs.Where(x => idMaubienthe.Contains(x.ID_Mau)).ToList();

                //lấy ra các sản phẩm nằm trong biến thể sản phẩm
                var idsanphambienthe = item2.Select(bt => bt.ID_Sanpham).ToList();
                var item5 = db.Sanphams.Where(x => idsanphambienthe.Contains(x.Masanpham)).ToList();
                List<detailshoppingbill> bill = new List<detailshoppingbill>();
                int tongdon = 0;
                foreach (var i_tem in data)
                {
                    detailshoppingbill query = new detailshoppingbill
                    {
                        madonhang = (int)i_tem.ID_Donhang,
                        masanpham = (int)i_tem.ID_Sanphambienthe,
                        soluong = i_tem.Soluong,
                    };
                    int sizeid = 0, colorid = 0, sanphamid = 0;
                    foreach (var i_tem2 in item2)
                    {
                        if (i_tem2.ID_Bienthesanpham == i_tem.ID_Sanphambienthe)
                        {
                            sizeid = (int)i_tem2.SizeID;
                            colorid = (int)i_tem2.ID_Mau;
                            sanphamid = (int)i_tem2.ID_Sanpham;

                            break;
                        }
                    }
                    foreach (var i_tem3 in item3)
                    {
                        if (i_tem3.ID_Size == sizeid)
                        {
                            query.size = i_tem3.TenSize;
                            break;
                        }
                    }
                    foreach (var i_tem4 in item4)
                    {
                        if (i_tem4.ID_Mau == colorid)
                        {
                            query.mausac = i_tem4.Tenmau; break;
                        }
                    }
                    foreach (var i_tem5 in item5)
                    {
                        if (i_tem5.Masanpham == sanphamid)
                        {
                            query.tensanpham = i_tem5.Tensanpham;
                            query.dongia = (int)i_tem5.Giaban;
                            query.thanhtien = query.dongia * query.soluong;
                            tongdon += query.thanhtien;
                            break;

                        }
                    }
                    bill.Add(query);
                }
            
            ViewBag.Nguoimua = db.Nguoidungs.Where(x => x.ID_Nguoidung == model.ID_Nguoidung).FirstOrDefault();
            ViewBag.Trangthai = db.TrangThaiDHs.ToList();
            ViewBag.Bill = bill;
            ViewBag.tongdon = tongdon;
            return View(model);
        }
        [HttpPost]
        public ActionResult Changeofstatusbill(int id, int valueofbill)
        {
            var data = db.Donmuahangs.FirstOrDefault(x => x.ID_Donhang == id);
            var query = db.Chitietdonhangs.Where(x => x.ID_Donhang == id);
            if (valueofbill == 2002)
            {
                
                if (query.Any())
                {
                    List<int?> listNullableInts = query.Select(x => x.ID_Sanphambienthe).ToList();
                    List<int> masp = listNullableInts
                            .Where(x => x.HasValue) // Lọc ra các giá trị không null
                            .Select(x => x.Value)   // Lấy giá trị không null
                            .ToList();
                    var result = db.Bienthesanphams.Where(x => masp.Contains(x.ID_Bienthesanpham)).ToList();
                    foreach(var item in result)
                    {
                        foreach(var item2 in query)
                        {
                            if(item2.ID_Sanphambienthe == item.ID_Bienthesanpham)
                            {
                                item.Soluongton -= item2.Soluong;
                                
                                
                                break;
                            }
                        }
                    }
                }
            }
            else
            {
                if (query.Any())
                {
                    List<int?> listNullableInts = query.Select(x => x.ID_Sanphambienthe).ToList();
                    List<int> masp = listNullableInts
                            .Where(x => x.HasValue) // Lọc ra các giá trị không null
                            .Select(x => x.Value)   // Lấy giá trị không null
                            .ToList();
                    var result = db.Bienthesanphams.Where(x => masp.Contains(x.ID_Bienthesanpham)).ToList();
                    foreach (var item in result)
                    {
                        foreach (var item2 in query)
                        {
                            if (item2.ID_Sanphambienthe == item.ID_Bienthesanpham)
                            {
                                item.Soluongton += item2.Soluong;
                                
                                break;
                            }
                        }
                    }
                }
            }
            data.ID_Trangthai = valueofbill;
            UpdateModel(data);
            db.SaveChanges();
            return Json(new { success = true });
        }
        
        public ActionResult DeleteBill (int id)
        {
            var data = db.Donmuahangs.FirstOrDefault(x => x.ID_Donhang == id);
            var detaildata = db.Chitietdonhangs.Where(x => x.ID_Donhang == data.ID_Donhang).ToList();
            if (detaildata != null)
            {
                db.Chitietdonhangs.RemoveRange(detaildata);
            }
            db.Donmuahangs.Remove(data);
            db.SaveChanges();

            return RedirectToAction("Index");
        }
        public ActionResult Generatepdf(int idbill)
        {
            var order = (from x in db.Donmuahangs
                         where x.ID_Donhang == idbill
                         join y in db.Nguoidungs on x.ID_Nguoidung equals y.ID_Nguoidung
                         select new
                         {
                             madonhang = x.ID_Donhang,
                             ngaytao = x.Ngaytao,
                             Tennguoidung = x.Tennguoidung,
                             sodt = y.SoDT,
                             Diachi = x.DiaChi,
                             Tongdon = x.Tongdonhang
                         }).First();

            var detailorder = (from x in db.Chitietdonhangs
                               where x.ID_Donhang == idbill
                               join y in db.Bienthesanphams on x.ID_Sanphambienthe equals y.ID_Bienthesanpham
                               join z in db.Sanphams on y.ID_Sanpham equals z.Masanpham
                               join a in db.Sizes on y.SizeID equals a.ID_Size
                               join b in db.MauSacs on y.ID_Mau equals b.ID_Mau
                               select new
                               {
                                   mabienthe = y.ID_Bienthesanpham,
                                   masanpham = z.Masanpham,
                                   Tensanpham = z.Tensanpham,
                                   Size = a.TenSize,
                                   Mau = b.Tenmau,
                                   Soluongmua = x.Soluong,
                                   Gia = z.Giaban,
                                   Thanhtien = x.Thanhtien
                               }).ToList();

            billoforder bill = new billoforder();
            bill.madonhang = order.madonhang;
            bill.ngaydat = (DateTime)order.ngaytao;
            bill.Tennguoidung = order.Tennguoidung;
            bill.sodt = order.sodt;
            bill.Diachi = order.Diachi;
            bill.Tongdon = (int) order.Tongdon;

            foreach(var item in detailorder)
            {
                var query = new detailbillorder
                {
                    mabienthe = item.mabienthe,
                    masanpham = item.masanpham,
                    Tensanpham = item.Tensanpham,
                    Size = item.Size,
                    Mau = item.Mau,
                    Soluongmua = item.Soluongmua,
                    Gia = (int)item.Gia,
                    Thanhtien = (int)item.Thanhtien,
                };
                bill.ctdh.Add(query);
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

            var htmlPdf = base.RenderPartialToString("~/Areas/Admin/Views/BuyBill/PartialViewPdfResult.cshtml", bill, ControllerContext);


            // create a new pdf document converting an html string
            PdfDocument doc = converter.ConvertHtmlString(htmlPdf);
            string fileName = string.Format("{0}.pdf", DateTime.Now.Ticks);
            string pathFile = string.Format("{0}/{1}", Server.MapPath("~/Resource/Pdf"), fileName);
            doc.Save(pathFile);

            // save pdf document


            // close pdf document
            doc.Close();

            return Json(fileName, JsonRequestBehavior.AllowGet);
        }
        public ActionResult PartialViewPdfResult()
        {
            var model = new billoforder();
            return PartialView(model);
        }
    }
}