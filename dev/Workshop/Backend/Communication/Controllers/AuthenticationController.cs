using Backend.Communication.DTO;
using Microsoft.AspNetCore.Mvc;
using Backend.ServiceLayer;


namespace Backend.Communication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        public AuthenticationController(Service service)
        {

        }

        [HttpPost]
        public AuthenticationResult Post([FromBody] string value)
        {
            return null;
        }
    }
}
