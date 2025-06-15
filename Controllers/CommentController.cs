using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.DTOs.Comment;
using api.Extensions;
using api.Interfaces;
using api.Mappers;
using api.Models;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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
        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;
        public CommentController(ICommentRepository commentRepo, IStockRepository stockRepo, UserManager<AppUser> userManager, IMapper mapper)
        {
            _mapper = mapper;
            _stockRepo = stockRepo;
            _commentRepo = commentRepo;
            _userManager = userManager;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAllComments()
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

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

        [HttpGet("{id:int}")]
        [Authorize]
        public async Task<IActionResult> GetCommentById([FromRoute] int id)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

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

        [HttpPost("{stockId:int}")]
        [Authorize]
        public async Task<IActionResult> CreateComment([FromRoute] int stockId, [FromBody] CreateCommentRequest createCommentDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                // Check if the Stock Exists
                if (!await _stockRepo.StockExists(stockId))
                {
                    return BadRequest($"Stock does not found");
                }

                var username = User.GetUsername();
                var appUser = await _userManager.FindByNameAsync(username);

                var comment = _mapper.Map<Comment>(createCommentDto);
                comment.StockId = stockId;
                comment.AppUserId = appUser.Id;

                await _commentRepo.CreateCommentAsync(comment);
                return CreatedAtAction(nameof(GetCommentById), new { id = comment.Id }, _mapper.Map<CommentDto>(comment));
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut]
        [Route("{id:int}")]
        public async Task<IActionResult> UpdateComment([FromRoute] int id, [FromBody] UpdateCommentRequest updateCommentRequest)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var comment = await _commentRepo.UpdateCommentAsync(id, updateCommentRequest);

                if (comment == null)
                {
                    return NotFound($"Values with Id of: {id} was not found.");
                }

                var commentDto = _mapper.Map<CommentDto>(comment);
                return Ok(commentDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete]
        [Route("{id:int}")]
        public async Task<IActionResult> DeleteComment(int id)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var comment = await _commentRepo.DeleteCommentAsync(id);

                if (comment == null)
                {
                    return NotFound($"Comment does not exist");
                }

                var commentDto = _mapper.Map<CommentDto>(comment);
                return Ok(commentDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}