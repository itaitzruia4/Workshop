using API.Requests;
using API.Responses;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
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
        public ActionResult<FrontResponse<int>> Get()
        {
            Response<User> response = Service.EnterMarket(userId);
            if (response.ErrorOccured)
            {
                return BadRequest(new FrontResponse<int>(response.ErrorMessage));
            }
            return Ok(new FrontResponse<int>(userId++));
        }

        [HttpPost("exitmarket")]
        public ActionResult<FrontResponse<int>> Post([FromBody] BaseRequest request)
        {
            Response response = Service.ExitMarket(request.UserId);
            if (response.ErrorOccured)
            {
                return BadRequest(new FrontResponse<int>(response.ErrorMessage));
            }
            return Ok(new FrontResponse<int>(request.UserId));
        }

        [HttpPost("login")]
        public ActionResult<FrontResponse<Member>> Post([FromBody] AuthenticationRequest request)
        {
            Response<Member> response = Service.Login(request.UserId, request.Membername, request.Password);
            if (response.ErrorOccured)
            {
                return BadRequest(new FrontResponse<Member>(response.ErrorMessage));
            }
            return Ok(new FrontResponse<Member>(response.Value));
        }

        [HttpPost("logout")]
        public ActionResult<FrontResponse<int>> Post([FromBody] MemberRequest request)
        {
            Response response = Service.Logout(request.UserId, request.Membername);
            if (response.ErrorOccured)
            {
                return BadRequest(new FrontResponse<int>(response.ErrorMessage));
            }
            return Ok(new FrontResponse<int>(request.UserId));
        }

        [HttpPost("register")]
        public ActionResult<FrontResponse<int>> Post([FromBody] RegisterRequest request)
        {
            try
            {
                Response response = Service.Register(request.UserId, request.Membername, request.Password, DateTime.ParseExact(request.Birthdate, "dd/MM/yyyy", CultureInfo.InvariantCulture));
                if (response.ErrorOccured)
                {
                    return BadRequest(new FrontResponse<int>(response.ErrorMessage));
                }
                return Ok(new FrontResponse<int>(response.UserId));
            }
            catch (Exception _)
            {
                return BadRequest(new FrontResponse<int>("Bad date format in register request"));
            }
        }
    }
}