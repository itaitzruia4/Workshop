import { memberToken } from "../Types/roles"

export function handleRemoveMember(token: memberToken, memberToRemove: string): Promise<any> {
    let url = "http://localhost:5165/api/useractions/cancelmember";

    return fetch(url, {
        method: 'POST',
        mode: 'cors',
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
            userId: token.userId,
            membername: token.membername,
            MemberToCancel: memberToRemove
        })
    }).then(async response => {
        const data = await response.json();
        if (!response.ok) {
            return Promise.reject(data.error);
        }
        return Promise.resolve(data.value)
    })
}

export function handleGetStorePurchaseHistory(token: memberToken, storeId: number): Promise<any> {
    let url = "http://localhost:5165/api/useractions/cancelmember";

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
        return Promise.resolve(data.value)
    })
}

export function handleGetMemberInformation(token: memberToken): Promise<any> {
    let url = "http://localhost:5165/api/useractions/cancelmember";

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

export function handleGetDailyIncome(token: memberToken): Promise<any> {
    // TODO add daily income API request
    return Promise.reject("Not yet implemented");
}

export function handleViewStatistics(token: memberToken, fromDate: string, toDate: string): Promise<any> {
    // TODO add daily statistics API request
    return Promise.reject("Not yet implemented");
}