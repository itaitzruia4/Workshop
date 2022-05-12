import React, { useState } from 'react';
import { useNavigate } from "react-router-dom";
import './Login.css';

function Login() {
    const textStyle = { color: 'white' }

    let navigate = useNavigate();
    const routeChange = (path: string) =>
        () => {
            navigate(path);
        }

    const [username, setUsername] = useState("");
    const [password, setPassword] = useState("");

    const handleUserDetails = () => {
        if (username === "" || password === "") {
            alert('User details must not be empty');
            return;
        }

        let config = {
            method: 'POST',
            headers: { 'Content-type': 'application/json' },
            body: JSON.stringify({ username: username, password: password })
        }

        let url = 'http://localhost:3000/authentication';


        return fetch(url, config)
            .then(
                async (response) =>
                    await response
                        .json()
                        .then((responsePayload) => ({ responsePayload, response }))
            )
            .then(({ responsePayload, response }) => {
                if (response.ok) {
                    routeChange('/users/' + responsePayload.UserId);
                }
                else {
                    alert(responsePayload.Error);
                }
            });
        
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