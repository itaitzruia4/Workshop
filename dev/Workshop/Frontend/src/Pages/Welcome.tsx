import React, { useState, useEffect } from 'react';
import { useNavigate } from "react-router-dom";
import './Home.css';

function Welcome() {
    
    const textStyle = { color: 'white' }
   
    const [userId, setUserId] = useState(0);

    let navigate = useNavigate();
    const routeChange = (path: string) =>
        () => {
            navigate(path);
        }
    /*
    useEffect(() => {
        fetch("http://localhost:5165/api/authentication/entermarket", {
            method: 'GET',
            mode: 'cors',
            headers: { "Content-Type": "application/json" }
        })
            .then((res) => res.json())
            .then((data) => setUserId(data.value))
            .then(() => localStorage.setItem("userId", JSON.stringify(userId)));
    }, []);
    */

    function HandleEnterMarket(e: React.MouseEvent<HTMLButtonElement, MouseEvent>) {
        let url = "http://localhost:5165/api/authentication/entermarket";

        fetch(url, {
            method: 'GET',
            mode: 'cors',
            headers: { "Content-Type": "application/json" }
        })
            .then((res) => res.json()
                .then((data) => {
                    if (res.ok)
                        setUserId(data.value)
                })
                .then(routeChange('/home'))
            );
    }

    return (
        <p className="welcome">
            <div className="welcome_title" style={textStyle}> Welcome to the Trading System website! </div>
            <p className="welcome_buttons">
                <button className="welcome_enter_btn" onClick={routeChange('/home')}> Enter Market </button>
            </p>
        </p>
    )
}


export default Welcome;