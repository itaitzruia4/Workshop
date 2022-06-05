import { token, memberToken, StoreToken } from '../Types/roles';


export function handleChangeProductName(token: memberToken, storeId: number, productId: number, name: string) {
    let url = "http://localhost:5165/api/productactions/changeproductname";

    return fetch(url, {
        method: 'POST',
        mode: 'cors',
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
            userId: token.userId,
            membername: token.membername,
            productId: productId,
            storeId: storeId,
            newName: name,
        })
    }).then(async response => {
        const data = await response.json();
        if (!response.ok) {
            return Promise.reject(data.error);
        }
        return Promise.resolve(data.value)
    })
}

export function handleChangeProductPrice(token: memberToken, storeId: number, productId: number, price: number) {
    let url = "http://localhost:5165/api/productactions/changeproductprice";

    return fetch(url, {
        method: 'POST',
        mode: 'cors',
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
            userId: token.userId,
            membername: token.membername,
            productId: productId,
            storeId: storeId,
            newPrice: price,
        })
    }).then(async response => {
        const data = await response.json();
        if (!response.ok) {
            return Promise.reject(data.error);
        }
        return Promise.resolve(data.value)
    })
}

export function handleChangeProductQuantity(token: memberToken, storeId: number, productId: number, quantity: number) {
    let url = "http://localhost:5165/api/productactions/changeproductquantity";

    return fetch(url, {
        method: 'POST',
        mode: 'cors',
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
            userId: token.userId,
            membername: token.membername,
            productId: productId,
            storeId: storeId,
            newQuantity: quantity,
        })
    }).then(async response => {
        const data = await response.json();
        if (!response.ok) {
            return Promise.reject(data.error);
        }
        return Promise.resolve(data.value)
    })
}

export function handleChangeProductCategory(token: memberToken, storeId: number, productId: number, category: string) {
    let url = "http://localhost:5165/api/productactions/changeproductcategory";

    return fetch(url, {
        method: 'POST',
        mode: 'cors',
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
            userId: token.userId,
            membername: token.membername,
            productId: productId,
            storeId: storeId,
            newCategory: category,
        })
    }).then(async response => {
        const data = await response.json();
        if (!response.ok) {
            return Promise.reject(data.error);
        }
        return Promise.resolve(data.value)
    })
}