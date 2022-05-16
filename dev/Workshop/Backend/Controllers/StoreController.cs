using API.Requests;
using API.Responses;
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
        public ActionResult<StoreResponse> CreateNewStore([FromBody] CreateNewStoreRequest request)
        {
            Response<Store> response = Service.CreateNewStore(request.UserId, request.Creator, request.StoreName);
            if (response.ErrorOccured)
            {
                return BadRequest(new StoreResponse(response.ErrorMessage));
            }
            return Ok(new StoreResponse(response.Value));
        }

        [HttpPost("getstores")]
        public ActionResult<StoresResponse> GetAllStores([FromBody] GuestRequest request)
        {
            Response<List<Store>> response = Service.GetAllStores(request.UserId);
            if (response.ErrorOccured)
            {
                return BadRequest(new StoresResponse(response.ErrorMessage));
            }
            return Ok(new StoresResponse(response.Value));
        }

        [HttpPost("addproduct")]
        public ActionResult<ProductResponse> AddProduct([FromBody] AddProductRequest request)
        {
            Response<Product> response = Service.AddProduct(request.UserId, request.Membername, request.StoreId, request.ProductName, request.Description, request.Price, request.Quantity, request.Category);
            if (response.ErrorOccured)
            {
                return BadRequest(new ProductResponse(response.ErrorMessage));
            }
            return Ok(new ProductResponse(response.Value));
        }

        [HttpPost("nominatemanager")]
        public ActionResult<StoreManagerResponse> NominateStoreManager([FromBody] NominationRequest request)
        {
            Response<StoreManager> response = Service.NominateStoreManager(request.UserId, request.Nominator, request.Nominated, request.StoreId);
            if (response.ErrorOccured)
            {
                return BadRequest(new StoreManagerResponse(response.ErrorMessage));
            }
            return Ok(new StoreManagerResponse(response.Value));
        }

        [HttpPost("nominateowner")]
        public ActionResult<StoreOwnerResponse> NominateStoreOwner([FromBody] NominationRequest request)
        {
            Response<StoreOwner> response = Service.NominateStoreOwner(request.UserId, request.Nominator, request.Nominated, request.StoreId);
            if (response.ErrorOccured)
            {
                return BadRequest(new StoreOwnerResponse(response.ErrorMessage));
            }
            return Ok(new StoreOwnerResponse(response.Value));
        }

        [HttpPost("removeownernomination")]
        public ActionResult<MemberResponse> RemoveStoreOwnerNomination([FromBody] NominationRequest request)
        {
            Response<Member> response = Service.RemoveStoreOwnerNomination(request.UserId, request.Nominator, request.Nominated, request.StoreId);
            if (response.ErrorOccured)
            {
                return BadRequest(new MemberResponse(response.ErrorMessage));
            }
            return Ok(new MemberResponse(response.Value));
        }

        [HttpPost("getworkersinformation")]
        public ActionResult<MembersResponse> GetWorkersInformation([FromBody] UserStoreRequest request)
        {
            Response<List<Member>> response = Service.GetWorkersInformation(request.UserId, request.Membername, request.StoreId);
            if (response.ErrorOccured)
            {
                return BadRequest(new MembersResponse(response.ErrorMessage));
            }
            return Ok(new MembersResponse(response.Value));
        }

        [HttpPost("closestore")]
        public ActionResult<AuthenticationResponse> CloseStore([FromBody] UserStoreRequest request)
        {
            Response response = Service.CloseStore(request.UserId, request.Membername, request.StoreId);
            if (response.ErrorOccured)
            {
                return BadRequest(new AuthenticationResponse(response.ErrorMessage));
            }
            return Ok(new AuthenticationResponse(request.UserId));
        }
    }
}

