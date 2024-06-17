using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Website_clothe.Models;
using System.Drawing;
using System.IO;

namespace Website_clothe.Areas.Admin.Controllers
{
    
    public class ProductImageController : Controller
    {
        Server_clothesEntities5 db = new Server_clothesEntities5();
        public ActionResult Index(int id)
        {
            if (Session["AdminUsername"] == null)
            {
                return View("/Areas/Admin/Views/AdminUser/Login.cshtml");
            }
            ViewBag.ProductId = id;
            var items = db.HinhanhSanphams.Where(x => x.Masanpham == id).ToList();
            return View(items);
        }
        [HttpPost]
        public ActionResult AddImage (int productId,string url)
        {
            try {
                string filepath = Server.MapPath(url);

                if (System.IO.File.Exists(filepath))
                {
                    using (FileStream fs = new FileStream(filepath, FileMode.Open, FileAccess.Read))
                    {
                        Image img = Image.FromStream(fs);
                        // Lưu tên file ban đầu (không có phần mở rộng)
                        string fileName = Path.GetFileName(filepath);

                        string extension = Path.GetExtension(fileName);
                        string uniqueFileName = Path.GetFileNameWithoutExtension(fileName) + "_" + Guid.NewGuid().ToString() + extension;
                        string pathToSave = Path.Combine(Server.MapPath("~/Content/img"), uniqueFileName);

                        // Lưu file mới
                        img.Save(pathToSave);


                        //lưu tên lên database
                        var data = new HinhanhSanpham
                        {
                            TenAnh = uniqueFileName,
                            Masanpham = productId
                        };
                        db.HinhanhSanphams.Add(data);
                        db.SaveChanges();
                    }
                }
                // Xử lý xong và trả về kết quả thành công
                return Json(new { Success = true });
            }
            catch(Exception ex)
            {
                return Json(new { Success = false, ErrorMessage = ex.Message });
            }
        }
        [HttpPost]
        public ActionResult DeleteItem (int id)
        {
            var item = db.HinhanhSanphams.Find(id);
            db.HinhanhSanphams.Remove(item);
            db.SaveChanges();
            return Json(new { success = true });
        }
        [HttpPost]
        public ActionResult DeleteAllItem (int id)
        {
            var item = db.HinhanhSanphams.Where(x => x.Masanpham == id).ToList();
            if(item.Count() == 0)
            {
                return Json(new {success = false});
            }
            db.HinhanhSanphams.RemoveRange(item);
            db.SaveChanges();
            return Json(new { success = true });

        }
    }
}