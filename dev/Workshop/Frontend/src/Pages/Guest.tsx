import * as React from 'react';
import ReactDOM from 'react-dom';
import Button from '@mui/material/Button';
import ButtonGroup from '@mui/material/ButtonGroup';
import Appbar from '../Components/Appbar';
import StoresList from '../Components/storesList'
import { Stack } from '@mui/material';
import { ThemeProvider, createTheme } from '@mui/material/styles';
import { useNavigate, useLocation } from "react-router-dom";
import AddStoreDialog from '../Components/Dialogs/addStoreDialog';
import { useState, useEffect } from 'react';

import { handleLogout, handleExitMarket } from '../Actions/AuthenticationActions';
import { handleGetStores, handleNewStore, handleAddProduct, handleCloseStore, handleOpenStore, handleRemoveProduct, handleAddDiscount, handleAddProductDiscount, handleAddCategoryDiscount } from '../Actions/StoreActions';
import { handleAddToCart, handleViewCart, handleReviewProduct } from '../Actions/UserActions';
import { handleChangeProductCategory, handleChangeProductName, handleChangeProductPrice, handleChangeProductQuantity } from '../Actions/ProductActions';

import { userToken } from '../Types/roles';
import { Store } from "../Types/store"
import { Product } from "../Types/product"
import { Cart, Bag } from '../Types/shopping';


function Guest() {
    const [refreshKey, setRefreshKey] = useState(0);

    const location = useLocation();
    const token = location.state as userToken;

    let navigate = useNavigate();
    const routeChange = (path: string, token: userToken) =>
        () =>
            navigate(path, { state: token });

    const [stores, setStores] = useState<Store[]>([])
    const [cart, setCart] = useState<Cart>({ shoppingBags: [] })


    const refresh = () => {
        handleGetStores(token).then(value => setStores(value as Store[])).catch(error => alert(error));
        handleViewCart(token).then(value => setCart(value as Cart)).catch(error => alert(error));
    };

    useEffect(() => {
        refresh();
    }, [refreshKey])

    const addStore = (storeName: string) => {};
    const addProduct = (storeId: number, productName: string, description: string, price: number, quantity: number, category: string) => {};
    const removeProduct = (storeId: number, productId: number) => {};

    const updateProduct = (storeId: number, productId: number, productName: string, price: number, quantity: number, category: string) => {};

    const reviewProduct = (productId: number, review: string, rating: number) => { };

    const addDiscount = (storeId: number, discountJson: string) => {};

    const addProductDiscount = (storeId: number, productId: number, discountJson: string) => {};

    const addCategoryDiscount = (storeId: number, category: string, discountJson: string) => {};

    const closeStore = (storeId: number) => {};
    const openStore = (storeId: number) => {};

    //cart actions 

    const addToCart = (storeId: number, productId: number, quantity: number) => {
        handleAddToCart(token, storeId, productId, quantity).then(() => setRefreshKey(oldKey => oldKey + 1)).catch(error => alert(error));
    }

    return (
        <div>
            {Appbar(token, "guest", stores, cart)}
            {StoresList(stores, addProduct, removeProduct, updateProduct, reviewProduct, closeStore, openStore, addDiscount, addProductDiscount, addCategoryDiscount, addToCart)}
            <Stack direction="row" spacing={2}>
                <Button variant='contained' onClick={e =>
                    handleExitMarket(token)
                        .then(routeChange("/", token))
                        .catch(error => {
                            alert(error)
                        })
                } >Exit market </Button>
            </Stack>
        </div>
    );

}

export default Guest;