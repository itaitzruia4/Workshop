import { token, memberToken, StoreToken } from '../Types/roles';


export function handleGetStores(token: token) {
    let url = "http://localhost:5165/api/store/getstores";

    return fetch(url, {
        method: 'POST',
        mode: 'cors',
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
            userId: token.userId,
        })
    }).then(async response => {
        const data = await response.json();
        if (!response.ok) {
            return Promise.reject(data.error);
        }
        return Promise.resolve(data.value)
    })
}

export function handleNewStore(token: memberToken, storeName: string) {
    let url = "http://localhost:5165/api/store/newstore";

    return fetch(url, {
        method: 'POST',
        mode: 'cors',
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
            userId: token.userId,
            membername: token.membername,
            storeName: storeName
        })
    }).then(async response => {
        const data = await response.json();
        if (!response.ok) {
            return Promise.reject(data.error);
        }
        return Promise.resolve(data.value)
    })
}

export function handleAddProduct(token: memberToken, storeId: number, productName : string, description : string, price : number, quantity: number, category: string) {
    let url = "http://localhost:5165/api/store/addproduct";

    return fetch(url, {
        method: 'POST',
        mode: 'cors',
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
            userId: token.userId,
            membername: token.membername,
            storeId: storeId,
            productName: productName,
            description: description,
            price: price,
            quantity: quantity,
            category: category,
        })
    }).then(async response => {
        const data = await response.json();
        if (!response.ok) {
            return Promise.reject(data.error);
        }
        return Promise.resolve(data.value)
    })
}

export function handleCloseStore(token: memberToken, storeId: number) {
    let url = "http://localhost:5165/api/store/closestore";

    return fetch(url, {
        method: 'POST',
        mode: 'cors',
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
            userId: token.userId,
            membername: token.membername,
            storeId: storeId
        })
    }).then(async response => {
        const data = await response.json();
        if (!response.ok) {
            return Promise.reject(data.error);
        }
        alert(`Store closed successfully`);
        return Promise.resolve(data.value)
    })
}

export function handleOpenStore(token: memberToken, storeId: number) {
    let url = "http://localhost:5165/api/store/openstore";

    return fetch(url, {
        method: 'POST',
        mode: 'cors',
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
            userId: token.userId,
            membername: token.membername,
            storeId: storeId
        })
    }).then(async response => {
        const data = await response.json();
        if (!response.ok) {
            return Promise.reject(data.error);
        }
        alert(`Store opened successfully`);
        return Promise.resolve(data.value)
    })
}

export function handleRemoveProduct(token: memberToken, storeId: number, productId: number) {
    let url = "http://localhost:5165/api/store/removeproduct";

    return fetch(url, {
        method: 'POST',
        mode: 'cors',
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
            userId: token.userId,
            membername: token.membername,
            storeId: storeId,
            productId: productId
        })
    }).then(async response => {
        const data = await response.json();
        if (!response.ok) {
            return Promise.reject(data.error);
        }
        return Promise.resolve(data.value)
    })
}



export function handleAddDiscount(token: memberToken, storeId: number, discountJson: string) {
    const url = "http://localhost:5165/api/discount/addstorediscount";

    return fetch(url, {
        method: 'POST',
        mode: 'cors',
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
            userId: token.userId,
            membername: token.membername,
            storeId: storeId,
            JsonDiscount: discountJson
        })
    }).then(async response => {
        const data = await response.json();
        if (!response.ok) {
            return Promise.reject(data.error);
        }
        alert(`Discount in store ${storeId} has been added successfully`);
        return Promise.resolve(data.value)
    })
}

export function handleAddProductDiscount(token: memberToken, storeId: number, productId: number, discountJson: string) {
    const url = "http://localhost:5165/api/discount/addproductdiscount";

    return fetch(url, {
        method: 'POST',
        mode: 'cors',
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
            userId: token.userId,
            membername: token.membername,
            storeId: storeId,
            JsonDiscount: discountJson,
            productId: productId
        })
    }).then(response => response.json()
        .then(data => {
            if(!response.ok) {
                return Promise.reject(data.error);
            }
            alert(`Product discount for product ${productId} in store ${storeId} has been added successfully`);
            return Promise.resolve(data.value)
        }))
}

export function handleAddCategoryDiscount(token: memberToken, storeId: number, category: string, discountJson: string) {
    const url = "http://localhost:5165/api/discount/addcategorydiscount";

    return fetch(url, {
        method: 'POST',
        mode: 'cors',
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
            userId: token.userId,
            membername: token.membername,
            storeId: storeId,
            JsonDiscount: discountJson,
            category: category
        })
    }).then(async response => {
        const data = await response.json();
        if (!response.ok) {
            return Promise.reject(data.error);
        }
        alert(`Product discount for category ${category} in store ${storeId} has been added successfully`);
        return Promise.resolve(data.value)
    })
}

export function handleNominateStoreOwner(token: memberToken, storeId: number, nominee: string) {
    const url = "http://localhost:5165/api/store/nominateowner";

    return fetch(url, {
        method: 'POST',
        mode: 'cors',
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
            userId: token.userId,
            membername: token.membername,
            storeId: storeId,
            nominee: nominee
        })
    }).then(async response => {
        const data = await response.json();
        if (!response.ok) {
            return Promise.reject(data.error);
        }
        alert(`Voted for ${nominee} to be Store Owner of store ${storeId}`);
        return Promise.resolve(data.value)
    })
}

export function handleNominateStoreManager(token: memberToken, storeId: number, nominee: string) {
    const url = "http://localhost:5165/api/store/nominatemanager";

    return fetch(url, {
        method: 'POST',
        mode: 'cors',
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
            userId: token.userId,
            membername: token.membername,
            storeId: storeId,
            nominee: nominee
        })
    }).then(async response => {
        const data = await response.json();
        if (!response.ok) {
            return Promise.reject(data.error);
        }
        alert(`${nominee} is now Store Manager of store ${storeId}`);
        return Promise.resolve(data.value)
    })
}

export function handleRemoveStoreOwnerNomination(token: memberToken, storeId: number, nominee: string) {
    const url = "http://localhost:5165/api/store/removeownernomination";

    return fetch(url, {
        method: 'POST',
        mode: 'cors',
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
            userId: token.userId,
            membername: token.membername,
            storeId: storeId,
            nominee: nominee
        })
    }).then(async response => {
        const data = await response.json();
        if (!response.ok) {
            return Promise.reject(data.error);
        }
        alert(`${nominee} is no longer Store Owner of store ${storeId}`);
        return Promise.resolve(data.value)
    })
}

export function handleAddStorePurchasePolicy(token: memberToken, storeId: number, purchaseJson: string) {
    const url = "http://localhost:5165/api/store/addstorepurchaseterm";

    return fetch(url, {
        method: 'POST',
        mode: 'cors',
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
            userId: token.userId,
            membername: token.membername,
            storeId: storeId,
            Term: purchaseJson
        })
    }).then(async response => {
        const data = await response.json();
        if (!response.ok) {
            return Promise.reject(data.error);
        }
        alert(`Purchase Policy in store ${storeId} has been added successfully`);
        return Promise.resolve(data.value)
    })
}

export function handleAddProductPurchasePolicy(token: memberToken, storeId: number, productId: number, purchaseJson: string) {
    const url = "http://localhost:5165/api/store/addproductpurchaseterm";

    return fetch(url, {
        method: 'POST',
        mode: 'cors',
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
            userId: token.userId,
            membername: token.membername,
            storeId: storeId,
            productId: productId,
            Term: purchaseJson
        })
    }).then(async response => {
        const data = await response.json();
        if (!response.ok) {
            return Promise.reject(data.error);
        }
        alert(`Purchase Policy for product ${productId} in store ${storeId} has been added successfully`);
        return Promise.resolve(data.value)
    })
}

export function handleAddCategoryPurchasePolicy(token: memberToken, storeId: number, category: string, purchaseJson: string) {
    const url = "http://localhost:5165/api/store/addcategorypurchaseterm";

    return fetch(url, {
        method: 'POST',
        mode: 'cors',
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
            userId: token.userId,
            membername: token.membername,
            storeId: storeId,
            category: category,
            Term: purchaseJson
        })
    }).then(async response => {
        const data = await response.json();
        if (!response.ok) {
            return Promise.reject(data.error);
        }
        alert(`Purchase Policy for category ${category} in store ${storeId} has been added successfully`);
        return Promise.resolve(data.value)
    })
}

export function handleAddUserPurchasePolicy(token: memberToken, storeId: number, purchaseJson: string) {
    const url = "http://localhost:5165/api/store/adduserpurchaseterm";

    return fetch(url, {
        method: 'POST',
        mode: 'cors',
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
            userId: token.userId,
            membername: token.membername,
            storeId: storeId,
            Term: purchaseJson
        })
    }).then(async response => {
        const data = await response.json();
        if (!response.ok) {
            return Promise.reject(data.error);
        }
        alert(`User Purchase Policy in store ${storeId} has been added successfully`);
        return Promise.resolve(data.value)
    })
}

export function handleAddActionToManager(token: memberToken, nominee: string ,storeId: number, action: string) {
    const url = "http://localhost:5165/api/store/addactiontomanager";

    return fetch(url, {
        method: 'POST',
        mode: 'cors',
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
            userId: token.userId,
            membername: token.membername,
            nominee: nominee,
            storeId: storeId,
            action: action
        })
    }).then(async response => {
        const data = await response.json();
        if (!response.ok) {
            return Promise.reject(data.error);
        }
        alert(`${nominee} now has the permission of ${action} at store ${storeId}`);
        return Promise.resolve(data.value)
    })
}

export function handleGetStorePurchaseHistory(token: memberToken, storeId: number) {
    const url = "http://localhost:5165/api/store/getstorepurchasehistory";

    return fetch(url, {
        method: 'POST',
        mode: 'cors',
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
            userId: token.userId,
            membername: token.membername,
            storeId: storeId,
        })
    }).then(async response => {
        const data = await response.json();
        if (!response.ok) {
            return Promise.reject(data.error);
        }
        return Promise.resolve(data.value)
    })
}
