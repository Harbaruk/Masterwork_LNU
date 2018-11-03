using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using Starter.DAL.Entities;

namespace Starter.Services.Mapper
{
    public class UserModelMapper
    {
        public Guid Id { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
    }

    /// <summary>
    /// Example for automapper profile
    /// </summary>
    /// <see href="https://dotnetcoretutorials.com/2017/09/23/using-automapper-asp-net-core/"/>
    public class UserModelProfile : Profile
    {
        public UserModelProfile()
        {
            CreateMap<UserEntity, UserModelMapper>();
        }
    }
}