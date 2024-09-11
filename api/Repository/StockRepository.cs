using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Dtos.Stock;
using api.Helpers;
using api.Interfaces;
using api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.Repository
{
    public class StockRepository : IStockRepository
    {
        private readonly ApplicationDBContext _context;
        
        public StockRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<Stock> CreateStock(Stock stockModel)
        {
            await _context.AddAsync(stockModel);
            await _context.SaveChangesAsync();
            return stockModel;
        }

        public async Task<Stock?> DeleteStockAsync(int id)
        {
            var stockModel = await _context.Stock.FirstOrDefaultAsync(x => x.Id == id);

            if (stockModel == null){
                return null;
            }

            _context.Stock.Remove(stockModel);
            await _context.SaveChangesAsync();
            return stockModel;
        }

        public async Task<List<Stock>> GetAllStocksAsync(QueryObject query)
        {
            var stock = _context.Stock.Include(C=> C.Comments).ThenInclude(a => a.AppUser).AsQueryable();

            if(!string.IsNullOrWhiteSpace(query.CompanyName)){
                stock = stock.Where(x => x.CompanyName.Contains(query.CompanyName));
            }

            if(!string.IsNullOrWhiteSpace(query.Symbol)){
                stock = stock.Where(s => s.Symbol.Contains(query.Symbol));
            }

            if(!string.IsNullOrWhiteSpace(query.SortBy)){
                if(query.SortBy.Equals("Symbol", StringComparison.OrdinalIgnoreCase)){
                    stock = query.IsDescending ? stock.OrderByDescending(d => d.Symbol) : stock.OrderBy(d => d.Symbol);
                }
            }

            var skipNumber = (query.PageNumber - 1) * query.PageSize;

            return await stock.Skip(skipNumber).Take(query.PageSize).ToListAsync();

        }

        public async Task<Stock?> GetByIdAsync(int id)
        {
            var stockModel = await _context.Stock.Include(C=> C.Comments).FirstOrDefaultAsync(x => x.Id == id);

            if(stockModel == null){
                return null;
            }

            return stockModel;
        }

        public async Task<Stock?> GetBySymbolAsync(string symbol)
        {
            return await _context.Stock.FirstOrDefaultAsync(s => s.Symbol == symbol);
        }

        public Task<bool> StockExists(int id)
        {
            return _context.Stock.AnyAsync(s => s.Id == id);
        }

        public async Task<Stock?> UpdateStockAsync(int id, UpdateStockRequestDto stockDto)
        {
            var existingStock = await _context.Stock.FirstOrDefaultAsync(x => x.Id == id);

            if(existingStock == null){
                return null;
            }

            existingStock.Symbol = stockDto.Symbol;
            existingStock.Purchase = stockDto.Purchase;
            existingStock.CompanyName = stockDto.CompanyName;
            existingStock.LastDiv = stockDto.LastDiv;
            existingStock.MarketCap = stockDto.MarketCap;
            existingStock.Industry = stockDto.Industry;

            await _context.SaveChangesAsync();

            return existingStock;
        }
    }
}