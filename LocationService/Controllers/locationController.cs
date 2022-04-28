using LocationService.App_Codes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace LocationService.Controllers
{
    public class locationController : ApiController
    {
        [HttpGet]
        public bool GetLocation()
        {
            _service svc = new _service();
            svc.CallLocationService();

            return true;
        }
    }
}