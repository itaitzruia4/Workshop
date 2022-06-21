﻿using System.Globalization;
using API.Requests;
using API.Responses;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using Workshop.ServiceLayer;
using Workshop.ServiceLayer.ServiceObjects;
using WebSocketSharp.Server;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthenticationController : ControllerBase
    {
        IService Service;
        WebSocketServer WsServer;
        static volatile int userId = 0;
        public AuthenticationController(IService service, WebSocketServer wsServer)
        {
            Service = service;
            WsServer = wsServer;
        }

        [HttpGet("entermarket")]
        public ActionResult<FrontResponse<int>> EnterMarket()
        {
            Response<User> response = Service.EnterMarket(userId, DateTime.Now.Date);
            if (response.ErrorOccured)
            {
                return BadRequest(new FrontResponse<int>(response.ErrorMessage));
            }
            int CURR_ID = userId++;
            foreach (string path in WsServer.WebSocketServices.Paths)
            {
                WsServer.WebSocketServices[path].Sessions.Broadcast(BitConverter.GetBytes(CURR_ID).Reverse().ToArray());
            }
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
            return Ok(new LoginResponse(response.Value.Key, response.Value.Value));
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