import React from 'react';
import { useNavigate } from 'react-router-dom';
import './Guest.css';

function Register() {
    const textStyle = { color: 'white' }
    let navigate = useNavigate();
    const routeChange = (path: string) =>
        () => {
            navigate(path);
        }
    return (
        <p className="guest">
            <p className="guest_searchbars">
                <input className="guest_search_product" type="text" placeholder="Search products" />
            </p>
            <p className="guest_cart">
                <input className="guest_cart_scrollbar" type="text" placeholder="Search products" />
            </p>
            <p className="guest_buttons">
                <button className="guest_register_btn" onClick={routeChange('/register') }> Register </button>
                <button className="guest_login_btn" onClick={routeChange('/login')}> Login </button>
                <button className="guest_exit_btn" onClick={routeChange('/')}> Exit Market </button>
            </p>
        </p>
    )
}

export default Register;