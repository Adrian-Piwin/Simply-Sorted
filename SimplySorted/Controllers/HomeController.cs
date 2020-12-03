using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SimplySorted.Models;

namespace SimplySorted.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private ItemDatabase _itemDatabase;

        private static string currentOwnershipId;

        private static bool isLoggedOn;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
            _itemDatabase = new ItemDatabase();
        }

        public IActionResult Index()
        {
            isLoggedOn = true; // testing purposes
            if (isLoggedOn)
            {
                return RedirectToAction("Homepage");
            }

            return View();
        }

        public IActionResult Logout()
        {
            isLoggedOn = false;
            currentOwnershipId = null;
            return Index();
        }

        public IActionResult HomePage()
        {
            if (!isLoggedOn)
            {
                // Go to login Page
                return RedirectToAction("Index");
            }

            // Set current owner ship id from user that logs in
            if (currentOwnershipId == null)
            {
                currentOwnershipId = "test123"; // testing purposes
            }

            List<Item> userItems = new List<Item>();
            foreach (Item item in _itemDatabase.Items)
            {
                if (item.ownershipId == currentOwnershipId)
                    userItems.Add(item);
            }

            return View(userItems);
        }

        [HttpGet]
        public IActionResult NewItem()
        {
            return View();
        }

        [HttpPost]
        public IActionResult NewItem(Item newItem)
        {
            newItem.ownershipId = currentOwnershipId;
            _itemDatabase.Items.Add(newItem);
            _itemDatabase.SaveChanges();

            return RedirectToAction("HomePage");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
