using System.Globalization;
using API.Requests;
using API.Responses;
using Microsoft.AspNetCore.Mvc;
using Workshop.ServiceLayer;
using Workshop.ServiceLayer.ServiceObjects;
using WebSocketSharp.Server;
using System.Text;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthenticationController : ControllerBase
    {
        IService Service;
        StaisticsViewingServer StatsServer;
        static int userId = 0;
        public AuthenticationController(IService service, StaisticsViewingServer statsServer)
        {
            Service = service;
            StatsServer = statsServer;
        }

        [HttpGet("entermarket")]
        public ActionResult<FrontResponse<int>> EnterMarket()
        {
            int CURR_ID = Interlocked.Increment(ref userId);
            Response<User> response = Service.EnterMarket(CURR_ID, DateTime.Now.Date);
            if (response.ErrorOccured)
            {
                return BadRequest(new FrontResponse<int>(response.ErrorMessage));
            }
            StatsServer.SendMessageToAllAdmins("GUEST");
            return Ok(new FrontResponse<int>(CURR_ID));
        }

        [HttpPost("exitmarket")]
        public ActionResult<FrontResponse<int>> ExitMarket([FromBody] BaseRequest request)
        {
            Response response = Service.ExitMarket(request.UserId);
            if (response.ErrorOccured)
            {
                return BadRequest(new FrontResponse<int>(response.ErrorMessage));
            }
            return Ok(new FrontResponse<int>(request.UserId));
        }

        [HttpPost("login")]
        public ActionResult<LoginResponse> Login([FromBody] AuthenticationRequest request)
        {
            Response<KeyValuePair<Member, List<Notification>>> response = Service.Login(request.UserId, request.Membername, request.Password, DateTime.Now.Date);
            if (response.ErrorOccured)
            {
                return BadRequest(new LoginResponse(response.ErrorMessage));
            }
            LoginResponse res = new LoginResponse(response.Value.Key, response.Value.Value);
            bool MANAGER = false;
            bool OWNER = false;
            bool MARKET = false;
            string message = "MEMBER";
            foreach (Role role in res.Value.Roles)
            {
                if (!MARKET && role is MarketManager)
                {
                    MARKET = true;
                }
                else if (!OWNER && (role is StoreOwner || role is StoreFounder))
                {
                    OWNER = true;
                }
                else if (!MANAGER && role is StoreManager)
                {
                    MANAGER = true;
                }
            }
            if (MARKET) message = "MARKET";
            else if (OWNER) message = "OWNER";
            else if (MANAGER) message = "MANAGER";
            StatsServer.SendMessageToAllAdmins(message);
            return Ok(res);
        }

        [HttpPost("logout")]
        public ActionResult<FrontResponse<int>> Logout([FromBody] MemberRequest request)
        {
            Response response = Service.Logout(request.UserId, request.Membername);
            if (response.ErrorOccured)
            {
                return BadRequest(new FrontResponse<int>(response.ErrorMessage));
            }
            return Ok(new FrontResponse<int>(request.UserId));
        }

        [HttpPost("register")]
        public ActionResult<FrontResponse<int>> Register([FromBody] RegisterRequest request)
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