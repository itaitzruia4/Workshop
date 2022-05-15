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
            if (response.ErrorOccured)
            {
                return BadRequest(new AuthenticationResponse
                {
                    UserId = response.UserId,
                    Error = response.ErrorMessage
                });
            }
            return Ok(new AuthenticationResponse
            {
                UserId = response.UserId,
                Error = ""
            });
        }

        [HttpPost("register")]
        public ActionResult<AuthenticationResponse> Post([FromBody] RegisterRequest request)
        {
            try
            {
                Response response = service.Register(request.UserId, request.Membername, request.Password, DateTime.Parse(request.Birthdate));
                if (response.ErrorOccured)
                {
                    return BadRequest(new AuthenticationResponse
                    {
                        UserId = response.UserId,
                        Error = response.ErrorMessage
                    });
                }
                return Ok(new AuthenticationResponse
                {
                    UserId = response.UserId,
                    Error = ""
                });
            }
            catch (Exception _)
            {
                return BadRequest(new AuthenticationResponse
                    {
                        UserId = request.UserId,
                        Error = "Bad date format in register request"
                    });
                }
            }
        }

    }
}
