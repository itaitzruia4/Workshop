import React, { useState, useEffect } from 'react';
import { BrowserRouter as Router, Routes, Route } from "react-router-dom";
import Register from './Pages/Register'
import Welcome from './Pages/Welcome'
import Member from './Pages/Member'
import Guest from './Pages/Guest'
import Profile from './Pages/Profile'
import { ThemeProvider, createTheme } from '@mui/material/styles';
import Login from './Pages/Login'
import CssBaseline from '@mui/material/CssBaseline';
import Grid from '@mui/material/Grid';
import Admin from './Pages/Admin';


function App() {
    const darkTheme = createTheme({
        palette: {
            mode: 'light',
        },
    });


    return (
        <ThemeProvider theme={darkTheme}>
            <Grid>
                
                <Router>
                <Routes>
                    <Route path="/" element={<Welcome />} />    
                    <Route path="/register" element={<Register />} />
                    <Route path="/login" element={<Login />} />
                    <Route path="/member" element={<Member />} />
                    <Route path="/guest" element={<Guest />} />\
                    <Route path="/profile" element={<Profile />} />
                    <Route path="/admin" element={<Admin />} />
                </Routes>
                </Router>
             </Grid>
        </ThemeProvider>
    )
}

export default App;