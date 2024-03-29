﻿using Microsoft.Build.Framework;
using System.ComponentModel.DataAnnotations;
using RequiredAttribute = System.ComponentModel.DataAnnotations.RequiredAttribute;

namespace FinalProject.Models
{
    public class Order
    {
        public int Id { get; set; }

        public string? AppUserId { get; set; }
        [StringLength(maximumLength: 60), Required(ErrorMessage = "Text_Required")]
        public string FullName { get; set; }
        [StringLength(maximumLength: 100), Required(ErrorMessage = "Text_Required")]
        public string Country { get; set; }
        [StringLength(maximumLength: 100), Required(ErrorMessage = "Text_Required")]
        public string City { get; set; }
        [StringLength(maximumLength: 100), DataType(dataType: DataType.EmailAddress), Required(ErrorMessage = "Text_Required")]
        public string Email { get; set; }
        [StringLength(maximumLength: 25),DataType(dataType:DataType.PhoneNumber), Required(ErrorMessage = "Text_Required")]
        public string PhoneNumber { get; set; }
        [StringLength(maximumLength:150), Required(ErrorMessage = "Text_Required")]
        public string Address1 { get; set; }
        [StringLength(maximumLength: 150), Required(ErrorMessage = "Text_Required")]
        public string? Address2 { get; set; }
        [StringLength(maximumLength: 1500), Required(ErrorMessage = "Text_Required")]
        public string? OtherNotes { get; set; }
        public double TotalPrice { get; set; }
        public DateTime CreatedTime { get; set; }
        public AppUser AppUser { get; set; }
        public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
