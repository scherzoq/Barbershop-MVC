using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BarbershopApp.Models;

namespace BarbershopApp.Controllers
{
	public class HomeController : Controller
	{
		// **Include a README later. Cut and paste and modify as needed:
		//
		// For my first attempt at this coding challenge, I used the online IDE dotnetfiddle
		// (it was originally requested that I use an online IDE). The MVC template on
		// dotnetfiddle is very bare-bones, with no database support. So I did my best to
		// adapt by using some flexible solutions (e.g., private static variables to maintain
		// the data state).
		//
		// Eventually, I found that dotnetfiddle simply could not support the requirements of
		// this project -- even with attempts to implement flexible solutions. So I moved my
		// code to Visual Studio and made some necessary modifications. However, I also
		// retained much of the overall design structure from my dotnetfiddle code, such as
		// private static variables to maintain data state. This is why some of the design
		// choices in this program vary from more commonly seen MVC design.**
		private static List<Barber> barbers = new List<Barber>()
		{
		new Barber{FirstName="Joe"},
		new Barber{FirstName="Gary"}
		};
		private static List<Customer> customers = new List<Customer>()
		{
		new Customer{FullName="George Washington",Phone="202-456-1111",Barber="Joe"},
		new Customer{FullName="John Adams",Phone="202-456-2222",Barber="Gary"},
		new Customer{FullName="Tom Jefferson",Phone="202-456-3333"},
		new Customer{FullName="Jim Madison",Phone="202-456-4444",Barber="Gary"},
		new Customer{FullName="Jim Monroe",Phone="202-456-5555",Barber="Joe"},
		new Customer{FullName="John Tyler",Phone="202-456-1010"},
		new Customer{FullName="Abe Lincoln",Phone="202-456-1616",Barber="Joe"},
		new Customer{FullName="Ted Roosevelt",Phone="202-456-2626"}
		};

		public ActionResult Index()
		{
			IList<Barber> barberList = new List<Barber>();
			foreach (Barber barber in barbers)
			{
				barberList.Add(barber);
			}
			ViewData["barbers"] = barberList;
			ViewBag.Position = TempData["position"];
			string barberName = TempData["barberName"]?.ToString();

			if (barbers.Exists(x => x.FirstName == barberName))
			{
				List<Customer> barberCustomers = new List<Customer>();
				foreach (Customer customer in customers)
				{
					if (customer.Barber == barberName)
					{
						barberCustomers.Add(customer);
					}
				}
				return View(barberCustomers);
			}
			else
			{
				return View(customers);
			}
		}

		public ActionResult BarberWaitlist(string barber)
		{
			if (barbers.Exists(x => x.FirstName == barber))
			{
				TempData["barberName"] = barber;
				return RedirectToAction("Index");
			}
			else
			{
				return RedirectToAction("Index");
			}
		}

		public ActionResult GetPositionByName(string name)
		{
			if (customers.Exists(x => x.FullName == name))
			{
				int index = customers.FindIndex(x => x.FullName == name);
				TempData["position"] = index + 1;
			}
			else
			{
				TempData["position"] = "name not found";
			}
			return RedirectToAction("Index");
		}

		public ActionResult GetPositionByPhone(string phone)
		{
			if (customers.Exists(x => x.Phone == phone))
			{
				int index = customers.FindIndex(x => x.Phone == phone);
				TempData["position"] = index + 1;
			}
			else
			{
				TempData["position"] = "phone number not found";
			}
			return RedirectToAction("Index");
		}

		// will first try to move customer to requested barber's chair (if applicable);
		// if that chair is taken, will move customer to any open chair
		//
		// method will do nothing if all chairs are taken; customers need to be moved out
		// of chairs first using the "EmptyChair" method
		public ActionResult ToChair(string name)
		{
			int custIndex = customers.FindIndex(x => x.FullName == name);
			Customer activeCustomer = new Customer();
			activeCustomer = customers[custIndex];
			if (activeCustomer.Barber != null)
			{
				int barberIndex = barbers.FindIndex(x => x.FirstName == activeCustomer.Barber);
				if (barbers[barberIndex].Customer == null)
				{
					barbers[barberIndex].Customer = activeCustomer;
					customers.RemoveAll(x => x.FullName == name);
				}
				else
				{
					foreach (Barber barber in barbers)
					{
						if (barber.Customer == null)
						{
							barber.Customer = activeCustomer;
							customers.RemoveAll(x => x.FullName == name);
							break;
						}
					}
				}
			}
			else
			{
				foreach (Barber barber in barbers)
				{
					if (barber.Customer == null)
					{
						barber.Customer = activeCustomer;
						customers.RemoveAll(x => x.FullName == name);
						break;
					}
				}
			}
			return RedirectToAction("Index");
		}

		public ActionResult EmptyChair(string name)
		{
			int barberIndex = barbers.FindIndex(x => x.FirstName == name);
			barbers[barberIndex].Customer = null;
			return RedirectToAction("Index");
		}
	}
}