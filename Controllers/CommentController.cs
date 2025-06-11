using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.DTOs.Comment;
using api.Interfaces;
using api.Mappers;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;

namespace api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CommentController : ControllerBase
    {
        private readonly ICommentRepository _commentRepo;
        private readonly IMapper _mapper;
        public CommentController(ICommentRepository commentRepository, IMapper mapper)
        {
            _mapper = mapper;
            _commentRepo = commentRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCommentsAsync()
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
        public async Task<IActionResult> GetCommentByIdAsync([FromRoute] int id)
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
    }
}