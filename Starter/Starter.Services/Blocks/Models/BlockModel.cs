﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Starter.Services.Blocks.Models
{
    public class BlockModel
    {
        public string Hash { get; set; }

        public string Miner { get; set; }

        public DateTimeOffset Date { get; set; }
    }
}