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

        private static List<Item> userItems;

        private static List<Item> searchedItems;

        public static bool isSearching = false;

        private static string currentOwnershipId;

        private static int currentEditingId;

        public HomeController(ILogger<HomeController> logger, ItemDatabase database)
        {
            _logger = logger;
            _itemDatabase = database;

            // Initialize lists
            if (userItems == null)
                userItems = new List<Item>();
            if (searchedItems == null)
                searchedItems = new List<Item>();
        }

        // Home page, displaying inventory
        [Authorize]
        public IActionResult Index()
        {

            // Set current owner ship id from user that logs in
            if (TempData.ContainsKey("loggedInUser"))
            {
                // Get the id for the user that links them to their items
                currentOwnershipId = TempData["loggedInUser"].ToString();
                // Retrieve the user's items
                refreshUserItems();
            }

            // Check if a search is happening
            if (isSearching)
            {
                isSearching = false;
                // Return searched items
                return View(searchedItems);
            }

            // Return all items
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

            refreshUserItems();

            return RedirectToAction("Index");
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
        public IActionResult EditItem(Item editedItem, string editButton, string deleteButton)
        {
            // Delete Item
            if (deleteButton != null)
            {
                var oldItem = _itemDatabase.Items.SingleOrDefault(x => x.id == currentEditingId);
                // Item to edit not found
                if (oldItem == null)
                {
                    return RedirectToAction("Index");
                }

                _itemDatabase.Items.Remove(oldItem);
                _itemDatabase.SaveChanges();

                refreshUserItems();
            }

            // Update edited item
            if (editButton != null)
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

                refreshUserItems();
            }

            return RedirectToAction("Index");
        }

        public IActionResult HomePageSearch()
        {
            return View();
        }

        // Search item in database
        [HttpPost]
        public IActionResult SearchItem(Search searchedItem)
        {  
            // Return if nothing was searched, or there is nothing to search
            if (string.IsNullOrEmpty(searchedItem.searched) || userItems.Count == 0)
                return RedirectToAction("Index");

            // Check category for result
            List<Item> result = userItems
                .Where(x => x.category.ToLower().Contains(searchedItem.searched.ToLower()))
                .OrderBy(x => x.category)
                .ToList();

            // If nothing was found in category, check title
            if (result.Count == 0)
            {
                result = userItems
                    .Where(x => x.title.ToLower().Contains(searchedItem.searched.ToLower()))
                    .OrderBy(x => x.title)
                    .ToList();
            }

            // If nothing was found in title, check description
            if (result.Count == 0)
            {
                result = userItems
                    .Where(x => x.description.ToLower().Contains(searchedItem.searched.ToLower()))
                    .OrderBy(x => x.description)
                    .ToList();
            }

            // Set searched items
            searchedItems = result;
            isSearching = true;

            return RedirectToAction("Index");
        }

        // Retrieve the user's items, must do after any database change
        private void refreshUserItems()
        {
            userItems = _itemDatabase.Items.Where(x => x.ownershipId == currentOwnershipId).ToList();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
