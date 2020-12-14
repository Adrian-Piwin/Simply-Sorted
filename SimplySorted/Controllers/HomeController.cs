using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SimplySorted.Models;
using SimplySorted.ViewModels;

namespace SimplySorted.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private ItemDatabase _itemDatabase;

        private static string currentOwnershipId;

        private static int currentEditingId;

        private static IQueryable<Item> searchResults;



        public HomeController(ILogger<HomeController> logger, ItemDatabase database)
        {
            _logger = logger;
            _itemDatabase = database;
        }

   


        // Home page, displaying inventory
        [Authorize]
        public IActionResult Index()
        {
           

            // Set current owner ship id from user that logs in
            if (currentOwnershipId == null)
            {
                currentOwnershipId = "test123"; // testing purposes
            }

            IQueryable<Item> userItems;

            if (searchResults == null)
            {
                // Get list of user items for owner
                userItems = _itemDatabase.Items.Where(x => x.ownershipId == currentOwnershipId);
            }
            else
            {
                userItems = searchResults;
                searchResults = null;
            }

            return View(userItems);
        }

        // Add new item page
        [HttpGet]
        public IActionResult NewItem()
        {
            return View();
        }

        // Add new item to database
        [HttpPost]
        public IActionResult NewItem(Item newItem)
        {
            newItem.ownershipId = currentOwnershipId;
            _itemDatabase.Items.Add(newItem);
            _itemDatabase.SaveChanges();

            return RedirectToAction("Index");
        }

        public IActionResult HomepageSearch()
        {
            return View();
        }

        // Edit item page
        [HttpGet]
        public IActionResult EditItem(int id)
        {
            // Get item to edit
            currentEditingId = id;
            Item editItem = _itemDatabase.Items.SingleOrDefault(x => x.id == id);

            // Item to edit not found
            if (editItem == null)
            {
                return RedirectToAction("Index");
            }

            return View(editItem);
        }

        // Edit item in database
        [HttpPost]
        public IActionResult EditItem(Item editedItem)
        {
            var oldItem = _itemDatabase.Items.SingleOrDefault(x => x.id == currentEditingId);

            // Item to edit not found
            if (oldItem == null)
            {
                return RedirectToAction("Index");
            }
            
            // Edit item with new properties
            oldItem.title = editedItem.title;
            oldItem.category = editedItem.category;
            oldItem.description = editedItem.description;
            _itemDatabase.SaveChanges();

            return RedirectToAction("Index");
        }

        // Search item in database
        [HttpPost]
        public IActionResult SearchItem(Search searchedItem)
        {  
            // Return if nothing was searched
            if (string.IsNullOrEmpty(searchedItem.searched))
                return RedirectToAction("Index");

            // Check category for result
            var result = _itemDatabase.Items
                .Where(x => x.category.ToLower().Contains(searchedItem.searched.ToLower()))
                .Where(x => x.ownershipId == currentOwnershipId)
                .OrderBy(x => x.category);

            // If nothing was found in category, check title
            if (result.Count() == 0)
            {
                result = _itemDatabase.Items
                    .Where(x => x.title.ToLower().Contains(searchedItem.searched.ToLower()))
                    .Where(x => x.ownershipId == currentOwnershipId)
                    .OrderBy(x => x.title);
            }

            // If nothing was found in title, check description
            if (result.Count() == 0)
            {
                result = _itemDatabase.Items
                    .Where(x => x.description.ToLower().Contains(searchedItem.searched.ToLower()))
                    .Where(x => x.ownershipId == currentOwnershipId)
                    .OrderBy(x => x.description);
            }

            // Set results for index
            searchResults = result;

            return RedirectToAction("Index");
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
