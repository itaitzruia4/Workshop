import React from 'react';
import { useNavigate } from "react-router-dom";
import './Register.css';

function Register() {
    const textStyle = { color: 'white' }

    let navigate = useNavigate();
    const routeChange = (path: string) =>
        () => {
            navigate(path);
        }

    return (
        <p className="register">
            <div className="register_title" style={textStyle}> Register </div>
            <p className="register_userInput">
                <div style={textStyle}> Username: </div>
                <input className="register_username_textbox" type="text" />
                <div style={textStyle}> Password: </div>
                <input className="register_password_textbox" type="password" />
                <div style={textStyle}> Name: </div>
                <input className="register_name_textbox" type="text" />
                <div style={textStyle}> Birth Date: </div>
                <input className="register_date_textbox" type="text" placeholder="dd/mm/yyyy" />
            </p>
            <p className="register_buttons">
                <button className="register_register_btn"> Register </button>
                <button className="login_back_btn" onClick={routeChange('/')}> Back to home</button>
            </p>
        </p>
    )
}

export default Register;