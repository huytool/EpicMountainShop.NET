using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Webb.Service
{
   public interface ISmsSender
    {
        public Task SendSmsAsync(string number, string message);
    }
}
