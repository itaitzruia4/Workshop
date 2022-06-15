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

export function handleEditCart(token: userToken, productId: number, quantity: number) {
    let url = "http://localhost:5165/api/useractions/editcart";

    return fetch(url, {
        method: 'POST',
        mode: 'cors',
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
            userId: token.userId,
            productId: productId,
            quantity: quantity
        })
    }).then(async response => {
        const data = await response.json();
        if (!response.ok) {
            return Promise.reject(data.error);
        }
        return Promise.resolve(data.value)
    })
}

export function handleBuyCart(token: userToken, number: string, year: string, month: string, ccv: string, holder: string, id: string, name: string, address: string,
    city: string, country: string, zip: string) {
    let url = "http://localhost:5165/api/useractions/buycart";

    return fetch(url, {
        method: 'POST',
        mode: 'cors',
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
            userId: token.userId,
            card_number: number,
            year: year,
            month: month,
            ccv: ccv,
            holder: holder,
            id: id,
            name: name,
            address: address,
            city: city,
            country: country,
            zip: zip
        })
    }).then(async response => {
        const data = await response.json();
        if (!response.ok) {
            return Promise.reject(data.error);
        }
        alert("Bought cart successfully!")
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

export function handleGetMemberPermissions(token: memberToken) {
    let url = "http://localhost:5165/api/useractions/getmemberpermissions";

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