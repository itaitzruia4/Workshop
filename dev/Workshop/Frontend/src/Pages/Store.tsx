import React from 'react';
import { useNavigate } from "react-router-dom";
import './Store.css';

function Store() {
    const textStyle = { color: 'white' }

    let navigate = useNavigate();
    const routeChange = (path: string) =>
        () => {
            navigate(path);
        }

    return (
        <div style={{ color: 'white'}}>Hi!</div>
    )
}


export default Store;