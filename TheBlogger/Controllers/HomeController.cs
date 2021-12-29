﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TheBlogger.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Http;

namespace TheBlogger.Controllers
{
    public class HomeController : Controller
    {
        private readonly IPostRepository repo;
        private readonly UserManager<IdentityUser> _userManager;

        public HomeController(IPostRepository postRepository, UserManager<IdentityUser> userManager)
        {
            repo = postRepository;
            _userManager = userManager;
        }
        public IActionResult ChangeLanguage(string culture)
        {
            Response.Cookies.Append(CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions() { Expires = DateTimeOffset.UtcNow.AddYears(1) });

            return Redirect(Request.Headers["Referer"].ToString());
        }
        public IActionResult Index()
        {
            return View(repo.GetAllPosts());
        }

        [Route("{username}")]
        public async Task<IActionResult> PostsofUser(string username)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null) return NotFound("404 Sayfa Bulunamadı");
            var temp = repo.GetByUser(user);
            return View("Index", temp);
        }

        [Route("{username}/post/{id?}")]
        public IActionResult PostofUser(string username, int id)
        {
            if (repo.GetStatePost(id) != 0)
            {
                return View(repo.GetPost(id));
            }
            else
            {
                return NotFound("Post yayınlanmak için onaylanmadı... !");

            }
        }


    }
}