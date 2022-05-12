import React, { useState, useEffect } from 'react';
import { useNavigate } from "react-router-dom";
import './Login.css';
import { HubConnectionBuilder, HubConnection } from '@microsoft/signalr';

function Login() {
    const textStyle = { color: 'white' }

    let navigate = useNavigate();
    const routeChange = (path: string) =>
        () => {
            navigate(path);
        }

    const [username, setUsername] = useState("");
    const [password, setPassword] = useState("");

    const handleUserDetails = (): void => {
        if (username === "" || password === "") {
            // TODO show error message
        }
    }

    return (
        <p className="login">
            <div className="login_title" style={textStyle}> Login </div>
            <p className="login_userInput">
                <div style={textStyle}> Username: </div>
                <input className="login_username_textbox" type="text" onChange={e => setUsername(e.target.value)} />
                <div style={textStyle}> Password: </div>
                <input className="login_password_textbox" type="password" onChange={e => setPassword(e.target.value)} />
            </p>
            <p className="login_buttons">
                <button className="login_login_btn" onClick={handleUserDetails} > Login </button>
                <button className="login_back_btn" onClick={routeChange('/')}> Back to home </button>
            </p>
        </p>
    )
}


export default Login;