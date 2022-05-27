import React, { useState, useEffect } from 'react';
import { useNavigate } from "react-router-dom";
import { handleEnterMarket } from '../Actions/AuthenticationActions';
import Button from '@mui/material/Button';



function Welcome() {

   
    const textStyle = { color: 'white' }
   
    const [userId, setUserId] = useState(0);

    let navigate = useNavigate();
    const routeChange = (path: string, userId: number) =>
        () => 
            navigate(path, { state: { userId: userId } });
        
    return (
        <p className="welcome">
            <div className="welcome_title" style={textStyle}> Welcome to the Trading System website! </div>
            <p className="welcome_buttons">
                <Button variant="contained"
                    onClick={() =>
                        handleEnterMarket()
                            .then(value => setUserId(value)).then(routeChange('/login', userId))
                            .catch(error => {
                                alert("Couldnt connect to server")
                            })
                    }> Enter Market </Button>
            </p>
        </p>
    )
}


export default Welcome;