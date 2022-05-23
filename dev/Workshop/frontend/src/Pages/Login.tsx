import React, { useState} from 'react';
import { useNavigate, useLocation} from "react-router-dom";
import './Login.css';
import { userToken, token } from '../Components/roles';
import { handleLogin } from '../Actions/AuthenticationActions';

function Login() {

    const textStyle = { color: 'white' }

    const location = useLocation();
    const token = location.state as userToken;

    let navigate = useNavigate();
    const routeChange = (path: string, token: token) =>
        () =>
            navigate(path, { state: token });

    const [membername, setMembername] = useState("");
    const [password, setPassword] = useState("");

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
                <button className="login_login_btn" type="submit" onClick={e =>
                    handleLogin(token, membername, password)
                        .then(routeChange("/member", { userId: token.userId, membername: membername }))
                        .catch(error => {
                            alert(error)
                        })
                } > Login </button>
                <button className="login_back_btn" onClick={routeChange('/home', { userId: token.userId })}> Back to home </button>
            </p>
        </div>
    )
}


export default Login;