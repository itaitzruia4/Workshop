import React from 'react';
import { Link} from "react-router-dom";
import './App.css';

function App() {
    const textStyle = { color: 'white' }


    return (
        <p className="all">
            <div className="welcome_msg" style={textStyle}> Welcome to the Trading System website! </div>
            <p className="userInput">
                <div style={textStyle}> Username: </div>
                <input className="username_textbox" type="text" />
                <div style={textStyle}> Password: </div>
                <input className="password_textbox" type="password" />
            </p>
            <p className="buttons">
                <button className="login_btn"> Login </button>
                <button className="register_btn">
                    {/* <Link to="/RegisterWindow">Route Name</Link> */}
                    Register
                </button>
                <button className="cag_btn"> Continue as guest </button> 
            </p>
        </p>
    )
}


export default App;