using API.Requests;
using API.Responses;
using Microsoft.AspNetCore.Mvc;
using Workshop.ServiceLayer;
using Workshop.ServiceLayer.ServiceObjects;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductActionsController : ControllerBase
    {
        IService Service;
        public ProductActionsController(IService service)
        {
            Service = service;
        }

        [HttpPost("changeproductname")]
        public ActionResult<FrontResponse<int>> ChangeProductName([FromBody] ChangeProductNameRequest request)
        {
            Response response = Service.ChangeProductName(request.UserId, request.Membername, request.StoreId, request.ProductId, request.NewName);
            if (response.ErrorOccured)
            {
                return BadRequest(new FrontResponse<int>(response.ErrorMessage));
            }
            return Ok(new FrontResponse<int>(request.ProductId));
        }

        [HttpPost("changeproductprice")]
        public ActionResult<FrontResponse<int>> ChangeProductPrice([FromBody] ChangeProductPriceRequest request)
        {
            Response response = Service.ChangeProductPrice(request.UserId, request.Membername, request.StoreId, request.ProductId, request.NewPrice);
            if (response.ErrorOccured)
            {
                return BadRequest(new FrontResponse<int>(response.ErrorMessage));
            }
            return Ok(new FrontResponse<int>(request.ProductId));
        }

        [HttpPost("changeproductquantity")]
        public ActionResult<FrontResponse<int>> ChangeProductQuantity([FromBody] ChangeProductQuantityRequest request)
        {
            Response response = Service.ChangeProductQuantity(request.UserId, request.Membername, request.StoreId, request.ProductId, request.NewQuantity);
            if (response.ErrorOccured)
            {
                return BadRequest(new FrontResponse<int>(response.ErrorMessage));
            }
            return Ok(new FrontResponse<int>(request.ProductId));
        }

        [HttpPost("changeproductcategory")]
        public ActionResult<FrontResponse<int>> ChangeProductCategory([FromBody] ChangeProductCategoryRequest request)
        {
            Response response = Service.ChangeProductCategory(request.UserId, request.Membername, request.StoreId, request.ProductId, request.NewCategory);
            if (response.ErrorOccured)
            {
                return BadRequest(new FrontResponse<int>(response.ErrorMessage));
            }
            return Ok(new FrontResponse<int>(request.ProductId));
        }

        [HttpPost("addproductpurchaseterm")]
        public ActionResult<FrontResponse<int>> AddProductPurchaseTerm([FromBody] AddProductPurchaseTermRequest request)
        {
            Response response = Service.AddProductPurchaseTerm(request.UserId, request.Membername, request.StoreId, request.Term, request.ProductId);
            if (response.ErrorOccured)
            {
                return BadRequest(new FrontResponse<int>(response.ErrorMessage));
            }
            return Ok(new FrontResponse<int>(request.ProductId));
        }

    }
}
