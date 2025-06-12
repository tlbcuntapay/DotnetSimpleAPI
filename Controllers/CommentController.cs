using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.DTOs.Comment;
using api.Interfaces;
using api.Mappers;
using api.Models;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Identity.Client;

namespace api.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class CommentController : ControllerBase
    {
        private readonly ICommentRepository _commentRepo;
        private readonly IStockRepository _stockRepo;
        private readonly IMapper _mapper;
        public CommentController(ICommentRepository commentRepo, IStockRepository stockRepo, IMapper mapper)
        {
            _mapper = mapper;
            _stockRepo = stockRepo;
            _commentRepo = commentRepo;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllComments()
        {
            try
            {
                var comments = await _commentRepo.GetAllCommentsAsync();

                if (comments == null || comments.Count == 0)
                {
                    return NotFound("No comments found.");
                }

                var commentDto = _mapper.Map<List<CommentDto>>(comments);
                return Ok(commentDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCommentById([FromRoute] int id)
        {
            try
            {
                var comment = await _commentRepo.GetCommentByIdAsync(id);

                if (comment == null)
                {
                    return NotFound();
                }
                var commentDto = _mapper.Map<CommentDto>(comment);
                return Ok(commentDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("{stockId}")]
        public async Task<IActionResult> CreateComment([FromRoute] int stockId, [FromBody] CreateCommentDto createCommentDto)
        {
            try
            {
                // Check if the Stock Exists
                if (!await _stockRepo.StockExists(stockId))
                {
                    return BadRequest($"Stock does not found");
                }

                var comment = _mapper.Map<Comment>(createCommentDto);
                comment.StockId = stockId;

                await _commentRepo.CreateCommentAsync(comment);
                return CreatedAtAction(nameof(GetCommentById), new { id = comment }, _mapper.Map<CommentDto>(comment));
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> UpdateComment([FromRoute] int id, [FromBody] CommentDto commentDto)
        {
            try
            {
                var comment = await _commentRepo.UpdateCommentAsync(id, commentDto);

                if (comment == null)
                {
                    return NotFound($"Values with Id of: {id} was not found.");
                }

                return Ok(comment);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}