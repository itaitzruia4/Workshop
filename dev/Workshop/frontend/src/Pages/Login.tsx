import React, { useState} from 'react';
import { useNavigate, useLocation} from "react-router-dom";
import './Login.css';
import { userToken } from '../Components/roles';

function Login() {

    const textStyle = { color: 'white' }

    const location = useLocation();
    const token = location.state as userToken;

    let navigate = useNavigate();
    const routeChange = (path: string, userId: number) =>
        () =>
            navigate(path, { state: { userId: userId } });

    const [membername, setMembername] = useState("");
    const [password, setPassword] = useState("");

    function handleUserDetails(event: React.MouseEvent<HTMLButtonElement, MouseEvent>) {
        if (membername === "" || password === "") {
            alert('User details must not be empty');
            return;
        }

        let url = "http://localhost:5165/api/authentication/login";

        fetch(url, {
            method: 'POST',
            mode: 'cors',
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({
                userId: token.userId,
                membername: membername,
                password: password,
            })
        }).then((res) => res.json()
            .then((data) => res.ok ? null : alert(data.error)))
            .then(routeChange("/member", token.userId))
    }

    return (
        <div className="login">
            <div className="login_title" style={textStyle}> Login </div>
            <p className="login_userInput">
                <div style={textStyle}> Username: </div>
                <input className="login_membername_textbox" type="text" onChange={e => setMembername(e.target.value)} />
                <div style={textStyle}> Password: </div>
                <input className="login_password_textbox" type="password" onChange={e => setPassword(e.target.value)} />
            </p>
            <p className="login_buttons">
                <button className="login_login_btn" type="submit" onClick={e => handleUserDetails(e)} > Login </button>
                <button className="login_back_btn" onClick={routeChange('/home', token.userId)}> Back to home </button>
            </p>
        </div>
    )
}


export default Login;