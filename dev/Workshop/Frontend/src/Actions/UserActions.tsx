import { token, memberToken } from '../Types/roles';


export function handleAddToCart(token: memberToken,storeId : number, productId : number, quantity: number ) {
    let url = "http://localhost:5165/api/useractions/addtocart";

    return fetch(url, {
        method: 'POST',
        mode: 'cors',
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
            userId: token.userId,
            membername: token.membername,
            productId: productId,
            storeId: storeId,
            quantity: quantity,
        })
    }).then(async response => {
        const data = await response.json();
        if (!response.ok) {
            return Promise.reject(data.error);
        }
        return Promise.resolve(data.value)
    })
}

export function handleViewCart(token: memberToken) {
    let url = "http://localhost:5165/api/useractions/addtocart";

    return fetch(url, {
        method: 'POST',
        mode: 'cors',
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
            userId: token.userId,
            membername: token.membername,
        })
    }).then(async response => {
        const data = await response.json();
        if (!response.ok) {
            return Promise.reject(data.error);
        }
        return Promise.resolve(data.value)
    })
}