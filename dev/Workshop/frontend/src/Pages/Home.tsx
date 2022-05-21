import React from 'react';
import { useNavigate, useLocation } from "react-router-dom";
import { userToken } from '../Components/roles';


import './Home.css';

function Home() {
    type user = {
        userId: number
    }

    const textStyle = { color: 'white' }

    const location = useLocation();
    const token = location.state as userToken;

    let navigate = useNavigate();
    const routeChange = (path: string, userId: number) =>
        () =>
            navigate(path, { state: { userId: userId } });

    

    return (
        <p className="home">
            <div className="home_title" style={textStyle}> Trading System Homepage</div>
            <p className="home_buttons">
                <button className="home_login_btn" onClick={routeChange('/login',token.userId )}> Login </button>
                <button className="home_register_btn" onClick={routeChange('/register', token.userId)}> Register </button>
                <button className="home_cag_btn" onClick={routeChange('/guest', token.userId)}> Continue as guest </button>
            </p>
        </p>
    )
}


export default Home;