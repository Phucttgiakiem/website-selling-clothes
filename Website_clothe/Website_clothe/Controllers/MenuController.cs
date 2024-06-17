using System;
using System.Collections.Generic;
using System.EnterpriseServices.Internal;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Website_clothe.Models;

namespace Website_clothe.Controllers
{
    
    public class MenuController : Controller
    {
        private Server_clothesEntities5 db = new Server_clothesEntities5();
        // GET: Menu
        //public ActionResult Index()
        //{
        //    return View();
        //}
        
        [ChildActionOnly]
        public ActionResult MenuTop ()
        {
            var items = db.Danhmucsanphams.ToList();
            ViewBag.ctdm = db.Chitietdanhmucs.ToList();
            return PartialView("MenuTop",items);
        }
        public ActionResult Menuleft()
        {
            var items = db.Loais.ToList();
            ViewBag.size = db.Sizes.ToList();
            ViewBag.mau = db.MauSacs.ToList();
            ViewBag.thuonghieu = db.Thuonghieux.ToList();
            return PartialView("Menuleft",items);
        }
        public ActionResult Footermenu()
        {
            return PartialView();
        }
    }
}