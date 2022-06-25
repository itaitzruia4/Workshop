import { memberToken } from "../Types/roles"

export function handleIsAdmin(token: memberToken): Promise<any> {
    let url = "http://localhost:5165/api/useractions/GetMemberPermissions";

    return fetch(url, {
        method: 'POST',
        mode: 'cors',
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
            userId: token.userId,
            membername: token.membername
        })
    }).then(response => response.json()
        .then(data => {
            if (!response.ok) {
                return Promise.reject(data.error);
            }
            return Promise.resolve((data.value as Array<{storeId: number}>).filter(elem => elem.storeId === -1).length > 0);
        }))
}

function handleRemoveMember(token: memberToken, memberToRemove: string): Promise<any> {
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

export const removeMember = (token: memberToken, membername: string): void => {
    handleRemoveMember(token, membername)
        .catch(error => {
            alert(error)
        });
}

function handleGetStorePurchaseHistory(token: memberToken, storeId: number): Promise<any> {
    let url = "http://localhost:5165/api/store/GetDailyIncomeStore";

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

export const getStorePurchaseHistory = (token: memberToken, storeId: number): void => {
    handleGetStorePurchaseHistory(token, storeId)
        .catch(error => {
            alert(error)
        });
}

export function handleGetMemberInformation(token: memberToken): Promise<any> {
    let url = "http://localhost:5165/api/useractions/getmembersonlinestats";

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

export function getDailyIncome(token: memberToken): Promise<any> {
    let url = "http://localhost:5165/api/useractions/getdailyincomemarketmanager";

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
        return Promise.resolve(data.value);
    })
}

export function handleViewStatistics(token: memberToken, fromDate: string, toDate: string): Promise<any> {
    if (fromDate === "" || toDate === "") {
        return Promise.reject('User details must not be empty');
    }

    var date_regex = /\d{1,2}\/\d{1,2}\/\d{4}/;
    if (!(date_regex.test(fromDate)) || !(date_regex.test(toDate))) {
        return Promise.reject("Invalid date format");
    }

    const url = "http://localhost:5165/api/useractions/marketmanagerdaily";
    return fetch(url, {
        method: 'POST',
        mode: 'cors',
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
            userId: token.userId,
            membername: token.membername,
            StartDate: fromDate,
            EndDate: toDate
        })
    }).then(async response => {
        const data = await response.json();
        if (!response.ok) {
            return Promise.reject(data.error);
        }
        return Promise.resolve(data.value);
    })
}