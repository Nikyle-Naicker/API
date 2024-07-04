using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Dtos.Stock;
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

        public async Task<Stock> CreateStockAsync(Stock stockModel)
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

        public async Task<List<Stock>> GetAllStocksAsync()
        {
            return await _context.Stock.Include(C=> C.Comments).ToListAsync();
        }

        public async Task<Stock?> GetByIdAsync(int id)
        {
            var stockModel = await _context.Stock.Include(C=> C.Comments).FirstOrDefaultAsync(x => x.Id == id);

            if(stockModel == null){
                return null;
            }

            return stockModel;
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