using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using Starter.DAL.Entities;
using Starter.Services.Transactions.Models;

namespace Starter.Services.Automapper
{
    public class ServerMappingProfile : Profile
    {
        public ServerMappingProfile()
        {
            CreateMap<TransactionEntity, TransactionModel>()
                .ForMember(x => x.Status, opt => opt.MapFrom(x => Enum.Parse<TransactionStatus>(x.State)));
        }
    }
}