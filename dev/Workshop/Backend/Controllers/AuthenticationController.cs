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
    public class AuthenticationController : ControllerBase
    {
        IService service;
        AuthenticationController()
        {
            service = new Service();
        }

        [HttpPost("login")]
        public ActionResult<AuthenticationResponse> Post([FromBody] LoginRequest request)
        {
            Response<Member> response = service.Login(request.UserId, request.Membername, request.Password);
            return BadRequest(new AuthenticationResponse
            {
                UserId = -1,
                Error = "Username must be itai"
            }
);
        }

        [HttpPost("register")]
        public ActionResult<AuthenticationResponse> Post([FromBody] RegisterRequest request)
        {

            if (request.Membername == "itai")
                return Ok(new AuthenticationResponse
                {
                    UserId = 8
                });
            else
                return BadRequest(new AuthenticationResponse
                {
                    UserId = -1,
                    Error = "Username must be itai"
                }
                );
        }

    }
}
