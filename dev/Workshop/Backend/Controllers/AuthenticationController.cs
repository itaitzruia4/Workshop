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
        IService Service;
        int userId;
        public AuthenticationController(IService service)
        {
            Service = service;
            userId = 0;
        }

        [HttpGet("entermarket")]
        public ActionResult<AuthenticationResponse> Get()
        {
            Response<User> response = Service.EnterMarket(userId);
            if (response.ErrorOccured)
            {
                return BadRequest(new AuthenticationResponse { Error = response.ErrorMessage });
            }
            return Ok(new AuthenticationResponse { UserId = userId++ });
        }

        [HttpPost("exitmarket")]
        public ActionResult<AuthenticationResponse> Post([FromBody] ExitMarketRequest request)
        {
            Response response = Service.ExitMarket(request.UserId);
            if (response.ErrorOccured)
            {
                return BadRequest(new AuthenticationResponse { Error = response.ErrorMessage });
            }
            return Ok(new AuthenticationResponse { UserId = request.UserId });
        }

        [HttpPost("login")]
        public ActionResult<AuthenticationResponse> Post([FromBody] LoginRequest request)
        {
            Response<Member> response = Service.Login(request.UserId, request.Membername, request.Password);
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
            });
        }

        [HttpPost("logout")]
        public ActionResult<AuthenticationResponse> Post([FromBody] LogoutRequest request)
        {
            Response response = Service.Logout(request.UserId, request.Membername);
            if (response.ErrorOccured)
            {
                return BadRequest(new AuthenticationResponse { Error = response.ErrorMessage });
            }
            return Ok(new AuthenticationResponse { UserId = request.UserId });
        }


        [HttpPost("register")]
        public ActionResult<AuthenticationResponse> Post([FromBody] RegisterRequest request)
        {
            try
            {
                Response response = Service.Register(request.UserId, request.Membername, request.Password, DateTime.Parse(request.Birthdate));
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