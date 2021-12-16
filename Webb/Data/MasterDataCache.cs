using ASC.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Webb.Data
{
    public class MasterDataCache
    {
        public List<MasterDataKey> Keys { get; set; }
        public List<MasterDataValue> Values { get; set; }
    }
}
