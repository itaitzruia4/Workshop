using API.DTO;
using API.Requests;
using API.Responses;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Workshop.ServiceLayer;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthenticationController : ControllerBase
    {
        [HttpPost("login")]
        public ActionResult<AuthenticationResponse> Post([FromBody] LoginRequest request)
        {
            
            if (request.Username == "itai")
                return Ok(new AuthenticationResponse
                {
                    UserId = 8
                });
            else
                return BadRequest(new AuthenticationResponse {
                    UserId = -1,
                    Error = "Username must be itai"
                    }
                );
        }
    }
}
