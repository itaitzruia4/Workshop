using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshop.DomainLayer.UserPackage.Permissions
{
    public enum Action
    {
        AddProduct,
        RemoveProduct,
        ChangeProductName,
        ChangeProductPrice,
        ChangeProductQuantity,
        ChangeProductDescription,
        NominateStoreOwner,
        NominateStoreManager,
        GetWorkersInformation,
        OpenStore,
        CloseStore,
        AddPermissionToStoreManager,
        RemovePermissionFromStoreManager,
        GetStoreOrdersList,
        ViewClosedStore,
        AddDiscount,
        GetMarketStatistics,
        CancelMember,
        GetMembersOnlineStats,
        AddPurchaseTerm
    }
}
