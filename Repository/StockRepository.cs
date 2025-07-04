using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.DTOs.Stock;
using api.Helpers;
using api.Interfaces;
using api.Mappers;
using api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace api.Repository
{
    public class StockRepository : IStockRepository
    {
        private readonly DBContext _context;
        public StockRepository(DBContext context)
        {
            _context = context;
        }

        public async Task<List<Stock>> GetAllStocksAsync(QueryObject query)
        {
            var stocks = _context.Stocks.Include(c => c.Comments).ThenInclude(a => a.AppUser).AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.CompanyName))
            {
                stocks = stocks.Where(s => s.CompanyName.Contains(query.CompanyName));
            }

            if (!string.IsNullOrWhiteSpace(query.Symbol))
            {
                stocks = stocks.Where(s => s.Symbol.Contains(query.Symbol));
            }

            if (!string.IsNullOrWhiteSpace(query.SortBy))
            {
                if (query.SortBy.Equals("Symbol", StringComparison.OrdinalIgnoreCase))
                {
                    stocks = query.IsDescending ? stocks.OrderByDescending(s => s.Symbol) : stocks.OrderBy(s => s.Symbol);
                }
            }

            var skipNumber = (query.PageNo - 1) * query.PageSize;
            var stocksPagination = await stocks.Skip(skipNumber).Take(query.PageSize).ToListAsync();

            foreach (var stock in stocksPagination)
            {
                stock.Comments = stock.Comments
                    .OrderByDescending(c => c.CreatedOn)
                    .Skip((query.CommentPageNo - 1) * query.CommentPageSize)
                    .Take(query.CommentPageSize)
                    .ToList();
            }
            
            return stocksPagination;
        }
        
        public async Task<Stock?> GetStockByIdAsync(int id)
        {
            return await _context.Stocks.Include(c => c.Comments).FirstOrDefaultAsync(i => i.Id == id);
        }
        public async Task<Stock> CreateStockAsync(Stock stock)
        {
            await _context.Stocks.AddAsync(stock);
            await _context.SaveChangesAsync();
            return stock;      
        }
        public async Task<Stock?> UpdateStockAsync(int id, UpdateStockRequest updateStockRequest)
        {
            var existingStock = await _context.Stocks.FindAsync(id);
            
            if (existingStock == null)
            {
                return null;
            }
            _context.Entry(existingStock).CurrentValues.SetValues(updateStockRequest);
            await _context.SaveChangesAsync();
            return existingStock;
        }
        public async Task<Stock?> DeleteStockAsnyc(int id)
        {
            var stock = await _context.Stocks.FirstOrDefaultAsync(s => s.Id == id);
            if (stock == null)
            {
                return null;
            }

            _context.Stocks.Remove(stock);
            await _context.SaveChangesAsync();
            return stock;
        }

        public async Task<bool> StockExists(int id)
        {
            return await _context.Stocks.AnyAsync(stock => stock.Id == id);
        }

        public async Task<Stock?> GetBySymbolAsync(string symbol)
        {
            return await _context.Stocks.FirstOrDefaultAsync(s => s.Symbol == symbol);
        }
  }
}