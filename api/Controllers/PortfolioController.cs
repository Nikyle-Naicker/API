using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Extensions;
using api.Interfaces;
using api.Models;
using api.Repository;
using api.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace api.Controllers
{
    [Route("api/portfolio")]
    [ApiController]
    public class PortfolioController :  ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IStockRepository _stockRepo;
        private readonly IPortfolioRepository _portfolioRepo;
        private readonly IFMPService _fmpService;
        public PortfolioController(UserManager<AppUser> userManager, 
        IStockRepository stockRepo, IPortfolioRepository portfolioRepo, 
        IFMPService fmpService)
        {
            _userManager = userManager;
            _stockRepo = stockRepo;
            _portfolioRepo = portfolioRepo;
            _fmpService = fmpService;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetUserPortfolio()
        {
            var username = User.GetUsername();
            var AppUser = await _userManager.FindByNameAsync(username);
            var userPortfolio = await _portfolioRepo.GetUserPortfolio(AppUser);
            return Ok(userPortfolio);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddPortfolio(string symbol)
        {
            var username = User.GetUsername();
            var AppUser = await _userManager.FindByNameAsync(username);

            var stock = await _stockRepo.GetBySymbolAsync(symbol);

            if (stock == null)
            {
                stock = await _fmpService.FindStockBySymbolAsync(symbol);
                if (stock == null)
                {
                    return BadRequest("Stock does not exists");
                }
                else
                {
                    await _stockRepo.CreateStock(stock);
                }
            }

            if(stock == null)
            {
                return BadRequest("Stock not found");
            }

            var portfolio = await _portfolioRepo.GetUserPortfolio(AppUser);

            if(portfolio.Any(p => p.Symbol.ToLower() == symbol.ToLower()))
            {
                return BadRequest("Stock already added to portfolio");
            }

            var portfolioModel = new Portfolio
            {
                StockId = stock.Id,
                AppUserId = AppUser.Id,
            };

            await _portfolioRepo.CreatePortfolio(portfolioModel);

            if(portfolioModel == null)
            {
                return StatusCode(500, "Could not create portfolio");
            }

            return Created();            

        }

        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> DeletePortfolio(string symbol)
        {
            var username = User.GetUsername();
            var AppUser = await _userManager.FindByNameAsync(username);

            var userPortfolio = await _portfolioRepo.GetUserPortfolio(AppUser);

            var filteredstock = userPortfolio.Where(f => f.Symbol.ToLower() == symbol.ToLower());

            if(filteredstock.Count() > 0)
            {
                await _portfolioRepo.DeletePortfolio(AppUser, symbol);
                
            }
            else
            {
                return BadRequest("Stock not in your portfolio");
            }
            
            return Ok("Portfolio deleted Successfully");
            
        
        }

    }  

}