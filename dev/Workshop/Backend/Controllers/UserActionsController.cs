using API.Requests;
using API.Responses;
using Microsoft.AspNetCore.Mvc;
using Workshop.DomainLayer.Reviews;
using Workshop.ServiceLayer;
using Workshop.ServiceLayer.ServiceObjects;

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
        public ActionResult<ReviewResponse> ReviewProduct([FromBody] ProductReviewRequest request)
        {
            Response<ReviewDTO> response = Service.ReviewProduct(request.UserId, request.Membername, request.ProductId, request.Review, request.Rating);
            if (response.ErrorOccured)
            {
                return BadRequest(new ReviewResponse(response.ErrorMessage));
            }
            return Ok(new ReviewResponse(response.Value));
        }

        [HttpPost("searchproduct")]
        public ActionResult<ProductsResponse> SearchProduct([FromBody] ProductSearchRequest request)
        {
            Response<List<Product>> response = Service.SearchProduct(request.UserId, request.Membername, request.KeyWords, request.Category, request.MinPrice, request.MaxPrice, request.ProductReview);
            if (response.ErrorOccured)
            {
                return BadRequest(new ProductsResponse(response.ErrorMessage));
            }
            return Ok(new ProductsResponse(response.Value));
        }

        [HttpPost("addtocart")]
        public ActionResult<ProductResponse> AddToCart([FromBody] AddToCartRequest request)
        {
            Response<Product> response = Service.addToCart(request.UserId, request.Membername, request.ProductId, request.StoreId, request.Quantity);
            if (response.ErrorOccured)
            {
                return BadRequest(new ProductResponse(response.ErrorMessage));
            }
            return Ok(new ProductResponse(response.Value));
        }

        [HttpPost("viewcart")]
        public ActionResult<ShoppingCartResponse> ViewCart([FromBody] LogoutRequest request)
        {
            Response<ShoppingCart> response = Service.viewCart(request.UserId, request.Membername);
            if (response.ErrorOccured)
            {
                return BadRequest(new ShoppingCartResponse(response.ErrorMessage));
            }
            return Ok(new ShoppingCartResponse(response.Value));
        }

        [HttpPost("editcart")]
        public ActionResult<ShoppingCartResponse> EditCart([FromBody] EditCartRequest request)
        {
            Response<ShoppingCart> response = Service.editCart(request.UserId, request.Membername, request.ProductId, request.Quantity);
            if (response.ErrorOccured)
            {
                return BadRequest(new ShoppingCartResponse(response.ErrorMessage));
            }
            return Ok(new ShoppingCartResponse(response.Value));
        }

        [HttpPost("buycart")]
        public ActionResult<PriceResponse> BuyCart([FromBody] BuyCartRequest request)
        {
            Response<double> response = Service.BuyCart(request.UserId, request.Membername, request.Address);
            if (response.ErrorOccured)
            {
                return BadRequest(new PriceResponse(response.ErrorMessage));
            }
            return Ok(new PriceResponse(response.Value));
        }

    }
}
