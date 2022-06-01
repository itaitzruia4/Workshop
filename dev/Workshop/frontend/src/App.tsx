import React, { useState, useEffect } from 'react';
import { BrowserRouter as Router, Routes, Route } from "react-router-dom";
import Register from './Pages/Register'
import Welcome from './Pages/Welcome'
import Member from './Pages/Member'
import { ThemeProvider, createTheme } from '@mui/material/styles';
import Login from './Pages/Login'
import CssBaseline from '@mui/material/CssBaseline';


function App() {
    const darkTheme = createTheme({
        palette: {
            mode: 'dark',
        },
    });


    return (
            <ThemeProvider theme={darkTheme}>
            <CssBaseline />
            <Router>
                <Routes>
                    <Route path="/" element={<Welcome />} />    
                    <Route path="/register" element={<Register />} />
                    <Route path="/login" element={<Login />} />
                    <Route path="/member" element={<Member />} />
                </Routes>
            </Router>
            </ThemeProvider>
    )
}

export default App;