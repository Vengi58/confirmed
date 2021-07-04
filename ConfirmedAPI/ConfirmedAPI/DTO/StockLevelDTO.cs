using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace ConfirmedAPI.DTO
{
    public class StockLevelDTO
    {
        public int IN_STOCK { get; set; }
        public int RESERVED { get; set; }
        public int SOLD { get; set; }
    }
}
