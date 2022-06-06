import { token, memberToken, userToken } from '../Types/roles';


export function handleAddToCart(token: userToken, storeId : number, productId : number, quantity: number ) {
    let url = "http://localhost:5165/api/useractions/addtocart";

    return fetch(url, {
        method: 'POST',
        mode: 'cors',
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
            userId: token.userId,
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

export function handleViewCart(token: userToken) {
    let url = "http://localhost:5165/api/useractions/viewcart";

    return fetch(url, {
        method: 'POST',
        mode: 'cors',
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
            userId: token.userId
        })
    }).then(async response => {
        const data = await response.json();
        if (!response.ok) {
            return Promise.reject(data.error);
        }
        return Promise.resolve(data.value)
    })
}

export function handleReviewProduct(token: memberToken, productId: number, review: string, rating: number) {
    let url = "http://localhost:5165/api/useractions/reviewproduct";

    return fetch(url, {
        method: 'POST',
        mode: 'cors',
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
            userId: token.userId,
            membername: token.membername,
            productId: productId,
            review: review,
            rating: rating
        })
    }).then(async response => {
        const data = await response.json();
        if (!response.ok) {
            return Promise.reject(data.error);
        }
        return Promise.resolve(data.value)
    })
}

export function handleUpdateNotifications(token: memberToken) {
    let url = "http://localhost:5165/api/useractions/takenotifications";

    return fetch(url, {
        method: 'POST',
        mode: 'cors',
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
            userId: token.userId,
            membername: token.membername
        })
    }).then(async response => {
        const data = await response.json();
        if (!response.ok) {
            return Promise.reject(data.error);
        }
        return Promise.resolve(data.value)
    })
}