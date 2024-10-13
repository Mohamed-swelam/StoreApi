﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Store.Models.Models
{
    public class Review
    {
        [Key]
        public int Id { get; set; }
        public string UserId {  get; set; }
        public int ProductId { get; set; }
        [Range(1,5)]
        public int Rating {  get; set; }
        public string Comment { get; set; }
        public DateTime CreatedDate { get; set; }
        [ForeignKey("UserId")]
        public ApplicationUser? User { get; set; }
        [ForeignKey("ProductId")]
        public Product? Product { get; set; }

    }
}
