using System.Globalization;
using API.Requests;
using API.Responses;
using Microsoft.AspNetCore.Mvc;
using Workshop.ServiceLayer;
using Workshop.ServiceLayer.ServiceObjects;
using CreditCard = Workshop.DomainLayer.MarketPackage.CreditCard;
using SupplyAddress = Workshop.DomainLayer.MarketPackage.SupplyAddress;

namespace API.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class BidController : ControllerBase
    {
        IService Service;
        public BidController(IService service)
        {
            Service = service;
        }

        [HttpPost("offerbid")]
        public ActionResult<FrontResponse<Bid>> OfferBid([FromBody] OfferBidRequest request)
        {
            Response<Bid> response = Service.OfferBid(request.UserId, request.Membername, request.StoreId, request.ProductId, request.Price);
            if (response.ErrorOccured)
            {
                return BadRequest(new FrontResponse<Bid>(response.ErrorMessage));
            }
            return Ok(new FrontResponse<Bid>(response.Value));
        }

        [HttpPost("counterbid")]
        public ActionResult<FrontResponse<Bid>> CounterBid([FromBody] CounterBidRequest request)
        {
            Response<Bid> response = Service.CounterBid(request.UserId, request.Membername, request.StoreId, request.BidId, request.Price);
            if (response.ErrorOccured)
            {
                return BadRequest(new FrontResponse<Bid>(response.ErrorMessage));
            }
            return Ok(new FrontResponse<Bid>(response.Value));
        }

        [HttpPost("voteforbid")]
        public ActionResult<FrontResponse<Bid>> VoteForBid([FromBody] VoteForBidRequest request)
        {
            Response<Bid> response = Service.VoteForBid(request.UserId, request.Membername, request.StoreId, request.BidId, request.ToAccept);
            if (response.ErrorOccured)
            {
                return BadRequest(new FrontResponse<Bid>(response.ErrorMessage));
            }
            return Ok(new FrontResponse<Bid>(response.Value));
        }

        [HttpPost("buybidproduct")]
        public ActionResult<FrontResponse<double>> BuyBidProduct([FromBody] BuyBidRequest request)
        {
            Response<double> response = Service.BuyBidProduct(request.UserId, request.Membername, request.StoreId, request.BidId, new CreditCard(request.Card_number, request.Month, request.Year, request.Holder, request.Ccv, request.Id), new SupplyAddress(request.Name, request.Address, request.City, request.Country, request.Zip), DateTime.Now);
            if (response.ErrorOccured)
            {
                return BadRequest(new FrontResponse<double>(response.ErrorMessage));
            }
            return Ok(new FrontResponse<double>(response.Value));
        }

        [HttpPost("getbids")]
        public ActionResult<FrontResponse<List<Bid>>> GetBids([FromBody] StoreRequest request)
        {
            Response<List<Bid>> response = Service.GetBidsStatus(request.UserId, request.Membername, request.StoreId);
            if (response.ErrorOccured)
            {
                return BadRequest(new FrontResponse<List<Bid>>(response.ErrorMessage));
            }
            return Ok(new FrontResponse<List<Bid>>(response.Value));
        }
    }
}
