using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RoadTrip.Models
{
    public class AttractionsMetadata
    {
        public int AttractionId { get; set; }
        [Required][StringLength(50)]
        public string Name { get; set; }
        [Required][StringLength(50)]
        public string Region { get; set; }
        public string Description { get; set; }
        [Display(Name = "Image URL")]
        public string Image { get; set; }
        public int CountryId { get; set; }
    }
}