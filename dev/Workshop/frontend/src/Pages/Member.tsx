import * as React from 'react';
import ReactDOM from 'react-dom';
import Button from '@mui/material/Button';
import ButtonGroup from '@mui/material/ButtonGroup';
import Appbar from '../Components/Appbar';
import StoresList from '../Components/storesList'
import { Stack } from '@mui/material';
import { ThemeProvider, createTheme } from '@mui/material/styles';
import { useNavigate, useLocation} from "react-router-dom";
import AddStoreDialog from '../Components/Dialogs/addStoreDialog';
import { useState, useEffect } from 'react';

import { handleLogout, handleExitMarket } from '../Actions/AuthenticationActions';
import {
    handleGetStores, handleNewStore, handleAddProduct, handleCloseStore, handleOpenStore, handleRemoveProduct, handleAddDiscount,
    handleAddProductDiscount, handleAddCategoryDiscount, handleNominateStoreOwner, handleNominateStoreManager, handleRemoveStoreOwnerNomination,
    handleAddProductPurchasePolicy, handleAddCategoryPurchasePolicy, handleAddStorePurchasePolicy, handleAddUserPurchasePolicy
} from '../Actions/StoreActions';
import { handleAddToCart, handleViewCart, handleBuyCart, handleReviewProduct, handleUpdateNotifications, handleEditCart } from '../Actions/UserActions';
import { handleChangeProductCategory, handleChangeProductName, handleChangeProductPrice, handleChangeProductQuantity } from '../Actions/ProductActions';

import { makeUserToken, memberToken, token } from '../Types/roles';
import { Store } from "../Types/store"
import { Product } from "../Types/product"
import { Cart, Bag } from '../Types/shopping';
import { MarketNotification } from '../Types/Notification';

// Member page
function Member() {
    const [refreshKey, setRefreshKey] = useState(0);

    const location = useLocation();
    const token = location.state as memberToken;

    let navigate = useNavigate();
    const routeChange = (path: string, token: token) =>
        () =>
            navigate(path, { state: token });

    const [stores, setStores] = useState<Store[]>([])
    const [cart, setCart] = useState<Cart>({ shoppingBags: [] })
    const [notifications, setNotifications] = useState<MarketNotification[]>(token.notifications);


    const refresh = () => {
        handleGetStores(token).then(value => setStores(value as Store[])).catch(error => alert(error));
        handleViewCart(makeUserToken(token.userId)).then(value => setCart(value as Cart)).catch(error => alert(error));
        if (notifications.length > 0) {
            handleUpdateNotifications(token)
                .then(value => {
                    setNotifications(value as MarketNotification[]);
                })
                .catch(error => alert(error));
        }
    };

    useEffect(() => {
        refresh();
    }, [refreshKey])

    const addStore = (storeName: string) => {
        handleNewStore(token, storeName).then(() => setRefreshKey(oldKey => oldKey + 1)).catch(error => alert(error));
    };
    const addProduct = (storeId: number, productName: string, description: string, price: number, quantity: number, category: string) => {
        handleAddProduct(token, storeId, productName, description, price, quantity, category).then(() => setRefreshKey(oldKey => oldKey + 1)).catch(error => alert(error));
    };
    const removeProduct = (storeId: number, productId: number) => {
        handleRemoveProduct(token, storeId, productId).then(() => setRefreshKey(oldKey => oldKey + 1)).catch(error => alert(error));
    };

    const updateProduct = (storeId: number, productId: number, productName: string, price: number, quantity: number, category: string) => {
        handleChangeProductName(token, storeId, productId, productName).then(() =>
            handleChangeProductPrice(token, storeId, productId, price).then(() =>
                handleChangeProductQuantity(token, storeId, productId, quantity).then(() =>
                    handleChangeProductCategory(token, storeId, productId, category).then(() => setRefreshKey(oldKey => oldKey + 1))))).catch(error => alert(error));
    };

    const reviewProduct = (productId: number, review: string, rating: number) => {
        handleReviewProduct(token, productId, review, rating).then(() => setRefreshKey(oldKey => oldKey + 1)).catch(error => alert(error));
    }

    const addDiscount = (storeId: number, discountJson: string) => {
        handleAddDiscount(token, storeId, discountJson)
            .catch(error => {
                alert(error)
            });
    };

    const addProductDiscount = (storeId: number, productId: number, discountJson: string) => {
        handleAddProductDiscount(token, storeId, productId, discountJson)
            .catch(error => {
                alert(error)
            });
    };

    const addCategoryDiscount = (storeId: number, category: string, discountJson: string) => {
        handleAddCategoryDiscount(token, storeId, category, discountJson)
            .catch(error => {
                alert(error)
            });
    };

    const addProductPurchasePolicy = (storeId: number, productId: number, purchaseJson: string): void => {
        handleAddProductPurchasePolicy(token, storeId, productId, purchaseJson)
            .catch(error => {
                alert(error)
            });
    }

    const addCategoryPurchasePolicy = (storeId: number, category: string, purchaseJson: string): void => {
        handleAddCategoryPurchasePolicy(token, storeId, category, purchaseJson)
            .catch(error => {
                alert(error)
            });
    }

    const addBagPurchasePolicy = (storeId: number, purchaseJson: string): void => {
        handleAddStorePurchasePolicy(token, storeId, purchaseJson)
            .catch(error => {
                alert(error)
            });
    }

    const addUserPurchasePolicy = (storeId: number, purchaseJson: string): void => {
        handleAddUserPurchasePolicy(token, storeId, purchaseJson)
            .catch(error => {
                alert(error)
            });
    }

    const closeStore = (storeId: number) => {
        handleCloseStore(token, storeId).then(() => setRefreshKey(oldKey => oldKey + 1)).catch(error => alert(error));
    };
    const openStore = (storeId: number) => {
        handleOpenStore(token, storeId).then(() => setRefreshKey(oldKey => oldKey + 1)).catch(error => alert(error));
    };

    const nominateStoreOwner = (storeId: number, nominee: string) => {
        handleNominateStoreOwner(token, storeId, nominee)
            .catch(error => {
                alert(error)
            });
    }

    const nominateStoreManager = (storeId: number, nominee: string) => {
        handleNominateStoreManager(token, storeId, nominee)
            .catch(error => {
                alert(error)
            });
    }

    const removeStoreOwnerNomination = (storeId: number, nominee: string) => {
        handleRemoveStoreOwnerNomination(token, storeId, nominee)
            .catch(error => {
                alert(error)
            });
    }

    //cart actions 

    const addToCart = (storeId: number, productId: number, quantity: number) => {
        handleAddToCart(makeUserToken(token.userId), storeId, productId, quantity).then(() => setRefreshKey(oldKey => oldKey + 1)).catch(error => alert(error));
    }
    const buyCart = (number: string, year: string, month: string, ccv: string, holder: string, id: string, name: string, address: string,
        city: string, country: string, zip: string) => { 
        handleBuyCart(makeUserToken(token.userId), number, year, month, ccv, holder, id, name, address, city, country, zip)
            .then(() => setRefreshKey(oldKey => oldKey + 1)).catch(error => alert(error));
    }
    const editCart = (productId: number, quantity: number): void => {
        handleEditCart(makeUserToken(token.userId), productId, quantity).then(() => setRefreshKey(oldKey => oldKey + 1)).catch(error => alert(error));
    }
   
    return (
        <div>
            {Appbar(routeChange,token, token.membername, stores, cart, notifications, editCart, buyCart)}
            <ButtonGroup variant="contained" aria-label="outlined primary button group">
                {AddStoreDialog(addStore)}
            </ButtonGroup>
            {StoresList(stores, addProduct, removeProduct, updateProduct, reviewProduct, closeStore, openStore, addDiscount,
                addProductDiscount, addCategoryDiscount, addProductPurchasePolicy, addCategoryPurchasePolicy,
                addBagPurchasePolicy, addUserPurchasePolicy, addToCart, nominateStoreOwner, nominateStoreManager, removeStoreOwnerNomination)}
            <Stack direction="row" spacing={2}>
                <Button variant='contained' onClick={e =>
                    handleLogout(token)
                        .then(routeChange("/login", token))
                        .catch(error => {
                            alert(error)
                        })
                } >Logout </Button>
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

export default Member;