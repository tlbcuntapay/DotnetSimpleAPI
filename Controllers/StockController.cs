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
using api.Repository;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace api.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class StockController : ControllerBase
    {
        private readonly DBContext _context;
        private readonly IMapper _mapper;
        private readonly IStockRepository _stockRepo;
        public StockController(DBContext context, IStockRepository stockRepo, IMapper mapper)
        {
            _mapper = mapper;
            _stockRepo = stockRepo;
            _context = context;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAllStock([FromQuery] QueryObject query)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var stocks = await _stockRepo.GetAllStocksAsync(query);
                var stockDto = _mapper.Map<List<StockDto>>(stocks);
                return Ok(stockDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetStockById([FromRoute] int id)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var stock = await _stockRepo.GetStockByIdAsync(id);

                if (stock == null)
                {
                    return NotFound($"Stock with ID {id} is not found.");
                }

                var stockDto = _mapper.Map<StockDto>(stock);
                return Ok(stockDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateStock([FromBody] CreateStockRequest stockDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var newStock = _mapper.Map<Stock>(stockDto);
                await _stockRepo.CreateStockAsync(newStock);
                return CreatedAtAction(nameof(GetStockById), new { id = newStock }, _mapper.Map<StockDto>(newStock));
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut]
        [Route("{id:int}")]
        public async Task<IActionResult> UpdateStock([FromRoute] int id, [FromBody] UpdateStockRequest updateStockRequest)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var stock = await _stockRepo.UpdateStockAsync(id, updateStockRequest);

                if (stock == null)
                {
                    return NotFound($"Stock with ID {id} is not found.");
                }

                var stockSto = _mapper.Map<StockDto>(stock);
                return Ok(stockSto);

            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete]
        [Route("{id:int}")]
        public async Task<IActionResult> DeleteStock(int id)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var stock = await _stockRepo.DeleteStockAsnyc(id);

                if (stock == null)
                {
                    return NotFound($"Stock with ID {id} is not found.");
                }
                
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

    }
}