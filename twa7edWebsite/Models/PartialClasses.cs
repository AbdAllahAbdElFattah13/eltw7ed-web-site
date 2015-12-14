using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace twa7edWebsite.Models
{
    public partial class Item
    {
        [Required]
        [Display(Name = "اسم المنتج")]
        public string name { get; set; }

        [Display(Name = "صورة المنتج")]
        public string imageName { get; set; }

        [Required]
        [Display(Name = "سعر المنتج")]
        [DataType(DataType.Currency)]
        public decimal price { get; set; }
    }

    public partial class Category
    {
        [Required]
        [Display(Name = "اسم الصنف")]
        public string name { get; set; }

        [Display(Name = "صورة الصنف")]
        public string imageName { get; set; }

        [Display(Name = "تفاصيل عن الصنف")]
        public string description { get; set; }
    }

    public partial class ContactU
    {
        [Required]
        [MinLength(4), MaxLength(100)]
        public string name { get; set; }

        [Required]
        [MinLength(4), MaxLength(100)]
        [EmailAddress(ErrorMessage="Enter a correct Email Address")]
        public string email { get; set; }

        public string mobile { get; set; }

        [Required]
        [MinLength(4), MaxLength(100)]
        public string subject { get; set; }

        [Required]
        [MinLength(4), MaxLength(100)]
        public string massage { get; set; }
    }
}