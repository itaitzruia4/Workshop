using API.Requests;
using API.Responses;
using Microsoft.AspNetCore.Mvc;
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
        public UserActionsController(IService service)
        {
            Service = service;
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
            Response<List<Product>> response = Service.SearchProduct(request.UserId, request.Membername, request.Keywords, request.Category, request.MinPrice, request.MaxPrice, request.ProductReview);
            if (response.ErrorOccured)
            {
                return BadRequest(new FrontResponse<List<Product>>(response.ErrorMessage));
            }
            return Ok(new FrontResponse<List<Product>>(response.Value));
        }

        [HttpPost("addtocart")]
        public ActionResult<FrontResponse<Product>> AddToCart([FromBody] AddToCartRequest request)
        {
            Response<Product> response = Service.AddToCart(request.UserId, request.Membername, request.ProductId, request.StoreId, request.Quantity);
            if (response.ErrorOccured)
            {
                return BadRequest(new FrontResponse<Product>(response.ErrorMessage));
            }
            return Ok(new FrontResponse<Product>(response.Value));
        }

        [HttpPost("viewcart")]
        public ActionResult<FrontResponse<ShoppingCart>> ViewCart([FromBody] MemberRequest request)
        {
            Response<ShoppingCart> response = Service.ViewCart(request.UserId, request.Membername);
            if (response.ErrorOccured)
            {
                return BadRequest(new FrontResponse<ShoppingCart>(response.ErrorMessage));
            }
            return Ok(new FrontResponse<ShoppingCart>(response.Value));
        }

        [HttpPost("editcart")]
        public ActionResult<FrontResponse<ShoppingCart>> EditCart([FromBody] EditCartRequest request)
        {
            Response<ShoppingCart> response = Service.EditCart(request.UserId, request.Membername, request.ProductId, request.Quantity);
            if (response.ErrorOccured)
            {
                return BadRequest(new FrontResponse<ShoppingCart>(response.ErrorMessage));
            }
            return Ok(new FrontResponse<ShoppingCart>(response.Value));
        }

        [HttpPost("buycart")]
        public ActionResult<FrontResponse<double>> BuyCart([FromBody] BuyCartRequest request)
        {
            // TODO RONMI FIXME!!!!!!!!!!!!!!!!
            Response<double> response = Service.BuyCart(
                request.UserId, 
                "we shouldn't send a membername",   // TODO FIX THIS!!!!!!!!!!!!!! BUYCART IS ALSO A GUEST ACTION!
                new CreditCard("123", "March", "2024", "ronmi", "123", "123456789"),
                new SupplyAddress("ronmi", request.Address, "Ashkelon", "israel", "8502500")
                );
            if (response.ErrorOccured)
            {
                return BadRequest(new FrontResponse<double>(response.ErrorMessage));
            }
            return Ok(new FrontResponse<double>(response.Value));
        }

    }
}
