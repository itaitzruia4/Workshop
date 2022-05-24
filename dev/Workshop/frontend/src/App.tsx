import React, { useState, useEffect } from 'react';
import { BrowserRouter as Router, Routes, Route } from "react-router-dom";
import { delay } from './Utils/utils'
import Login from './Pages/Login'
import Register from './Pages/Register'
import Member from './Pages/Member'
import Guest from './Pages/Guest'
import Home from './Pages/Home'
import Welcome from './Pages/Welcome'
import Demo from './Pages/Demo'


function App() {

    return (
        <Router>
            <Routes>
                <Route path="/" element={
                    <Demo />} />
                <Route path="/asdddds" element={<Welcome />} />
                <Route path="/home" element={<Home />} />
                <Route path="/register" element={<Register />} />
                <Route path="/login" element={<Login />} />
                <Route path="/member" element={<Member />} />
                <Route path="/guest" element={<Guest />} />
            </Routes>
        </Router>
    )
}

export default App;