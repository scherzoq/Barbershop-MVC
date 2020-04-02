# Barbershop MVC Application

* [Introduction](#introduction)
* [Running the Program](#running-the-program)
* [Notes on Specific Components](#notes-on-specific-components)

## Introduction
For this coding project, I worked on two user stories for an MVC application for a barbershop waiting list. The requirements of the coding challenge, on my end, were quite detailed but nonetheless limited in scope.

I had to meet a number of specific acceptance criteria for the two user stories I worked on, such as the following functionalities: searching a customer's position in line by name or phone number, sorting the waiting list by a specfic barber, implementing an "in chair" button to move a customer to a barber's chair, and displaying expected wait times.

But certain essential functionalities -- most notably, adding new customers to the waiting list and database creation/maintenance -- were outside the scope of the user stories that I worked on, and are currently omitted from this application.

## Running the Program
Requires Visual Studio to run. This program was created in Visual Studio 2019, which is free to download here:
https://visualstudio.microsoft.com/downloads/

With Visual Studio installed, take the following steps to run the program:
1. In "Barbershop-MVC\BarbershopApp", double-click "BarbershopApp.sln" to open the solution in Visual Studio
2. Open the Solution Explorer (Ctrl + Alt + L) and find the "Controllers" folder. Double-click on "HomeController.cs" to open the controller file
3. Press Ctrl + F5 to run the program

## Notes on Specific Components

### "In Chair" and "Empty Chair" Buttons
Clicking the "Empty Chair" Button simply removes a customer from a barber's chair once their haircut is complete.

Clicking the "In Chair" button can result in a few outcomes:
* The "In Chair" button moves a customer to their requested barber's chair so long as it is unoccupied.
* If that chair is occupied (OR if there is no requested barber), then the program will search through the list of barbers and move the customer to the first available chair.
* If ALL chairs are taken, then nothing will happen (customers MUST be moved out of barbers' chairs, first, using the "Empty Chair" button).

### Expected Wait Times
The expected wait times use the following assumptions:

* A haircut takes 15 minutes.
* There are 90 seconds of cleanup required after each haircut.

Please note that specific barber requests are IGNORED for these particular calculations, since each customer on the waiting list is permitted to change their mind and go with the first available barber when it is their turn (hence the open-ended functionality of the "in chair" button described above).

The program does NOT know when each active haircut started, and instead uses math/probability to calculate the expected (average) amount of time it will take for seats to open up. To elaborate on the this, the ONLY factors that are used to calculate wait times are:

* the total number of working barbers (the calculation will still work if an additional barber is added)
* the number of active haircuts AT THE TIME that the home page is loaded or reloaded

The method which calculates wait times does NOT track how much time has elapsed since the last time the home page was loaded, and will not readjust expected wait times UNTIL actions are taken (customers are moved out of/into chairs). It also does not track WHEN a customer was moved into their chair. It is important to emphasize that it is a method that does no more or less than the following: it considers how many barbers are working and how many active haircuts are taking place at any given moment, and uses this information alone to estimate a wait time for every customer on the wait list.

*Jump to: [Introduction](#introduction), [Running the Program](#running-the-program), [Notes on Specific Components](#notes-on-specific-components), [Page Top](#Barbershop-MVC-Application)*
