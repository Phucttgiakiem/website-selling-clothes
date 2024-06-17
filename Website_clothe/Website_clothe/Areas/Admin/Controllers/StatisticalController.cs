using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Web;
using System.Web.Mvc;
using Website_clothe.Models;

namespace Website_clothe.Areas.Admin.Controllers
{
    public class StatisticalController : Controller
    {
        public Server_clothesEntities5 db = new Server_clothesEntities5();
        
        public ActionResult ShowStatsticalwithrevenue()
        {
            if (Session["AdminUsername"] == null)
            {
                return View("/Areas/Admin/Views/AdminUser/Login.cshtml");
            }
            return View();
        }

        public ActionResult ShowStatsticalwithproduct() 
        {
            if (Session["AdminUsername"] == null)
            {
                return View("/Areas/Admin/Views/AdminUser/Login.cshtml");
            }
            return View(); 
        }
        
        //[HttpPost]
        //public ActionResult GetSalesDatawithmonth()
        //{
        //    // Tạo danh sách chứa tất cả các tháng từ tháng 1 đến tháng 12
        //    var allMonths = Enumerable.Range(1, 12).Select(month => month.ToString()).ToList();
           
        //    var result = db.Donmuahangs
        //        .Where(x => x.ID_Trangthai == 2002)
        //        .GroupBy(x => new { Year = x.Ngaytao.Value.Year, Month = x.Ngaytao.Value.Month })
        //        .Select(g => new
        //        {
        //            Year = g.Key.Year,
        //            Month = g.Key.Month,
        //            TotalRevenue = g.Sum(x => x.Tongdonhang)
        //        })
        //        .OrderBy(g => g.Year)
        //        .ThenBy(g => g.Month)
        //        .ToList();

        //    var data = allMonths
        //        .GroupJoin(result,
        //                   m => m,
        //                   r => r.Month.ToString(),
        //                   (m, r) => new { Month = m, TotalRevenue = r.Select(x => x.TotalRevenue).FirstOrDefault() ?? 0 })
        //        .ToList();
        //    return Json(data);
        //}
        [HttpPost]
        public ActionResult GetSalesDataForMonthAndYear(int year, int month)
        {
            var daysInMonth = DateTime.DaysInMonth(year, month);
            var startDate = new DateTime(year, month, 1);
            var endDate = new DateTime(year, month, daysInMonth);

            var filteredData = db.Donmuahangs
                .Where(x => EntityFunctions.TruncateTime(x.Ngaytao) >= startDate.Date && EntityFunctions.TruncateTime(x.Ngaytao) <= endDate.Date && x.ID_Trangthai == 2002)
                .Select(x => new
                {
                    Date = EntityFunctions.TruncateTime(x.Ngaytao),
                    TotalRevenue = x.Tongdonhang
                })
                .ToList();

            var groupedData = filteredData
                .GroupBy(x => x.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    TotalRevenue = g.Sum(x => x.TotalRevenue)
                })
                .OrderBy(g => g.Date)
                .ToList();

            var salesData = Enumerable.Range(1, daysInMonth)
            .Select(day => new
            {
                Day = day,
                TotalRevenue = groupedData.FirstOrDefault(r => GetDayFromDate((DateTime)r.Date) == day && GetMonthFromDate((DateTime)r.Date) == month)?.TotalRevenue ?? 0
            })
            .ToList();

            return Json(salesData);
        }
        [HttpPost]
        public ActionResult GetTopSellingProductsForMonthAndYear(int year, int month, int top)
        {
            var startDate = new DateTime(year, month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);

            var topProducts = db.Donmuahangs
                .Where(x => x.Ngaytao >= startDate && x.Ngaytao <= endDate && x.ID_Trangthai == 2002)
                .SelectMany(order => order.Chitietdonhangs) // Assume Chitietdonhangs is the table holding order details
                .GroupBy(detail => detail.ID_Sanphambienthe) // Replace ProductId with the actual foreign key in your schema
                .Select(group => new
                {
                    ProductId = group.Key,
                    TotalQuantity = group.Sum(detail => detail.Soluong)
                })
                .OrderByDescending(group => group.TotalQuantity)
                .Take(top)
                .ToList();

            

            return Json(topProducts);
        }
        public int GetDayFromDate(DateTime date)
        {
            return date.Day;
        }

        public int GetMonthFromDate(DateTime date)
        {
            return date.Month;
        }
    }
}