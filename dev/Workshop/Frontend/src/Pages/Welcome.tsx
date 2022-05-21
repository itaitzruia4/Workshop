import React, { useState, useEffect } from 'react';
import { useNavigate } from "react-router-dom";
import './Welcome.css';



function Welcome() {

   
    const textStyle = { color: 'white' }
   
    const [userId, setUserId] = useState(0);

    let navigate = useNavigate();
    const routeChange = (path: string, userId: number) =>
        () => 
            navigate(path, { state: { userId: userId } });
        

    function HandleEnterMarket() {
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
                .then(routeChange('/home', userId))
            );
    }

    return (
        <p className="welcome">
            <div className="welcome_title" style={textStyle}> Welcome to the Trading System website! </div>
            <p className="welcome_buttons">
                <button className="welcome_enter_btn" onClick={() => HandleEnterMarket()}> Enter Market </button>
            </p>
        </p>
    )
}


export default Welcome;