import React, { useState, useEffect } from 'react';
import { BrowserRouter as Router, Routes, Route } from "react-router-dom";
import { delay } from './Utils/utils'
import Login from './Pages/Login'
import Register from './Pages/Register'
import Member from './Pages/Member'
import Guest from './Pages/Guest'
import Home from './Pages/Home'
import Welcome from './Pages/Welcome'
import AddDiscount from './Pages/AddDiscount';
import Demo from './Pages/Demo'
import { ThemeProvider, createTheme } from '@mui/material/styles';


function App() {
    const darkTheme = createTheme({
        palette: {
            mode: 'dark',
        },
    });


    return (
        <ThemeProvider theme={darkTheme}>
        <Router>
            <Routes>
                <Route path="/asdas" element={
                    <Demo />} />
                <Route path="/" element={<Welcome />} />
                <Route path="/home" element={<Home />} />
                <Route path="/register" element={<Register />} />
                <Route path="/login" element={<Login />} />
                <Route path="/member" element={<Member />} />
                <Route path="/guest" element={<Guest />} />
                <Route path="/add-discount" element={<AddDiscount />} />
            </Routes>
        </Router>
        </ThemeProvider>
    )
}

export default App;