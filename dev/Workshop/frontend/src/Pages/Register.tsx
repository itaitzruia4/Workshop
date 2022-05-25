import React, { useState } from 'react';
import { useNavigate, useLocation } from "react-router-dom";
import './Register.css';
import { userToken, memberToken, token } from '../Components/roles';

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

    function handleRegister(event: React.MouseEvent<HTMLButtonElement, MouseEvent>) {
        if (membername === "" || password === "" ||  birthDate == "") {
            alert('User details must not be empty');
            return;
        }

        var date_regex = /\d{1,2}\/\d{1,2}\/\d{4}/;
        if (!(date_regex.test(birthDate))) {
            alert("Invalid date format");
            return false;
        }

        let url = "http://localhost:5165/api/authentication/register";

        fetch(url, {
            method: 'POST',
            mode: 'cors',
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({
                userId: token.userId,
                membername: membername,
                password: password,
                birthDate: birthDate
            })
        }).then((response) =>
            response.json()
                .then((data) => response.ok ? null :
                    alert(data.error))
                .then(routeChange("/login", { userId: token.userId}))
        );
    }
    

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
                <button className="register_register_btn" onClick={e => handleRegister(e)} > Register </button>
                <button className="login_back_btn" onClick={routeChange('/home', { userId: token.userId })}> Back to home</button>
            </p>
        </p>
    )
}

export default Register;