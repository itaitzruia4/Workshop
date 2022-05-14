using Backend.Communication.DTO;
using Backend.Communication.Requests;
using Backend.Communication.Responses;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;


namespace Backend.Communication.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthenticationController : ControllerBase
    {
        [EnableCors("LiberalPolicy")]
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
