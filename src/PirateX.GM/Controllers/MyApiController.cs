using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace PirateX.GM.Controllers
{
    [RoutePrefix("s")]
    public class MyApiController : ApiController
    {
        [Route("/rewards/save")]
        public void Post()
        {
            
        }
    }
}
