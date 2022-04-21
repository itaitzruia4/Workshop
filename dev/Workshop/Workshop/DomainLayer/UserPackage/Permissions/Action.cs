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
        NominateStoreOwner,
        NominateStoreManager,
    }
}
