import React from 'react';
import { useNavigate } from "react-router-dom";
import './Login.css';

function Login() {
    const textStyle = { color: 'white' }

    let navigate = useNavigate();
    const routeChange = (path: string) =>
        () => {
            navigate(path);
        }

    return (
        <p className="login">
            <div className="login_title" style={textStyle}> Login </div>
            <p className="login_userInput">
                <div style={textStyle}> Username: </div>
                <input className="login_username_textbox" type="text" />
                <div style={textStyle}> Password: </div>
                <input className="login_password_textbox" type="password" />
            </p>
            <p className="login_buttons">
                <button className="login_login_btn" onClick={routeChange('/member')} > Login</button>
                <button className="login_back_btn" onClick={routeChange('/')}> Back to home</button>
            </p>
        </p>
    )
}


export default Login;