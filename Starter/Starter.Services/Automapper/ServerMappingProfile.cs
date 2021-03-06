﻿using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using Starter.DAL.Entities;
using Starter.Services.Blocks.Models;
using Starter.Services.Transactions.Models;

namespace Starter.Services.Automapper
{
    public class ServerMappingProfile : Profile
    {
        public ServerMappingProfile()
        {
            CreateMap<TransactionEntity, TransactionModel>()
                .ForMember(x => x.Status, opt => opt.MapFrom(x => Enum.Parse<TransactionStatus>(x.State)));

            CreateMap<TransactionEntity, TransactionDetailedModel>()
                .ForMember(x => x.SentTime, opt => opt.MapFrom(x => x.Date))
                .ForMember(x => x.FromAccount, opt => opt.MapFrom(x => x.FromAccount.Id))
                .ForMember(x => x.ProcessedTime, opt => opt.MapFrom(x => x.Block.Date))
                .ForMember(x => x.ToAccount, opt => opt.MapFrom(x => x.ToAccount.Id))
                .ForMember(x => x.Status, opt => opt.MapFrom(x => Enum.Parse<TransactionStatus>(x.State)))
                .ForMember(x => x.BlockId, opt => opt.MapFrom(x => x.Block == null ? null : x.Block.BlockHash));

            CreateMap<BlockEntity, BlockModel>();

            CreateMap<BlockEntity, UnverifiedBlockModel>()
                .ForMember(x => x.PrevHash, opt => opt.MapFrom(x => x.PreviousBlockHash))
                .ForMember(x => x.Miner, opt => opt.MapFrom(x => x.Miner.Hash));
        }
    }
}