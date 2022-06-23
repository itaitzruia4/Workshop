import { memberToken } from "../Types/roles"

function handleIsAdmin(token: memberToken): Promise<boolean> {
    let url = "http://localhost:5165/api/useractions/GetMemberPermissions";

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
        //if (data.filter(perm => perm.storeId === -1).length > 0) {
        //    return Promise.resolve(true);
        //}
        return Promise.resolve(false);
    })
}

export const isAdmin = (token: memberToken): boolean => {
    return token.membername === "admin";

    // TODO fix this
    //return handleIsAdmin(token)
    //    .then((b: boolean) => b)
    //    .catch(error => {
    //        return false;
    //    });
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

function handleViewStatistics(token: memberToken, fromDate: string, toDate: string): Promise<any> {
    // TODO add daily statistics API request
    return Promise.reject("Not yet implemented");
}

export const viewStatistics = (token: memberToken, fromDate: string, toDate: string): void => {
    handleViewStatistics(token, fromDate, toDate)
        .catch(error => {
            alert(error)
        });
}