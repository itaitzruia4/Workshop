using API.Requests;
using API.Responses;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using WebSocketSharp.Server;
using Workshop.DomainLayer.Reviews;
using Workshop.ServiceLayer;
using Workshop.ServiceLayer.ServiceObjects;
using CreditCard = Workshop.DomainLayer.MarketPackage.CreditCard;
using SupplyAddress = Workshop.DomainLayer.MarketPackage.SupplyAddress;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserActionsController : ControllerBase
    {
        IService Service;
        private WebSocketServer adminDailyService;
        private class AdminDailyService : WebSocketBehavior
        {

        }

        public UserActionsController(IService service, WebSocketServer serv)
        {
            Service = service;
            adminDailyService = serv;
        }

        [HttpPost("takenotifications")]
        public ActionResult<FrontResponse<List<Notification>>> TakeNotifications([FromBody] MemberRequest request)
        {
            Response<List<Notification>> response = Service.TakeNotifications(request.UserId, request.Membername);
            if (response.ErrorOccured)
            {
                return BadRequest(new FrontResponse<List<Notification>>(response.ErrorMessage));
            }
            return Ok(new FrontResponse<List<Notification>>(response.Value));
        }

        [HttpPost("reviewproduct")]
        public ActionResult<FrontResponse<ReviewDTO>> ReviewProduct([FromBody] ProductReviewRequest request)
        {
            Response<ReviewDTO> response = Service.ReviewProduct(request.UserId, request.Membername, request.ProductId, request.Review, request.Rating);
            if (response.ErrorOccured)
            {
                return BadRequest(new FrontResponse<ReviewDTO>(response.ErrorMessage));
            }
            return Ok(new FrontResponse<ReviewDTO>(response.Value));
        }

        [HttpPost("searchproduct")]
        public ActionResult<FrontResponse<List<Product>>> SearchProduct([FromBody] ProductSearchRequest request)
        {
            Response<List<Product>> response = Service.SearchProduct(request.UserId, request.Keywords, request.Category, request.MinPrice, request.MaxPrice, request.ProductReview);
            if (response.ErrorOccured)
            {
                return BadRequest(new FrontResponse<List<Product>>(response.ErrorMessage));
            }
            return Ok(new FrontResponse<List<Product>>(response.Value));
        }

        [HttpPost("addtocart")]
        public ActionResult<FrontResponse<Product>> AddToCart([FromBody] AddToCartRequest request)
        {
            Response<Product> response = Service.AddToCart(request.UserId, request.ProductId, request.StoreId, request.Quantity);
            if (response.ErrorOccured)
            {
                return BadRequest(new FrontResponse<Product>(response.ErrorMessage));
            }
            return Ok(new FrontResponse<Product>(response.Value));
        }

        [HttpPost("viewcart")]
        public ActionResult<FrontResponse<ShoppingCart>> ViewCart([FromBody] BaseRequest request)
        {
            Response<ShoppingCart> response = Service.ViewCart(request.UserId);
            if (response.ErrorOccured)
            {
                return BadRequest(new FrontResponse<ShoppingCart>(response.ErrorMessage));
            }
            return Ok(new FrontResponse<ShoppingCart>(response.Value));
        }

        [HttpPost("editcart")]
        public ActionResult<FrontResponse<ShoppingCart>> EditCart([FromBody] EditCartRequest request)
        {
            Response<ShoppingCart> response = Service.EditCart(request.UserId, request.ProductId, request.Quantity);
            if (response.ErrorOccured)
            {
                return BadRequest(new FrontResponse<ShoppingCart>(response.ErrorMessage));
            }
            return Ok(new FrontResponse<ShoppingCart>(response.Value));
        }

        [HttpPost("buycart")]
        public ActionResult<FrontResponse<double>> BuyCart([FromBody] BuyCartRequest request)
        {
            Response<double> response = Service.BuyCart(request.UserId, new CreditCard(request.Card_number, request.Month, request.Year, request.Holder, request.Ccv, request.Id), new SupplyAddress(request.Name, request.Address, request.City, request.Country, request.Zip), DateTime.ParseExact(request.Date, "dd/MM/yyyy", CultureInfo.InvariantCulture));
            if (response.ErrorOccured)
            {
                return BadRequest(new FrontResponse<double>(response.ErrorMessage));
            }
            return Ok(new FrontResponse<double>(response.Value));
        }

        [HttpPost("cancelmember")]
        public ActionResult<FrontResponse<string>> CancelMember([FromBody] CancelMemberRequest request)
        {
            Response response = Service.CancelMember(request.UserId, request.Membername, request.MemberToCancel);
            if (response.ErrorOccured)
            {
                return BadRequest(new FrontResponse<string>(response.ErrorMessage));
            }
            return Ok(new FrontResponse<string>(request.MemberToCancel));
        }

        [HttpPost("getmembersonlinestats")]
        public ActionResult<FrontResponse<KeyValuePair<Member[], Member[]>>> GetMembersOnlineStats([FromBody] MemberRequest request)
        {
            Response<Dictionary<Member, bool>> response = Service.GetMembersOnlineStats(request.UserId, request.Membername);
            if (response.ErrorOccured)
            {
                return BadRequest(new FrontResponse<KeyValuePair<Member[], Member[]>>(response.ErrorMessage));
            }
            Member[] onlineMembers = response.Value.Where((KeyValuePair<Member, bool> curr) => curr.Value).Select((KeyValuePair<Member, bool> curr) => curr.Key).ToArray();
            Member[] offlineMembers = response.Value.Where((KeyValuePair<Member, bool> curr) => !curr.Value).Select((KeyValuePair<Member, bool> curr) => curr.Key).ToArray();
            return Ok(new FrontResponse<KeyValuePair<Member[], Member[]>>(new KeyValuePair<Member[], Member[]>(onlineMembers, offlineMembers)));
        }

        [HttpPost("getmemberpermissions")]
        public ActionResult<FrontResponse<List<PermissionInformation>>> GetMemberPermissions([FromBody] MemberRequest request)
        {
            Response<List<PermissionInformation>> response = Service.GetMemberPermissions(request.UserId, request.Membername);
            if (response.ErrorOccured)
            {
                return BadRequest(new FrontResponse<List<PermissionInformation>>(response.ErrorMessage));
            }
            return Ok(new FrontResponse<List<PermissionInformation>>(response.Value));
        }

        [HttpPost("getdailyincomemarketmanager")]
        public ActionResult<FrontResponse<double>> GetDailyIncomeMarketManager([FromBody] MemberRequest request)
        {
            Response<double> response = Service.GetDailyIncomeMarketManager(request.UserId, request.Membername);
            if (response.ErrorOccured)
            {
                return BadRequest(new FrontResponse<double>(response.ErrorMessage));
            }
            return Ok(new FrontResponse<double>(response.Value));
        }

        [HttpPost("marketmanagerdaily")]
        public ActionResult<FrontResponse<int>> MarketManagerDailyStatistics([FromBody] MemberRequest request)
        {
            string relativeServicePath = "/" + request.Membername + "-live_view";
            try
            {
                if (adminDailyService.WebSocketServices[relativeServicePath] == null)
                    adminDailyService.AddWebSocketService<AdminDailyService>(relativeServicePath);
            }
            catch
            {
                return BadRequest(new FrontResponse<int>("Sorry, but it seems that we cant connect you"));
            }
            return Ok(new FrontResponse<int>(16666)); // TODO REMOVE
            
            // in case the client tries to login again

            /*Func<string[], bool> notifier = (msgs) =>
            {
                string username = request.Membername;

                // Try send, if not - add to unsent messages
                if (adminDailyService.WebSocketServices[relativeServicePath] == null || adminDailyService.WebSocketServices[relativeServicePath].Sessions.Count < 1)
                {
                    IList<string> unsentMsgs = new List<string>();
                    if (buyerUnsentMessages.ContainsKey(username))
                        unsentMsgs = buyerUnsentMessages[username];

                    // Adding new unsent messages
                    foreach (string msg in msgs)
                        unsentMsgs.Add(msg);

                    //this.buyerUnsentMessages[username] = unsentMsgs;
                    AddUnsentMessage(username, unsentMsgs);
                    return true; // So msgs delete on member msgs queue
                }

                foreach (string msg in msgs)
                    notificationServer.WebSocketServices[relativeServicePath].Sessions.Broadcast(msg);
                return true;
            };
            Response<int> response = buyerFacade.Login(request.UserName, request.Password, notifier);

            if (response.IsErrorOccured())
            {
                notificationServer.RemoveWebSocketService(relativeServicePath);
                return BadRequest(response);
            }

            buyerIdToRelativeNotificationPath.Add(response.Value, relativeServicePath);
            return Ok(response);*/
        }
    }
}
