import React, { useState, useEffect } from 'react';
import { BrowserRouter as Router, Routes, Route } from "react-router-dom";
import Register from './Pages/Register'
import Guest from './Pages/Guest'
import Welcome from './Pages/Welcome'
import AddDiscount from './Pages/AddDiscount';
import Member from './Pages/Member'
import { ThemeProvider, createTheme } from '@mui/material/styles';
import Login from './Pages/Login'


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
                <Route path="/" element={<Welcome />} />    
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