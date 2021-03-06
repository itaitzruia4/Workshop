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
            navigate(path, { state: token });
        }
        
    return (
        <div>
            <div className="welcome_title" style={textStyle}> Welcome to the Trading System website! </div>
                <Button variant="contained"
                    onClick={() =>
                        handleEnterMarket()
                            .then(value => routeChange('/login', makeUserToken(value as number))())
                            .catch(error => {
                                alert("Couldn't connect to server")
                            })
                    }> Enter Market </Button>
            </div>
    )
}


export default Welcome;