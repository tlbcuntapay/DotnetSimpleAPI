using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.DTOs.Comment;
using api.Interfaces;
using api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.Repository
{
    public class CommentRepository : ICommentRepository
    {
        private readonly DBContext _context;
        public CommentRepository(DBContext context)
        {
            _context = context;
        }

        public async Task<List<Comment>> GetAllCommentsAsync()
        {
            return await _context.Comments.Include(a => a.AppUser).ToListAsync();
        }

        public async Task<Comment?> GetCommentByIdAsync(int id)
        {
            var comment = await _context.Comments.Include(a => a.AppUser).FirstOrDefaultAsync(c => c.Id == id);
            if (comment == null)
            {
                return null;
            }

            return comment;
        }

        public async Task<Comment> CreateCommentAsync(Comment comment)
        {
            await _context.Comments.AddAsync(comment);
            await _context.SaveChangesAsync();
            return comment;
        }

        public async Task<Comment?> UpdateCommentAsync(int id, UpdateCommentRequest updateCommentRequest)
        {
            var existingComment = await _context.Comments.FindAsync(id);

            if (existingComment == null)
            {
                return null;
            }

            _context.Entry(existingComment).CurrentValues.SetValues(updateCommentRequest);
            await _context.SaveChangesAsync();
            return existingComment;
        }

        public async Task<Comment?> DeleteCommentAsync(int id)
        {
            var existingComment = await _context.Comments.FindAsync(id);

            if (existingComment == null)
            {
                return null;
            }

            _context.Comments.Remove(existingComment);
            await _context.SaveChangesAsync();

            return existingComment;
        }
  }
}