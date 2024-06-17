using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Website_clothe.Models
{
    public class User_and_account
    {
        [Required]
        public string username { get; set; }
        [Required]
        public string password { get; set; }

        public bool Remember { get; set; }
    }
    public class register_user
    {
        [Required(ErrorMessage = "Vui lòng nhập tên người dùng.")]
        public string Tennguoidung { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập địa chỉ email.")]
        [EmailAddress(ErrorMessage = "Địa chỉ email không hợp lệ.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên đăng nhập.")]
        public string username { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu.")]
        [StringLength(50, MinimumLength = 6, ErrorMessage = "Mật khẩu phải có độ dài từ 6 đến 30 ký tự.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại.")]
        public string Sodt { get; set; }
    }
    public class Forgetpassword
    {
        [Required(ErrorMessage = "Vui lòng nhập tên người dùng.")]
        public string username { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập password người dùng.")]
        [StringLength(50, MinimumLength = 6, ErrorMessage = "Password phải có độ dài từ 6 đến 30 ký tự.")]
        public string password { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập comfirmpassword.")]
        [Compare("password", ErrorMessage = "password không khớp")]
        public string comfirmpassword { get; set; }
    }
    public class info_pay
    {
        [Required]
        [Compare("tennguoidung",ErrorMessage = "tên người dùng là trường bắt buộc")]
        public string tennguoidung { get; set; }
        public string email { get; set; }
        public string sdt { get; set; }
        [Required]
        [Compare("Diachi",ErrorMessage = "Địa chỉ là một trường bắt buộc")]
        public string Diachi { get; set; }
        public string phuongthucthanhtoan { get; set; }

    }
}