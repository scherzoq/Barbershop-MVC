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
		// NOTE: the user stories presented here are limited in scope to specific
		// functionalities of a barbershop waiting list; these do NOT include
		// database creation/maintenance.
		//
		// At present, therefore, no database is yet used in this application.
		// To meet the needs of the specific user stories worked on for this
		// project, private static lists (populated with sample data) are used as
		// a straightforward solution to maintain the state of the waiting list
		// and the active haircuts.
		//
		// (But it is assumed that a database of all customers (new/returning) could
		// nonetheless later be implemented, and necessary modifications made at
		// that stage in the process.)
		private static List<Customer> customers = new List<Customer>()
		{
		new Customer{FullName="George Washington",Phone="202-456-1111",Barber="Joe"},
		new Customer{FullName="John Adams",Phone="202-456-2222",Barber="Gary"},
		new Customer{FullName="Tom Jefferson",Phone="202-456-3333"},
		new Customer{FullName="Jim Madison",Phone="202-456-4444",Barber="Gary"},
		new Customer{FullName="Jim Monroe",Phone="202-456-5555",Barber="Joe"},
		new Customer{FullName="Andy Jackson",Phone="202-456-7777",Barber="Joe"},
		new Customer{FullName="John Tyler",Phone="202-456-1010"},
		new Customer{FullName="Abe Lincoln",Phone="202-456-1616",Barber="Joe"},
		new Customer{FullName="Bill McKinley",Phone="202-456-2525",Barber="Gary"},
		new Customer{FullName="Ted Roosevelt",Phone="202-456-2626"}
		};
		private static List<Barber> barbers = new List<Barber>()
		{
		new Barber{FirstName="Joe"},
		new Barber{FirstName="Gary"}
		};

		public ActionResult Index()
		{
			// prepare display for active haircuts
			IList<Barber> barberList = new List<Barber>();
			foreach (Barber barber in barbers)
			{
				barberList.Add(barber);
			}
			ViewData["barbers"] = barberList;

			// assigned from GetPositionByName method or GetPositionByPhone
			// method when either is called
			ViewBag.Position = TempData["position"];

			// CustomerWaitingTime method will always be called here to calculate
			// customer wait times
			var currentTime = System.DateTime.Now;
			ViewBag.CurrentTime = currentTime;
			CustomerWaitingTime(currentTime);
					   
			// displays waiting list filtered by a specific requested barber when the
			// BarberWaitlist method is called (if that barber exists); if method is
			// not called or if searched-for barber does not exist, entire waiting list
			// is displayed
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

		// 1. method moves customer to requested barber's chair if it is unoccupied
		// 2. if that chair is occupied (OR if there is no requested barber), method will
		// search through the barber list and move customer to the first open chair
		// 3. if ALL chairs are taken, then nothing will happen (customers need to be moved
		// out of barbers' chairs, first, using the "EmptyChair" method)
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

		// method moves a customer out of a barber's chair after haircut is finished
		public ActionResult EmptyChair(string name)
		{
			int barberIndex = barbers.FindIndex(x => x.FirstName == name);
			barbers[barberIndex].Customer = null;
			return RedirectToAction("Index");
		}

		// calculates values for expected waiting/seating times for each customer on
		// the waiting list; the following assumptions are made:
		// 1. a haircut takes 15 minutes
		// 2. there are 90 seconds of cleanup required after each haircut
		// 3. specific barber requests are IGNORED for these particular calculations,
		// since each customer on the waiting list is permitted to go with the first
		// available barber when it is their turn
		//
		// the factors that this method uses to calculate wait times are:
		// 1. the total number of working barbers (this method will still work if a
		// third barber is added to the barbershop)
		// 2. the number of active haircuts AT THE TIME that the index action is called
		// 
		// the method does NOT track how much time has elapsed since the last time the
		// index action was called, and will therefore not readjust expected wait times
		// UNTIL customers are moved out of/into chairs). It also does not know when
		// a customer was moved into their chair. It estimates ONLY based on the two
		// factors listed above.
		public static void CustomerWaitingTime(DateTime currentTime)
		{
			TimeSpan haircut = new TimeSpan(0, 15, 0);
			TimeSpan cleanup = new TimeSpan(0, 1, 30);

			int barberCount = barbers.Count();
			int activeBarberCount = barbers.Count(x => x.Customer != null);
			int difference = barberCount - activeBarberCount;

			for (int i=0; i<customers.Count(); i++)
			{
				if (i < difference)
				{
					TimeSpan haircutWait = new TimeSpan(0, 0, 0);
					customers[i].WaitingTime = haircutWait;
				}
				else if (i < barberCount)
				{
					double haircutWaitNumber = (15 / (Convert.ToDouble(activeBarberCount) + 1)) * (i + 1 - difference);
					TimeSpan haircutWait = TimeSpan.FromMinutes(haircutWaitNumber);
					customers[i].WaitingTime = haircutWait.Add(cleanup);
				}
				else
				{
					int j = i / barberCount;
					int k = i % barberCount;
					TimeSpan totalAddedWait = new TimeSpan(0, 0, 0);
					for (int y = 0; y < j; y++)
					{
						totalAddedWait = totalAddedWait.Add(haircut).Add(cleanup);
					}			
					customers[i].WaitingTime = customers[k].WaitingTime.Add(totalAddedWait);
				}
			}

			for (int i = 0; i < customers.Count(); i++)
			{
				customers[i].SeatingTime = currentTime.Add(customers[i].WaitingTime);
			}
		}
		
		public ActionResult About()
		{
			return View();
		}
	}
}