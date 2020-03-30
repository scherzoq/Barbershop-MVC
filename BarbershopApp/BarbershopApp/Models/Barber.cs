using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace BarbershopApp.Models
{
    public class Barber
    {
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        public Customer Customer { get; set; }
    }
}