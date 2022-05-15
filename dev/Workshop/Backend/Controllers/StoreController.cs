using API.Requests;
using API.Responses;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Workshop.ServiceLayer;
using Workshop.ServiceLayer.ServiceObjects;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StoreController : ControllerBase
    {
        IService Service;
        public StoreController(IService service)
        {
            Service = service;
        }

        [HttpPost("newstore")]
        public ActionResult<StoreResponse> Post([FromBody] CreateNewStoreRequest request)
        {
            Response<Store> response = Service.CreateNewStore(request.UserId, request.Creator, request.StoreName);
            if (response.ErrorOccured)
            {
                return BadRequest(new StoreResponse(response.ErrorMessage));
            }
            return Ok(new StoreResponse(response.Value));
        }

        [HttpPost("getstores")]
        public ActionResult<StoresResponse> Get([FromBody] GetAllStoresRequest request)
        {
            Response<List<Store>> response = Service.GetAllStores(request.UserId);
            if (response.ErrorOccured)
            {
                return BadRequest(new StoresResponse(response.ErrorMessage));
            }
            List<StoreResponse> result = new List<StoreResponse>();
            foreach (Store st in response.Value)
            {
                result.Add(new StoreResponse(st));
            }
            return Ok(new StoresResponse(result));
        }

        [HttpPost("addproduct")]
        public ActionResult<ProductResponse> Post([FromBody] AddProductRequest request)
        {
            Response<Product> response = Service.AddProduct(request.UserId, request.Membername, request.StoreId, request.ProductName, request.Description, request.Price, request.Quantity, request.Category);
            if (response.ErrorOccured)
            {
                return BadRequest(new ProductResponse(response.ErrorMessage));
            }
            return Ok(new ProductResponse(response.Value));
        }

        [HttpPost("nominatemanager")]
        public ActionResult<StoreManagerResponse> Post([FromBody] NominationRequest request)
        {
            Response<StoreManager> response = Service.NominateStoreManager(request.UserId, request.Nominator, request.Nominated, request.StoreId);
            if (response.ErrorOccured)
            {
                return BadRequest(new StoreManagerResponse(response.ErrorMessage));
            }
            return Ok(new StoreManagerResponse(response.Value));
        }
    }
}

