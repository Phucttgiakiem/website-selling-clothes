using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Website_clothe.Models
{
    public class billofimport
    {
        public billofimport()
        {
            ctbill = new List<detailbillimp>(); // Khởi tạo ctbill trong constructor
        }
        public int ID_phieunhap { get; set; }
        public string tennhacungcap { get; set; }
        public string diachi { get; set; }
        public string sodt { get; set; }
        public DateTime ngaynhap { get; set; }
        public int tongtiennhap { get; set; }
        public string nguoilapphieu { get; set; }   
        public string ghichu { get; set; }
        public List<detailbillimp> ctbill { get; set; }

    }
}