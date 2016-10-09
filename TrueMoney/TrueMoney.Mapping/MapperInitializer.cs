﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;

namespace TrueMoney.Mapping
{
    public static class MapperInitializer
    {
        public static void Initialize()
        {
            Mapper.Initialize(
                conf =>
                {
                    conf.CreateMap<Data.Entities.User, Infrastructure.Entities.User>();
                });
        }
    }
}