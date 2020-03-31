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
		[Display(Name = "Expected Waiting Time")]
		public TimeSpan WaitingTime { get; set; }
		[Display(Name = "Expected Seating Time")]
		public DateTime SeatingTime { get; set; }
	}
}