//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Website_clothe.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class TrangThaiDH
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TrangThaiDH()
        {
            this.Donmuahangs = new HashSet<Donmuahang>();
        }
    
        public int ID_TrangThai { get; set; }
        public string TenTrangThai { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Donmuahang> Donmuahangs { get; set; }
    }
}
