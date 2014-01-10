using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Playground.Business.Contracts
{
    
    public class ResponseObject<T> where T : class
    {
        public T Data { get; set; }
        public bool Sucess { get; set; }
        public string Message { get; set; }
    }
}
