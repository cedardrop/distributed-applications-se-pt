using System;
using System.ComponentModel.DataAnnotations;

namespace BeverageWarehouseAPI.Models
{
    public class Category
    {
        public int CategoryId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(250)]
        public string Description { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [Required]
        public bool IsActive { get; set; } = true;

        [StringLength(250)]
        public string AdditionalInfo { get; set; }
    }
}
