import { token, memberToken } from '../Types/roles';


export function handleEnterMarket() {
    let url = "http://localhost:5165/api/authentication/entermarket";

    return fetch(url, {
        method: 'GET',
        mode: 'cors',
        headers: { "Content-Type": "application/json" }
    }).then(async response => {
        const data = await response.json();
        if (!response.ok) {
            return Promise.reject(data.error);
        }
        return Promise.resolve(data.value)
    })
}


export function handleRegister(token: token, membername: string, password: string, birthDate: string): Promise<any>{
    if (membername === "" || password === "" || birthDate == "") {
        return Promise.reject('User details must not be empty');
    }

    var date_regex = /\d{1,2}\/\d{1,2}\/\d{4}/;
    if (!(date_regex.test(birthDate))) {
        return Promise.reject("Invalid date format");
    }

    let url = "http://localhost:5165/api/authentication/register";

    return fetch(url, {
        method: 'POST',
        mode: 'cors',
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
            userId: token.userId,
            membername: membername,
            password: password,
            birthDate: birthDate
        })
    }).then(async response => {
        const data = await response.json();
        if (!response.ok) {
            return Promise.reject(data.error);
        }
        return Promise.resolve(data.value)
    })
}

export function handleLogin(token: token, membername: string, password: string): Promise<any> {
    if (membername === "" || password === "") {
        alert('User details must not be empty');
        return Promise.reject('User details must not be empty');
    }

    let url = "http://localhost:5165/api/authentication/login";

    return fetch(url, {
        method: 'POST',
        mode: 'cors',
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
            userId: token.userId,
            membername: membername,
            password: password,
        })
    }).then(async response => {
        const data = await response.json();
        if (!response.ok) {
            return Promise.reject(data.error);
        }
        return Promise.resolve(data)
    })
}

export function handleLogout(token: memberToken): Promise<any> {

    let url = "http://localhost:5165/api/authentication/logout";

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

export function handleExitMarket(token: token): Promise<any> {

    let url = "http://localhost:5165/api/authentication/exitmarket";

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

