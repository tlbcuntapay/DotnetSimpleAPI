using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.DTOs.Comment;
using api.Models;
using AutoMapper;

namespace api.Mappers
{
    public class CommentProfile : Profile
    {
        public CommentProfile()
        {
            CreateMap<Comment, CommentDto>()
                .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.AppUserId));
            CreateMap<CreateCommentRequest, Comment>();
        }
    } 
}