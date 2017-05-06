using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WSP.Entities
{
    public class SiteAddress
    {
        [ScaffoldColumn(false)]
        public int Id { get; set; }

        [Required(ErrorMessage = "Enter an url of the website")]
        [StringLength(150, MinimumLength = 6, ErrorMessage = "String length must be from 6 to 150 characters")]
        [RegularExpression(@"https?[\w\W]+", ErrorMessage = "Incorrect address")]
        [Display(Name = "Website address")]
        public string UrlAddress { get; set; }

        public string GuidString { get; set; }


        public virtual ICollection<SiteMape> SiteMapes { get; set; }
        public SiteAddress()
        {
            SiteMapes = new List<SiteMape>();
        }

    }
}