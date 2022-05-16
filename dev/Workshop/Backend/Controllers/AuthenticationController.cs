using API.Requests;
using API.Responses;
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
                return BadRequest(new AuthenticationResponse(response.ErrorMessage));
            }
            return Ok(new AuthenticationResponse(userId++));
        }

        [HttpPost("exitmarket")]
        public ActionResult<AuthenticationResponse> Post([FromBody] GuestRequest request)
        {
            Response response = Service.ExitMarket(request.UserId);
            if (response.ErrorOccured)
            {
                return BadRequest(new AuthenticationResponse(response.ErrorMessage));
            }
            return Ok(new AuthenticationResponse(request.UserId));
        }

        [HttpPost("login")]
        public ActionResult<MemberResponse> Post([FromBody] LoginRequest request)
        {
            Response<Member> response = Service.Login(request.UserId, request.Membername, request.Password);
            if (response.ErrorOccured)
            {
                return BadRequest(new MemberResponse(response.ErrorMessage));
            }
            return Ok(new MemberResponse(response.Value));
        }

        [HttpPost("logout")]
        public ActionResult<AuthenticationResponse> Post([FromBody] LogoutRequest request)
        {
            Response response = Service.Logout(request.UserId, request.Membername);
            if (response.ErrorOccured)
            {
                return BadRequest(new AuthenticationResponse(response.ErrorMessage));
            }
            return Ok(new AuthenticationResponse(request.UserId));
        }

        [HttpPost("register")]
        public ActionResult<AuthenticationResponse> Post([FromBody] RegisterRequest request)
        {
            try
            {
                Response response = Service.Register(request.UserId, request.Membername, request.Password, DateTime.Parse(request.Birthdate));
                if (response.ErrorOccured)
                {
                    return BadRequest(new AuthenticationResponse(response.ErrorMessage));
                }
                return Ok(new AuthenticationResponse(response.UserId));
            }
            catch (Exception _)
            {
                return BadRequest(new AuthenticationResponse("Bad date format in register request"));
            }
        }
    }
}