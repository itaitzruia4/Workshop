using API.DTO;
using API.Requests;
using API.Responses;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Workshop.ServiceLayer;
using Workshop.ServiceLayer.ServiceObjects;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StoreController : ControllerBase
    {
        IService Service;
        public StoreController(IService service)
        {
            Service = service;
        }
    }
}

