using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Runtime;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Website_clothe.Models;

namespace Website_clothe.Areas.Admin.Controllers
{
    
    public class DetailCatagoryController : Controller
    {
        public Server_clothesEntities5 db = new Server_clothesEntities5();
        // GET: Admin/DetailCatagory
        [HttpGet]
        public ActionResult Index( )
        {
            if (Session["AdminUsername"] == null)
            {
                return View("/Areas/Admin/Views/AdminUser/Login.cshtml");
            }
            List<Danhmucsanpham> kq = db.Danhmucsanphams.OrderBy(c => c.Madanhmuc).ToList();
            List<Chitietdanhmuc> query = null;
            var categoryIds = kq.Select(c => c.Madanhmuc).ToList();
            query = db.Chitietdanhmucs.Where(p => p.Madanhmuc == categoryIds.FirstOrDefault()).ToList();
            
            

            ViewBag.kq = kq;
           
             return View(query);
        }
        public JsonResult loadtable(int id)
        {
            string json = "";
            var result = db.Chitietdanhmucs.Where(c => c.Madanhmuc == id).ToList();
            if(result.Count > 0)
            {
                List<datajson> data = new List<datajson>();
                result.ForEach(c =>
                {
                    var kq1 = new datajson
                    {
                        mactdm = c.ID_Chitietdanhmuc,
                        hinhanh = c.HinhAnh,
                        tenctdm = c.Tenchitietdanhmuc,
                        madmg = (int)c.Madanhmuc,

                    };
                    data.Add(kq1);
                });
                json = JsonConvert.SerializeObject(data);
            }
            return Json(json, JsonRequestBehavior.AllowGet);
        }
        public async Task<ActionResult> Delete(int id_ctdm)
        {
            var CtDanhmuc = db.Chitietdanhmucs.FirstOrDefault(x => x.ID_Chitietdanhmuc == id_ctdm);
            if (CtDanhmuc != null)
            {
                var sanpham = db.Sanphams.Where(x => x.Chitietdanhmuc == CtDanhmuc.ID_Chitietdanhmuc).ToList();
                if (sanpham.Any())
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
                            await db.SaveChangesAsync();
                            if (dondat.Any())
                            {
                                db.Donmuahangs.RemoveRange(dondat);
                               await db.SaveChangesAsync();
                            }
                            
                        }
                        var phieunhapct = db.Chitietphieunhaps.Where(x => btsanphamIDs.Contains((int)x.ID_Bienthesanpham)).ToList();
                        if (phieunhapct.Any())
                        {
                            var phieunhapIDs = phieunhapct.Where(x => x.ID_Phieunhaphang.HasValue).Select(x => x.ID_Phieunhaphang.Value).ToList();
                            var phieunhap = db.Phieunhaps.Where(x => phieunhapIDs.Contains(x.ID_Phieunhap)).ToList();
                            db.Chitietphieunhaps.RemoveRange(phieunhapct);
                            await db.SaveChangesAsync();
                            if (phieunhap.Any())
                            {
                                db.Phieunhaps.RemoveRange(phieunhap);
                                await db.SaveChangesAsync();
                            }
                        }
                        db.Bienthesanphams.RemoveRange(btsanpham);
                      await  db.SaveChangesAsync();
                    }
                    var hinhsp = db.HinhanhSanphams.Where(x => sanphamIDs.Contains((int)x.Masanpham)).ToList();
                    if (hinhsp.Any())
                    {
                        db.HinhanhSanphams.RemoveRange(hinhsp);
                      await  db.SaveChangesAsync();
                    }
                    db.Sanphams.RemoveRange(sanpham); 
                   await  db.SaveChangesAsync();
                }
                db.Chitietdanhmucs.Remove(CtDanhmuc);
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }
        [HttpGet]
        public ActionResult Create()
        {
            if (Session["AdminUsername"] == null)
            {
                return View("/Areas/Admin/Views/AdminUser/Login.cshtml");
            }
            var result = db.Danhmucsanphams.ToList();
            return View(result);
        }
        [HttpPost]
        public ActionResult Create(int danhmucsp,string name_detaildb, HttpPostedFileBase image)
        {
            var result = db.Danhmucsanphams.ToList();
            var query = db.Chitietdanhmucs.Where(x => x.Tenchitietdanhmuc == name_detaildb);
            if(query.Any())
            {
                ViewBag.error = "Tên chi tiết danh mục đã tồn tại trên hệ thống";
                return View(result);
            }
            if(image == null)
            {
                ViewBag.error = "bạn chưa chọn ảnh";
                return View(result);
            }
            else
            {
                if (!IsImageFile(image))
                {
                    ViewBag.error = "File bạn chọn không phải là file ảnh";
                    return View(result);
                }
                try
                {
                    string filename = Path.GetFileName(image.FileName);
                    string extension = Path.GetExtension(filename);
                    string uniqueFileName = Path.GetFileNameWithoutExtension(filename) + "_" + Guid.NewGuid().ToString() + extension;
                    string path = Path.Combine(Server.MapPath("~/Content/img"), uniqueFileName);
                    image.SaveAs(path);

                    //lưu dữ liệu xuống database
                    var kq = new Chitietdanhmuc
                    {
                        Madanhmuc = danhmucsp,
                        Tenchitietdanhmuc = name_detaildb,
                        HinhAnh = uniqueFileName
                    };
                    db.Chitietdanhmucs.Add(kq);
                    db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = "Lỗi khi upload ảnh: " + ex.Message;
                    return View(result);
                }
            }
        }
        [HttpGet]
        public ActionResult Edit(int id_ctdm)
        {
            var result = db.Chitietdanhmucs.Where(c => c.ID_Chitietdanhmuc == id_ctdm).FirstOrDefault();
            ViewBag.danhmucsp = db.Danhmucsanphams.ToList();
            return View(result);
        }
        [HttpPost]
        public ActionResult Edit(int danhmucsp,string tenctdanhmuc,int mactdm, HttpPostedFileBase image)
        {
            ViewBag.danhmucsp = db.Danhmucsanphams.ToList();
            var result = db.Chitietdanhmucs.Where(c => c.ID_Chitietdanhmuc == mactdm).FirstOrDefault();
            
            
            
            if (image != null)
            {
                
                if(IsImageFile(image))
                {
                    string filename = Path.GetFileName(image.FileName);
                    string extension = Path.GetExtension(filename);
                    string uniqueFileName = Path.GetFileNameWithoutExtension(filename) + "_" + Guid.NewGuid().ToString() + extension;
                    string path = Path.Combine(Server.MapPath("~/Content/img"), uniqueFileName);
                    image.SaveAs(path);

                    var query = db.Chitietdanhmucs.First(c => c.ID_Chitietdanhmuc == mactdm);
                    query.Madanhmuc = danhmucsp;
                    query.Tenchitietdanhmuc = tenctdanhmuc;
                    query.HinhAnh = uniqueFileName;
                    UpdateModel(query);
                    db.SaveChangesAsync();
                }
                else
                {
                    ViewBag.error = "Bạn vừa chọn không phải là file ảnh";
                    return View(result);
                }
            }
            else
            {
                var query = db.Chitietdanhmucs.First(c => c.ID_Chitietdanhmuc == mactdm);
                query.Madanhmuc = danhmucsp;
                query.Tenchitietdanhmuc = tenctdanhmuc;
                UpdateModel(query);
                db.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }

        private bool IsImageFile(HttpPostedFileBase file)
        {
            if (file != null)
            {
                // Kiểm tra loại MIME của file để xác định xem nó có phải là file ảnh hay không
                string[] allowedImageTypes = { "image/jpeg", "image/jpg", "image/png", "image/gif" };
                return allowedImageTypes.Contains(file.ContentType);
            }

            return false;
        }

    }
    public class datajson
    {
        public int mactdm;
        public string tenctdm;
        public int madmg;
        public string hinhanh;
    }
}