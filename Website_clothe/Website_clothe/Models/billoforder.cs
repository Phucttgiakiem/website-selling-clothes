using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Website_clothe.Models
{
    public class billoforder
    {
        public billoforder() { 
            ctdh = new List<detailbillorder>();
        }
       public int madonhang { get; set; }
       public DateTime ngaydat { get; set; }
       public string Tennguoidung { get; set; }
       public string sodt { get; set; }
       public string Diachi { get; set; }
       public int Tongdon { get; set; } 
       public List<detailbillorder> ctdh { get; set; }
    }

    public class detailbillorder
    {
       public int mabienthe { get; set; }
       public int masanpham { get; set; }
       public string Tensanpham { get; set; }
       public string Size { get; set; }
       public string Mau { get; set; }
       public int Soluongmua { get; set; }
       public int Gia { get; set; }
       public int Thanhtien { get; set;}
    }
}