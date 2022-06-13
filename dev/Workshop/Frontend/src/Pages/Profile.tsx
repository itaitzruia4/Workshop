import React, { useState, useEffect } from 'react';
import { useNavigate, useLocation } from "react-router-dom";
import { handleEnterMarket } from '../Actions/AuthenticationActions';
import Button from '@mui/material/Button';

import { makeUserToken, memberToken } from '../Types/roles';
import { Store } from "../Types/store"
import { Product } from "../Types/product"
import { Cart, Bag } from '../Types/shopping';
import { MarketNotification } from '../Types/Notification';


function Profile() {

    const textStyle = { color: 'black' }
    const [refreshKey, setRefreshKey] = useState(0);

    const location = useLocation();
    const token = location.state as memberToken;

    let navigate = useNavigate();
    const routeChange = (path: string, token: memberToken) =>
        () =>
            navigate(path, { state: token });

    return (
        <p className="welcome">
            <div className="welcome_title" style={textStyle}> {token.membername + "'s profile" } </div>
            <p className="welcome_buttons">
                <Button variant="contained"> Test </Button>
                <Button variant="contained" onClick={routeChange('/member', token) }> Back to main window </Button>
            </p>
        </p>
    )
}


export default Profile;