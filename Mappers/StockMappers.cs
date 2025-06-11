using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.DTOs.Stock;
using api.Models;
using AutoMapper;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace api.Mappers
{
    public class StockProfile : Profile
    {
        public StockProfile()
        {
            CreateMap<Stock, StockDto>();
            CreateMap<CreateStockRequest, Stock>();
            CreateMap<UpdateStockRequest, Stock>();
        }
    }
}