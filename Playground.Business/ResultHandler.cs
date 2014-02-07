using log4net;
using Playground.Business.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Playground.Business
{
    public static class ResultHandler<T> 
    {
        public static Result<T> Sucess(T data)
        {
            return new Result<T>()
            {
                Sucess = true,
                Data = data
            };
        }

        public static Result<T> Erorr(String message)
        {
            return new Result<T>()
            {
                Sucess = false,
                Data = default(T),
                Message = message
            };
        }
    }
}
