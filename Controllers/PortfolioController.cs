using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Extensions;
using api.Interfaces;
using api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace api.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class PortfolioController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IStockRepository _stockRepo;
        private readonly IPortfolioRepository _portfolioRepo;
        public PortfolioController(UserManager<AppUser> userManager, IStockRepository stockRepo, IPortfolioRepository portfolioRepo)
        {
            _userManager = userManager;
            _stockRepo = stockRepo;
            _portfolioRepo = portfolioRepo;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAllPortfolio()
        {
            try
            {
                var username = User.GetUsername();
                var appUser = await _userManager.FindByNameAsync(username);
                if (appUser == null) return BadRequest("User not found");
                var userPortfolio = await _portfolioRepo.GetAllPortfolio(appUser);

                return Ok(userPortfolio);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreatePortfolio(string symbol)
        {
            try
            {
                var username = User.GetUsername();
                var appUser = await _userManager.FindByNameAsync(username);
                var stock = await _stockRepo.GetBySymbolAsync(symbol);

                if (stock == null || appUser == null) return BadRequest("Stock/User not found");

                var userPortfolio = await _portfolioRepo.GetAllPortfolio(appUser);

                if (userPortfolio.Any(s => s.Symbol.ToLower() == symbol.ToLower())) return BadRequest("Cannot add same stock to portfolio");

                var portfolioModel = new Portfolio
                {
                    AppUserId = appUser.Id,
                    StockId = stock.Id
                };

                await _portfolioRepo.CreatePortfolio(portfolioModel);

                if (portfolioModel == null)
                {
                    return StatusCode(500, "Could not create");
                }
                else
                {
                    return Created();
                }

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> DeletePortfolio(string symbol)
        {
            var user = User.GetUsername();
            var appUser = await _userManager.FindByNameAsync(user);
            if (appUser == null) return BadRequest("User not found");

            var userPortfolio = await _portfolioRepo.GetAllPortfolio(appUser);

            var filteredStock = userPortfolio.Where(s => s.Symbol.ToLower() == symbol.ToLower()).ToList();

            if (filteredStock.Count() == 1)
            {
                await _portfolioRepo.DeletePortfolio(appUser, symbol);
            }
            else
            {
                return BadRequest("Stock is not found on user");
            }

            return Ok();
        }
    }   
}