﻿using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using TrueMoney.Services.Interfaces;

namespace TrueMoney.Web.Controllers
{
    public class UserController : BaseController
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<ActionResult> Index()
        {
            return RedirectToAction("Details", new { id = await CurrentUserId() });
        }

        // нормальный метод
        public async Task<ActionResult> Details(int id)
        {
            var userModel = await _userService.GetDetails(await CurrentUserId(), id);

            return View(userModel);
        }
    }
}