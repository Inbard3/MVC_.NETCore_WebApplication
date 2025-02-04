﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RateTheRest.Data;
using RateTheRest.Models;
using RateTheRest.Additional;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace RateTheRest.Controllers
{
    public class ReviewsController : Controller
    {
        private readonly ApplicationContext _dbcontext;                 //Working with db

        public ReviewsController(ApplicationContext context) { _dbcontext = context; }

        //_____________________________________________Actions Functions___________________________________________________________________________________

        // GET: Reviews -> User's MyReviwes Page
        public async Task<IActionResult> Index(string username)
        {
            if (username == null) return NotFound();

            var user = _dbcontext.Users
                    .Where(u => u.UserName == username)
                    .Include(u => u.Reviews)
                    .FirstOrDefault();

            if (user.Reviews == null)
                ViewData["UserReviews"] = new List<Review>();
            else
                ViewData["UserReviews"] = user.Reviews.ToList();

            return View(await _dbcontext.Reviews
                    .Include(r => r.Restaurant)
                    .Include(r => r.User)
                    .ToListAsync());
        }

        //Partial View! - Show the Review in Restaurant Details page
        // GET: Reviews/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var review = await _dbcontext.Reviews
                    .Include(r => r.Restaurant)
                    .Include(r => r.User)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(m => m.ReviewID == id);

            if (review == null) return NotFound();

            return PartialView("DetailsPartial");
        }
        //_________________________________________________________

        //Partial View! - Create a Review in Restaurant Details page
        // GET: Reviews/CreatePartial
        //[Authorize] - Manually
        public IActionResult CreatePartial()
        {
            if (!User.Identity.IsAuthenticated)
                return PartialView("_AuthenticationRequiredPartial");

            return PartialView("CreatePartial");
        }

        // POST: Reviews/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create([Bind(nameof(Review.ReviewID), nameof(Review.Text))] Review review, int score, int restaurantId)
        {
            review.DateCreated = DateTime.Now;
            review.Score = score;
            review.Restaurant = await _dbcontext.Restaurants          //Include db tables
                .Include(r => r.Location)
                .Include(r => r.OpeningHours)
                .Include(r => r.Tags)
                .Include(r => r.Logo)
                .Include(r => r.Photos)
                .Include(r => r.Rating).ThenInclude(r => r.Users)
                .Include(r => r.Reviews).ThenInclude(r => r.User)
                .Include(r => r.Chefs)
                .FirstOrDefaultAsync(m => m.RestaurantID == restaurantId);
            review.User = _dbcontext.Users
                .Include(u => u.Rating).ThenInclude(r => r.Restaurant)
                .FirstOrDefault(u => u.UserName == User.Identity.Name);

            _dbcontext.Add(review);
            _dbcontext.SaveChanges();

            //Update Retaurant's Rating
            review.Restaurant.Rating.Users.Add(review.User);
            review.Restaurant.Rating.Score = review.Restaurant.Rating.calcScore();
            await _dbcontext.SaveChangesAsync();

            return RedirectToAction("Details", "Restaurants", new { id = restaurantId });
        }
        //_________________________________________________________

        //Form - Delete a Review in user's reviews page
        // POST: Reviews/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        //[Authorize] - No need 
        public async Task<IActionResult> Delete(int id)
        {
            var review = _dbcontext.Reviews
                .Include(r => r.Restaurant)
                .FirstOrDefault(m => m.ReviewID == id);

            var restaurant = await _dbcontext.Restaurants          //Include db tables
                .Include(r => r.Location)
                .Include(r => r.OpeningHours)
                .Include(r => r.Tags)
                .Include(r => r.Logo)
                .Include(r => r.Photos)
                .Include(r => r.Rating).ThenInclude(r => r.Users)
                .Include(r => r.Reviews).ThenInclude(r => r.User)
                .Include(r => r.Chefs)
                .FirstOrDefaultAsync(m => m.RestaurantID == review.Restaurant.RestaurantID);

            var user = _dbcontext.Users
                .Include(u => u.Rating).ThenInclude(r => r.Restaurant)
                .FirstOrDefault(u => u.UserName == User.Identity.Name);

            restaurant.Reviews.Remove(review);
            user.Reviews.Remove(review);
            _dbcontext.SaveChanges();

            restaurant.Rating.Score = restaurant.Rating.calcScore();
            _dbcontext.SaveChanges();

            return RedirectToAction("Index", new { username = User.Identity.Name });
        }
        //_________________________________________________________________________________________________________________________________________________



        //_____________________________________________Additional Functions___________________________________________________________________________________

        public void UpdateRating(Review review, string action = "add")
        {
            var restaurant = review.Restaurant;
            var user = review.User;
            Rating rating = restaurant.Rating;
            var chefs = _dbcontext.Chefs.Where(c => c.Restaurants.Contains(restaurant)).ToList();
        }


































        //_________________________________________________________________________________________________________________________________________________

        //_________________________________NOT IN USE__________________________________________________________________
        // GET: Reviews
        //public async Task<IActionResult> Index()
        //{
        //    return View(await _dbcontext.Reviews
        //            .Include(r => r.Restaurant)
        //            .Include(r => r.User)
        //            .ToListAsync());
        //}

        //// GET: Reviews/Details/5
        //public async Task<IActionResult> Details(int? id)
        //{
        //    if (id == null) return NotFound();

        //    var review = await _dbcontext.Reviews
        //            .Include(r => r.Restaurant)
        //            .Include(r => r.User)
        //            .AsNoTracking()
        //            .FirstOrDefaultAsync(m => m.ReviewID == id);

        //    if (review == null) return NotFound();

        //    return View(review);
        //}

        // GET: Reviews/Create
        //[Authorize]
        //public IActionResult Create()
        //{
        //    //ViewData["restaurantId"] = _restaurant.RestaurantID;
        //    return View();
        //}
        //// GET: Reviews/Edit/5
        //public async Task<IActionResult> Edit(int? id)
        //{
        //    return View();
        //}

        //// POST: Reviews/Edit/5
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, [Bind(nameof(Review.ReviewID), nameof(Review.Score), nameof(Review.Text))] Review review)
        //{
        //    try
        //    {
        //        return RedirectToAction(nameof(Index));
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}
        //_________________________________________________________

        // GET: Reviews/Delete/5
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    return View();
        //}

        //// POST: Reviews/Delete/5
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    try
        //    {
        //        return RedirectToAction(nameof(Index));
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}
        //_________________________________________________________
    }
}
