import React, { useState, useEffect } from 'react';
import { useNavigate } from "react-router-dom";
import { handleEnterMarket } from '../Actions/AuthenticationActions';
import Button from '@mui/material/Button';
import { makeUserToken, userToken } from '../Types/roles';



function Welcome() {

   
    const textStyle = { color: 'black' }
   
    //const [userId, setUserId] = useState(0);

    let navigate = useNavigate();
    const routeChange = (path: string, token: userToken) =>
        () => {
            console.log("navigating from enter market to login page, user id:", token.userId)
            navigate(path, { state: token });
        }
        
    return (
        <p className="welcome">
            <div className="welcome_title" style={textStyle}> Welcome to the Trading System website! </div>
            <p className="welcome_buttons">
                <Button variant="contained"
                    onClick={() =>
                        handleEnterMarket()
                            .then(value => { console.log("enter market user id:", value); return value; })
                            .then(value => routeChange('/login', makeUserToken(value as number))())
                            .catch(error => {
                                alert("Couldnt connect to server")
                            })
                    }> Enter Market </Button>
            </p>
        </p>
    )
}


export default Welcome;