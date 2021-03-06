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

import { handleGetStores, handleNewStore, handleAddProduct, handleCloseStore, handleOpenStore, handleRemoveProduct, handleAddDiscount, handleAddProductDiscount, handleAddCategoryDiscount } from '../Actions/StoreActions';
import { handleAddToCart, handleViewCart, handleReviewProduct, handleBuyCart, handleEditCart } from '../Actions/UserActions';
import { handleChangeProductCategory, handleChangeProductName, handleChangeProductPrice, handleChangeProductQuantity } from '../Actions/ProductActions';

import { userToken, makeUserToken ,token} from '../Types/roles';
import { Store } from "../Types/store"
import { Product } from "../Types/product"
import { Cart, Bag } from '../Types/shopping';
import { handleExitMarket } from '../Actions/AuthenticationActions';


function Guest() {
    const [refreshKey, setRefreshKey] = useState(0);

    const location = useLocation();
    const token = location.state as userToken;

    let navigate = useNavigate();
    const routeChange = (path: string, token: token) =>
        () =>
            navigate(path, { state: token });

    const [stores, setStores] = useState<Store[]>([])
    const [cart, setCart] = useState<Cart>({ shoppingBags: [] , price: 0})


    const refresh = () => {
        handleGetStores(token).then(value => setStores(value as Store[])).catch(error => alert(error));
        handleViewCart(token).then(value => setCart(value as Cart)).catch(error => alert(error));
    };

    useEffect(() => {
        refresh();
    }, [refreshKey])

    const reviewProduct = (productId: number, review: string, rating: number) => {
        alert("Permission Denied");
    };

    //cart actions 

    const addToCart = (storeId: number, productId: number, quantity: number) => {
        handleAddToCart(token, storeId, productId, quantity).then(() => setRefreshKey(oldKey => oldKey + 1)).catch(error => alert(error));
    }
    const editCart = (productId: number, quantity: number) => {
        handleEditCart(makeUserToken(token.userId), productId, quantity).then(() => setRefreshKey(oldKey => oldKey + 1)).catch(error => alert(error));
    }
    const buyCart = (number: string, year: string, month: string, ccv: string, holder: string, id: string, name: string, address: string,
        city: string, country: string, zip: string) => {
        handleBuyCart(makeUserToken(token.userId), number, year, month, ccv, holder, id, name, address, city, country, zip)
            .then(() => setRefreshKey(oldKey => oldKey + 1)).catch(error => alert(error));

    }

    return (
        <div>
            {Appbar(routeChange,token, "guest", stores, cart, [], editCart,buyCart)}
            {StoresList(stores, reviewProduct, addToCart)}
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