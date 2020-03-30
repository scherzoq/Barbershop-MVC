using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace BarbershopApp.Models
{
	public class Customer
	{
		[Display(Name = "Full Name")]
		public string FullName { get; set; }
		public string Phone { get; set; }
		[Display(Name = "Requested Barber")]
		public string Barber { get; set; }
	}
}