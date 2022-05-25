import React, { useState } from 'react';
import { useNavigate, useLocation } from "react-router-dom";
import './Register.css';
import { userToken, memberToken, token } from '../Components/roles';
import { handleRegister } from '../Actions/AuthenticationActions';

function Register() {
    const textStyle = { color: 'white' }

    const location = useLocation();
    const token = location.state as userToken;

    let navigate = useNavigate();
    const routeChange = (path: string, token: token) =>
        () =>
            navigate(path, { state: token });

    const [membername, setMembername] = useState("");
    const [password, setPassword] = useState("");
    const [birthDate, setBirthDate] = useState("");   

    return (
        <p className="register">
            <div className="register_title" style={textStyle}> Register </div>
            <p className="register_userInput">
                <div style={textStyle}> Username: </div>
                <input className="register_membername_textbox" type="text" onChange={e => setMembername(e.target.value)} />
                <div style={textStyle}> Password: </div>
                <input className="register_password_textbox" type="password" onChange={e => setPassword(e.target.value)} />
                <div style={textStyle}> Birth Date: </div>
                <input className="register_date_textbox" type="text" placeholder="dd/mm/yyyy" onChange={e => setBirthDate(e.target.value)} />
            </p>
            <p className="register_buttons">
                <button className="register_register_btn"
                    onClick={e =>
                        handleRegister(token, membername, password, birthDate)
                            .then(routeChange("/login", { userId: token.userId }))
                            .catch(error => {
                                alert(error)
                            })
                    } > Register </button>
                <button className="login_back_btn" onClick={routeChange('/home', { userId: token.userId })}> Back to home</button>
            </p>
        </p>
    )
}

export default Register;