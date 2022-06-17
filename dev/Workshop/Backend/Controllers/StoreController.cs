using API.Requests;
using API.Responses;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
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
        public ActionResult<FrontResponse<Store>> CreateNewStore([FromBody] StoreCreationRequest request)
        {
            Response<Store> response = Service.CreateNewStore(request.UserId, request.Membername, request.StoreName);
            if (response.ErrorOccured)
            {
                return BadRequest(new FrontResponse<Store>(response.ErrorMessage));
            }
            return Ok(new FrontResponse<Store>(response.Value));
        }

        [HttpPost("getstores")]
        public ActionResult<FrontResponse<List<Store>>> GetAllStores([FromBody] BaseRequest request)
        {
            Response<List<Store>> response = Service.GetAllStores(request.UserId);
            if (response.ErrorOccured)
            {
                return BadRequest(new FrontResponse<List<Store>>(response.ErrorMessage));
            }
            return Ok(new FrontResponse<List<Store>>(response.Value));
        }

        [HttpPost("addproduct")]
        public ActionResult<FrontResponse<Product>> AddProduct([FromBody] AddProductRequest request)
        {
            Response<Product> response = Service.AddProduct(request.UserId, request.Membername, request.StoreId, request.ProductName, request.Description, request.Price, request.Quantity, request.Category);
            if (response.ErrorOccured)
            {
                return BadRequest(new FrontResponse<Product>(response.ErrorMessage));
            }
            return Ok(new FrontResponse<Product>(response.Value));
        }

        [HttpPost("nominatemanager")]
        public ActionResult<FrontResponse<StoreManager>> NominateStoreManager([FromBody] NominationRequestWithDate request)
        {
            Response<StoreManager> response = Service.NominateStoreManager(request.UserId, request.Membername, request.Nominee, request.StoreId, DateTime.ParseExact(request.Date, "dd/MM/yyyy", CultureInfo.InvariantCulture));
            if (response.ErrorOccured)
            {
                return BadRequest(new FrontResponse<StoreManager>(response.ErrorMessage));
            }
            return Ok(new FrontResponse<StoreManager>(response.Value));
        }

        [HttpPost("nominateowner")]
        public ActionResult<FrontResponse<StoreOwner>> NominateStoreOwner([FromBody] NominationRequestWithDate request)
        {
            Response<StoreOwner> response = Service.NominateStoreOwner(request.UserId, request.Membername, request.Nominee, request.StoreId, DateTime.ParseExact(request.Date, "dd/MM/yyyy", CultureInfo.InvariantCulture));
            if (response.ErrorOccured)
            {
                return BadRequest(new FrontResponse<StoreOwner>(response.ErrorMessage));
            }
            return Ok(new FrontResponse<StoreOwner>(response.Value));
        }

        [HttpPost("rejectownernomination")]
        public ActionResult<FrontResponse<int>> RejectStoreOwnerNomination([FromBody] NominationRequest request)
        {
            Response response = Service.RejectStoreOwnerNomination(request.UserId, request.Membername, request.Nominee, request.StoreId);
            if (response.ErrorOccured)
            {
                return BadRequest(new FrontResponse<int>(response.ErrorMessage));
            }
            return Ok(new FrontResponse<int>(response.UserId));
        }

        [HttpPost("removeownernomination")]
        public ActionResult<FrontResponse<Member>> RemoveStoreOwnerNomination([FromBody] NominationRequest request)
        {
            Response<Member> response = Service.RemoveStoreOwnerNomination(request.UserId, request.Membername, request.Nominee, request.StoreId);
            if (response.ErrorOccured)
            {
                return BadRequest(new FrontResponse<Member>(response.ErrorMessage));
            }
            return Ok(new FrontResponse<Member>(response.Value));
        }

        [HttpPost("addactiontomanager")]
        public ActionResult<FrontResponse<string>> AddActionToManager([FromBody] AddActionToManagerRequest request)
        {
            Response response = Service.AddActionToManager(request.UserId, request.Membername, request.Nominee, request.StoreId, request.Action);
            if (response.ErrorOccured)
            {
                return BadRequest(new FrontResponse<string>(response.ErrorMessage));
            }
            return Ok(new FrontResponse<string>(request.Action));
        }

        [HttpPost("getworkersinformation")]
        public ActionResult<FrontResponse<List<Member>>> GetWorkersInformation([FromBody] StoreRequest request)
        {
            Response<List<Member>> response = Service.GetWorkersInformation(request.UserId, request.Membername, request.StoreId);
            if (response.ErrorOccured)
            {
                return BadRequest(new FrontResponse<List<Member>>(response.ErrorMessage));
            }
            return Ok(new FrontResponse<List<Member>>(response.Value));
        }

        [HttpPost("closestore")]
        public ActionResult<FrontResponse<int>> CloseStore([FromBody] StoreRequest request)
        {
            Response response = Service.CloseStore(request.UserId, request.Membername, request.StoreId);
            if (response.ErrorOccured)
            {
                return BadRequest(new FrontResponse<int>(response.ErrorMessage));
            }
            return Ok(new FrontResponse<int>(request.UserId));
        }

        [HttpPost("openstore")]
        public ActionResult<FrontResponse<int>> OpenStore([FromBody] StoreRequest request)
        {
            Response response = Service.OpenStore(request.UserId, request.Membername, request.StoreId);
            if (response.ErrorOccured)
            {
                return BadRequest(new FrontResponse<int>(response.ErrorMessage));
            }
            return Ok(new FrontResponse<int>(request.UserId));
        }

        [HttpPost("removeproduct")]
        public ActionResult<FrontResponse<int>> RemoveProduct([FromBody] ProductStoreRequest request)
        {
            Response response = Service.RemoveProductFromStore(request.UserId, request.Membername, request.StoreId, request.ProductId);
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

        [HttpPost("addcategorypurchaseterm")]
        public ActionResult<FrontResponse<string>> AddCategoryPurchaseTerm([FromBody] AddCategoryPurchaseTermRequest request)
        {
            Response response = Service.AddCategoryPurchaseTerm(request.UserId, request.Membername, request.StoreId, request.Term, request.Category);
            if (response.ErrorOccured)
            {
                return BadRequest(new FrontResponse<string>(response.ErrorMessage));
            }
            return Ok(new FrontResponse<string>(request.Category));

        }

        [HttpPost("addstorepurchaseterm")]
        public ActionResult<FrontResponse<int>> AddStorePurchaseTerm([FromBody] StoreTermRequest request)
        {
            Response response = Service.AddStorePurchaseTerm(request.UserId, request.Membername, request.StoreId, request.Term);
            if (response.ErrorOccured)
            {
                return BadRequest(new FrontResponse<int>(response.ErrorMessage));
            }
            return Ok(new FrontResponse<int>(request.StoreId));

        }

        [HttpPost("adduserpurchaseterm")]
        public ActionResult<FrontResponse<int>> AddUserPurchaseTerm([FromBody] StoreTermRequest request)
        {
            Response response = Service.AddUserPurchaseTerm(request.UserId, request.Membername, request.StoreId, request.Term);
            if (response.ErrorOccured)
            {
                return BadRequest(new FrontResponse<int>(response.ErrorMessage));
            }
            return Ok(new FrontResponse<int>(request.StoreId));

        }

        [HttpPost("getdailyincomestore")]
        public ActionResult<FrontResponse<double>> GetDailyIncomeStore([FromBody] StoreRequest request)
        {
            Response<double> response = Service.GetDailyIncomeStore(request.UserId, request.Membername, request.StoreId);
            if (response.ErrorOccured)
            {
                return BadRequest(new FrontResponse<double>(response.ErrorMessage));
            }
            return Ok(new FrontResponse<double>(response.Value));

        }
    }
}

