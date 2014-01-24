﻿using Playground.Data.Contracts;
using Playground.Model;
using Playground.Web.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace Playground.Web.Controllers
{
    public abstract class ApiBaseController : ApiController
    {
        protected IPlaygroundUow Uow { get; set; }
    }
}