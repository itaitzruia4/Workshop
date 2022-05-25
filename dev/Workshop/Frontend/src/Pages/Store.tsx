import React from 'react';
import { useNavigate, useLocation } from "react-router-dom";
import './Store.css';
import { Store, Stores } from "../Components/store"
import { Product } from "../Components/product"
import { token } from '../Components/roles';
import { handleLogout, handleExitMarket } from '../Actions/AuthenticationActions';
import { handleGetStores, handleNewStore } from '../Actions/StoreActions';

function Store() {
    const textStyle = { color: 'white' }

    const location = useLocation();
    const token = location.state as token;

    let navigate = useNavigate();
    const routeChange = (path: string, token: token) =>
        () =>
            navigate(path, { state: token });

    return (
            <button className="welcome_enter_btn"
               > Enter Market </button>
    )
}

export default Store;