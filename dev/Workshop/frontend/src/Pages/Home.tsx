import React from 'react';
import { useNavigate } from "react-router-dom";
import './Home.css';

function Home() {
    const textStyle = { color: 'white' }

    let navigate = useNavigate();
    const routeChange = (path: string) =>
        () => {
            navigate(path);
        }

    return (
        <p className="home">
            <div className="home_title" style={textStyle}> Trading System Homepage</div>
            <p className="home_buttons">
                <button className="home_login_btn" onClick={routeChange('/login')}> Login </button>
                <button className="home_register_btn" onClick={routeChange('/register')}> Register </button>
                <button className="home_cag_btn" onClick={routeChange('/guest')}> Continue as guest </button>
            </p>
        </p>
    )
}


export default Home;