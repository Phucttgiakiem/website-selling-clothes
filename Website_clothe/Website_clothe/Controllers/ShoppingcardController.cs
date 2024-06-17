using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Website_clothe.Models;

namespace Website_clothe.Controllers
{
    

    public class ShoppingcardController : Controller
    {
        private Server_clothesEntities5 db = new Server_clothesEntities5();
        
        public ActionResult Index()
        {
            Shopping_card card = (Shopping_card)Session["Cart"];
            if(card != null && card.Items.Any())
            {
                ViewBag.Checkcard = card;
            }    
            return View();
        }
        
        public ActionResult Partial_Item_Cart()
        {
            Shopping_card card = (Shopping_card)Session["Cart"];
            if (card != null && card.Items.Any())
            {
                return PartialView(card.Items);
            }
            return PartialView();
        }
        [HttpPost]
        public ActionResult Addtocart(string id,int quantity,string colorpd,string sizepd)
        {
            var code = new { Success = false, msg = "", code = -1 };
            string[] code_pd = id.Split('-');
            int idsp = int.Parse(code_pd[1]+"");
            int idtype = int.Parse(code_pd[0]+"");

            var id_size = db.Sizes.Where(x => x.ID_Loai == idtype && x.TenSize == sizepd.Trim()).First();
            var id_color = db.MauSacs.Where(x => x.ID_Loai == idtype && x.Tenmau == colorpd.Trim()).First();
            var getproductdeital = db.Bienthesanphams.FirstOrDefault(x => x.ID_Sanpham == idsp && x.ID_Mau == id_color.ID_Mau && x.SizeID == id_size.ID_Size);
            
            
            if(getproductdeital!= null)
            {
                Shopping_card card = (Shopping_card)Session["Cart"];
                if(card == null)
                {
                    card = new Shopping_card();
                }
                var product = db.Sanphams.FirstOrDefault(x => x.Masanpham == getproductdeital.ID_Sanpham);
                
                ShoppingcardItem item = new ShoppingcardItem
                {
                    productid = product.Masanpham,
                    productname = product.Tensanpham,
                    variantproductid = getproductdeital.ID_Bienthesanpham,
                    size = sizepd,
                    color = colorpd,
                    soluong = quantity,
                    price = int.Parse(product.Giaban+""),
                    totalprice = int.Parse((quantity * product.Giaban)+"")
                };
                card.AddTocard(item, quantity);
                Session["Cart"] = card;
                code = new { Success = true, msg = "Thêm sản phẩm vào giỏ hàng thành công", code = 1};
            }
            return Json(code);
        }
        [HttpPost]
        public ActionResult Update (string id,int soluong)
        {
            Shopping_card card = (Shopping_card)Session["Cart"];
            if(card != null)
            {
                string[] code_sp = id.Split('-');
                int id_bienthe = int.Parse(code_sp[1]);
                int id_sp = int.Parse(code_sp[0]);
                var result = db.Bienthesanphams.Where(x => x.ID_Bienthesanpham == id_bienthe).Select(x => x.Soluongton).First();
                if(soluong > result)
                {
                    return Json(new {Success= false, msg = "Sản phẩm hiện chỉ còn có "+ result + " cái trong kho hàng !!",code = -1  });
                }
                card.UpdateTocard(id_sp, id_bienthe, soluong);
                return Json(new { Success = true });
            }
            return Json(new { Success = false, msg = "có lỗi xảy ra khi thêm số lượng sản phẩm, vui lòng bạn cập nhật lại sau !!",code = -1 });
        }
        public ActionResult CheckOutSuccess()
        {
            return View();
        }
        [HttpGet]
        public ActionResult CheckOut()
        {
            Shopping_card card = (Shopping_card)Session["Cart"];
            if(card != null && card.Items.Any())
            {
                if (Session["Manguoidung"] == null)
                {
                    return View("/Views/Access/Login.cshtml");
                }
                ViewBag.CheckCard = card;
            }
            return View();
        }
        [HttpPost]
        public ActionResult CheckOut(info_pay user)
        {
            var code = new { Success = false, Code = -1 };
            if(ModelState.IsValid) 
            {
                Shopping_card card = (Shopping_card)Session["Cart"];
                if(card != null)
                {
                    int totalbill = 0;
                    foreach(var item in card.Items)
                    {
                        totalbill += item.totalprice;
                    }
                    
                    Donmuahang bill = new Donmuahang();
                    bill.ID_Nguoidung = (int)Session["Manguoidung"];
                    bill.DiaChi = user.Diachi;
                    bill.Tennguoidung = user.tennguoidung;
                    bill.Tongdonhang = totalbill;
                    bill.Ngaytao = DateTime.Today;
                    bill.ID_Trangthai = 1;
                    db.Donmuahangs.Add(bill);
                   
                    db.SaveChanges();
                    foreach (var item in card.Items)
                    {
                        var data = new Chitietdonhang
                        {
                            ID_Donhang = bill.ID_Donhang,
                            ID_Sanphambienthe = item.variantproductid,
                            Soluong = item.soluong,
                            Thanhtien = item.totalprice,
                        };
                        db.Chitietdonhangs.Add(data);
                       
                    }
                    db.SaveChanges();


                    //giảm sản phẩm trong khi tạo thành công đơn mua hàng
                    //List<int> varianproducts = card.Items.Select(x => x.variantproductid).ToList();
                    //var query = db.Bienthesanphams.Where(x => varianproducts.Contains(x.ID_Bienthesanpham)).ToList();
                    //var query2 = db.Chitietdonhangs.Where(x => varianproducts.Contains((int)x.ID_Sanphambienthe) && x.ID_Donhang == bill.ID_Donhang).ToList();
                    //foreach(var item in query)
                    //{
                    //    foreach(var item2 in query2)
                    //    {
                    //        if(item2.ID_Sanphambienthe == item.ID_Bienthesanpham)
                    //        {
                    //            item.Soluongton -= item2.Soluong;
                    //            break;
                    //        }
                    //    }
                    //    UpdateModel(item);
                    //}
                    //db.SaveChanges();

                    card.Clearcard();
                    code = new { Success = true, Code = -1 };
                }
                
            }
            return Json(code);
        }
        
        
        public ActionResult Partial_CheckOut()
        {
            int iduser = (int)Session["Manguoidung"];
            var item = db.Nguoidungs.FirstOrDefault(x => x.ID_Nguoidung == iduser);
            info_pay info = new info_pay();
            if (item != null)
            {
                info.tennguoidung = item.Tennguoidung;
                info.email = item.Email;
                info.sdt = item.SoDT;
                info.Diachi= item.Diachi;
                info.phuongthucthanhtoan = "OCD";
            }
            return PartialView(info);
        }
        
        public ActionResult Partial_Item_pay()
        {
            Shopping_card card = (Shopping_card)Session["Cart"];
            if(card != null&& card.Items.Any())
            {
                return PartialView(card.Items);
            }
            return PartialView();
        }
        [HttpPost]
        public ActionResult DeleteCard(string id)
        {
            var code = new { Success = false, msg = "", code = -1 };
            Shopping_card card = (Shopping_card)Session["Cart"];
            if(card != null)
            {
                string[] code_sp = id.Split('-');
                int id_bienthe = int.Parse(code_sp[1]);
                int id_sp = int.Parse(code_sp[0]);
                var checkdetailpd = db.Bienthesanphams.FirstOrDefault(x => x.ID_Sanpham == id_sp && x.ID_Bienthesanpham == id_bienthe);
                if(checkdetailpd != null)
                {
                    card.Removecard(id_sp, id_bienthe);
                    code = new { Success = true, msg = "", code = 1 };
                }
            }
            return Json(code);
        }
        [HttpPost]
        public ActionResult DeleteAll()
        {
            Shopping_card card = (Shopping_card)Session["Cart"];
            if(card != null)
            {
                card.Clearcard();
                ViewBag.Checkcard = null;
                return Json(new { Success = true });
            }
            return Json(new { Success = false });
        }
        public ActionResult GetBillofCustommer()
        {
            int manguoidung = (int)Session["Manguoidung"];
            if(manguoidung > 0)
            {
                var data = db.Donmuahangs.Where(x=> x.ID_Nguoidung == manguoidung).ToList();
                return PartialView(data);
            }
            return PartialView();
        }
        public ActionResult GetBillwithday (string timestart,string timeend)
        {
            int manguoidung = (int)Session["Manguoidung"];
            
            DateTime times;
            DateTime timee;

            DateTime.TryParseExact(timestart, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out times);
            DateTime.TryParseExact(timeend, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out timee);
            if (manguoidung > 0)
            {
                var data = db.Donmuahangs.Where(x => x.ID_Nguoidung == manguoidung && x.Ngaytao >= times && x.Ngaytao <= timee).ToList();
                return PartialView(data);
            }
            return PartialView();
        }
        public ActionResult GetDetailBill (int idbill)
        {
            var data = db.Chitietdonhangs.Where(x => x.ID_Donhang == idbill).ToList();
            if(data != null)
            {
                int iduser = (int)Session["Manguoidung"];
                ViewBag.Nguoidung = db.Nguoidungs.FirstOrDefault(x => x.ID_Nguoidung == iduser);
                var item = db.Donmuahangs.FirstOrDefault(x => x.ID_Donhang == idbill);
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
                
                ViewBag.Trangthai = db.TrangThaiDHs.FirstOrDefault(x => x.ID_TrangThai == item.ID_Trangthai);
                
                ViewBag.Donmua = item;
                
                List<detailshoppingbill> bill = new List<detailshoppingbill>();
                foreach(var i_tem in data)
                {
                    detailshoppingbill query = new detailshoppingbill
                    {
                        madonhang = (int)i_tem.ID_Donhang,
                        masanpham = (int)i_tem.ID_Sanphambienthe,
                        soluong = i_tem.Soluong,
                    };
                    int sizeid = 0, colorid = 0, sanphamid = 0;
                    foreach(var i_tem2 in item2)
                    {
                        if(i_tem2.ID_Bienthesanpham == i_tem.ID_Sanphambienthe)
                        {
                            sizeid = (int)i_tem2.SizeID;
                            colorid = (int)i_tem2.ID_Mau;
                            sanphamid = (int)i_tem2.ID_Sanpham;
                            
                            break;
                        }
                    }
                    foreach(var i_tem3 in item3)
                    {
                        if(i_tem3.ID_Size == sizeid)
                        {
                            query.size = i_tem3.TenSize;
                            break;
                        }
                    }
                    foreach(var i_tem4 in item4)
                    {
                        if(i_tem4.ID_Mau == colorid)
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
                            break;

                        }
                    }
                    bill.Add(query);

                }
                
                return PartialView(bill);
            }
            return PartialView();
        }
        [HttpPost]
        public ActionResult CancelBill (int iddonhang,int idtrangthai)
        {
            var data = db.Donmuahangs.FirstOrDefault(x => x.ID_Donhang == iddonhang);
            
            if(data != null)
            {
                var result = db.Chitietdonhangs.Where(x => x.ID_Donhang == data.ID_Donhang).ToList();
                if(result != null && result.Count() > 0)
                {
                    if(idtrangthai != 1)
                    {
                        List<int?> listNullableInts = result.Select(x => x.ID_Sanphambienthe).ToList();
                        List<int> masp = listNullableInts
                                .Where(x => x.HasValue) // Lọc ra các giá trị không null
                                .Select(x => x.Value)   // Lấy giá trị không null
                                .ToList();
                        var result1 = db.Bienthesanphams.Where(x => masp.Contains(x.ID_Bienthesanpham)).ToList();
                        foreach (var item in result1)
                        {
                            foreach (var item2 in result)
                            {
                                if (item2.ID_Sanphambienthe == item.ID_Bienthesanpham)
                                {
                                    item.Soluongton += item2.Soluong;
                                  
                                    break;
                                }
                            }
                        }
                    }
                    db.Chitietdonhangs.RemoveRange(result);
                    
                }
            }
            db.Donmuahangs.Remove(data);
            db.SaveChangesAsync();
            return Json(new { Success = true });
        }
    }
}