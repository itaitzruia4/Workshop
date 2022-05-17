using API.Requests;
using API.Responses;
using Microsoft.AspNetCore.Mvc;
using Workshop.DomainLayer.Reviews;
using Workshop.ServiceLayer;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DiscountController : ControllerBase
    {
        IService Service;
        public DiscountController(IService service)
        {
            Service = service;
        }

        [HttpPost("addproductdiscount")]
        public ActionResult<FrontResponse<bool>> AddProductDiscount([FromBody] ProductDiscountRequest request)
        {
            Response response = Service.AddProductDiscount(request.UserId, request.Membername, request.StoreId, request.JsonDiscount, request.ProductId);
            if (response.ErrorOccured)
            {
                return BadRequest(new FrontResponse<bool>(response.ErrorMessage));
            }
            return Ok(new FrontResponse<bool>(true));
        }

        [HttpPost("addcategorydiscount")]
        public ActionResult<FrontResponse<bool>> AddCategoryDiscount([FromBody] CategoryDiscountRequest request)
        {
            Response response = Service.AddCategoryDiscount(request.UserId, request.Membername, request.StoreId, request.JsonDiscount, request.Category);
            if (response.ErrorOccured)
            {
                return BadRequest(new FrontResponse<bool>(response.ErrorMessage));
            }
            return Ok(new FrontResponse<bool>(true));
        }

        [HttpPost("addstorediscount")]
        public ActionResult<FrontResponse<bool>> AddStoreDiscount([FromBody] DiscountRequest request)
        {
            Response response = Service.AddStoreDiscount(request.UserId, request.Membername, request.StoreId, request.JsonDiscount);
            if (response.ErrorOccured)
            {
                return BadRequest(new FrontResponse<bool>(response.ErrorMessage));
            }
            return Ok(new FrontResponse<bool>(true));
        }

    }
}
