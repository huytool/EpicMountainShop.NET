using ASC.DataAccess.Interfaces;
using ASC.Models.BaseTypes;
using System;
using System.Collections.Generic;
using System.Text;

namespace ASC.Models.Models
{
    public class MasterDataValue :BaseEntity,IAuditTracker
    {
        public MasterDataValue() { }
        public MasterDataValue(string masterDataPartitionKey,string value)
        {
            this.PartitionKey = masterDataPartitionKey;
            this.RowKey = Guid.NewGuid().ToString();
        }
        public bool IsActive { get; set; }
        public string Name { get; set; }
    }
}
