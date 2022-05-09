import React from 'react';
import { BrowserRouter as Router, Routes, Route} from "react-router-dom";
import Login from './Pages/Login'
import Register from './Pages/Register'
import Member from './Pages/Member'
import Guest from './Pages/Guest'
import Home from './Pages/Home'
import Store from './Pages/Store'

function App() {
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