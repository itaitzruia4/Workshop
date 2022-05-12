using Backend.Communication.DTO;
using Backend.Communication.Requests;
using Backend.Communication.Responses;
using Microsoft.AspNetCore.Mvc;


namespace Backend.Communication.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthenticationController : ControllerBase
    {
        [HttpPost]
        public void Post([FromBody] LoginRequest request)
        {
            Console.WriteLine("Reached post method!!!!!!!!!!!!!");
            /*
            if (request.Username == "itai")
                return Ok(new AuthenticationResponse
                {
                    UserId = 1
                });
            else
                return BadRequest("Username must be itai");
            */
        }
    }
}
