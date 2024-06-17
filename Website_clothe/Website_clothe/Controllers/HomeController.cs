using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Website_clothe.Models;

namespace Website_clothe.Controllers
{
    public class HomeController : Controller
    {
        public Server_clothesEntities5 _db = new Server_clothesEntities5();
        public ActionResult Index()
        {
            List<Danhmucsanpham> query = _db.Danhmucsanphams.ToList();
            List<Chitietdanhmuc> _kq = _db.Chitietdanhmucs.ToList();
            ViewBag.Query = _kq;
            return View(query);
        }
    }
}