using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
namespace Website_clothe.Models
{
    public class Shopping_card
    {
        public List<ShoppingcardItem> Items { get; set; }
        public Shopping_card() { 
            this.Items = new List<ShoppingcardItem>();
        }
        public void AddTocard (ShoppingcardItem item,int soluong) {
            var checkitem = Items.FirstOrDefault(x => x.productid == item.productid && x.variantproductid == item.variantproductid);
            if(checkitem != null)
            {
                checkitem.soluong += soluong;
                checkitem.totalprice = checkitem.price * checkitem.soluong;
            }
            else
            {
                Items.Add(item);
            }
        }
        public void UpdateTocard (int id_sp,int id_dsp,int soluong)
        {
            var checkitem = Items.SingleOrDefault(x => x.productid == id_sp && x.variantproductid == id_dsp);
            if(checkitem != null)
            {
                checkitem.soluong = soluong;
                checkitem.totalprice = checkitem.price * checkitem.soluong;
            }
        }
        public void Removecard (int id_sp,int id_dsp)
        {
            var checkitem = Items.SingleOrDefault(x => x.productid == id_sp && x.variantproductid == id_dsp);
            if(checkitem != null)
            {
                Items.Remove(checkitem);
            }
        }
        public void Clearcard()
        {
            Items.Clear();
        }
        public int TotalItem()
        {
            return Items.Count;
        }
    }
    public class ShoppingcardItem
    {
        public int productid { get; set; }
        public string productname { get; set; }
        public int variantproductid { get; set; }
        public string size { get; set; }
        public string color { get; set; }
        public int soluong { get; set; }
        public int price { get; set; }
        public int totalprice { get; set; }
    }
}

