import React, { useState, useEffect } from 'react';
import { BrowserRouter as Router, Routes, Route } from "react-router-dom";
import { delay } from './Utils/utils'
import Login from './Pages/Login'
import Register from './Pages/Register'
import Member from './Pages/Member'
import Guest from './Pages/Guest'
import Home from './Pages/Home'
import Store from './Pages/Store'

function App() {

    const [userId, setUserId] = useState(null);

    // Send "EnterMarket" request to server
    let url = "http://localhost:5165/api/authentication/entermarket";

    (async () => {
        await delay(5000);
    })();

    fetch(url, {
        method: 'GET',
        mode: 'cors',
        headers: { "Content-Type": "application/json" }
    }).then((res) => res.json())
        .then((data) => setUserId(data.value))
        .catch();

    // storing user id in local storage
    useEffect(() => {
        localStorage.setItem("userId", JSON.stringify(userId));
    }, [userId]);

    return (
        <Router>
            <Routes>
                <Route path="/" element={<Home />} />
                <Route path="/register" element={<Register />} />
                <Route path="/login" element={<Login />} />
                <Route path="/member" element={<Member />} />
                <Route path="/guest" element={<Guest />} />
                <Route path="/store" element={<Store />} />
            </Routes>
        </Router>
    )
}


export default App;