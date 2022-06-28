import React, { useState, useEffect } from 'react';
import { useNavigate, useLocation } from "react-router-dom";
import { handleEnterMarket } from '../Actions/AuthenticationActions';
import Button from '@mui/material/Button';
import AppBar from '@mui/material/AppBar';
import Toolbar from '@mui/material/Toolbar';
import Typography from '@mui/material/Typography';
import CloseIcon from '@mui/icons-material/Close';
import IconButton from '@mui/material/IconButton';
import RefreshIcon from '@mui/icons-material/Refresh';
import Tooltip from '@mui/material/Tooltip';
import ListItem from '@mui/material/ListItem';
import List from '@mui/material/List';
import Grid from '@mui/material/Grid';
import Container from '@mui/material/Container';

import AddStoreDialog from '../Components/Dialogs/addStoreDialog';
import StoreCard from '../Components/StoreCard';

import { memberToken, Actions, StorePermission, permissionsById, isManager } from '../Types/roles';
import { Store, storeById, Order, getOrders } from "../Types/store"
import { Product } from "../Types/product"
import { Cart, Bag } from '../Types/shopping';
import { MarketNotification } from '../Types/Notification';


import {
    handleGetStores, handleNewStore, handleAddProduct, handleCloseStore, handleOpenStore, handleRemoveProduct, handleAddDiscount,
    handleAddProductDiscount, handleAddCategoryDiscount, handleNominateStoreOwner, handleNominateStoreManager, handleRemoveStoreOwnerNomination,
    handleAddProductPurchasePolicy, handleAddCategoryPurchasePolicy, handleAddStorePurchasePolicy, handleAddUserPurchasePolicy, handleAddActionToManager,
    handleGetStorePurchaseHistory
} from '../Actions/StoreActions';
import { handleAddToCart, handleViewCart, handleBuyCart, handleReviewProduct, handleUpdateNotifications, handleEditCart, handleGetMemberPermissions } from '../Actions/UserActions';
import { handleChangeProductCategory, handleChangeProductName, handleChangeProductPrice, handleChangeProductQuantity } from '../Actions/ProductActions';

function Profile() {
    const [refreshKey, setRefreshKey] = useState(0);

    const [stores, setStores] = useState<Store[]>([])
    const [permissionsInfo, setPermissionsInfo] = useState<StorePermission[]>([])
    const [orders, setOrders] = useState < {id: number ,orders : Order[]}[] >([])

    const location = useLocation();
    const token = location.state as memberToken;

    let navigate = useNavigate();
    const routeChange = (path: string, token: memberToken) =>
        () =>
            navigate(path, { state: token });

    async function waterfallWithRetry<T>(promises: Promise<T>[]): Promise<T[]> {
        return promises.reduce(
            async (promisesSum: Promise<T[]>, currVal: Promise<T>) => {
                return (await promisesSum).concat([(await currVal)])
            }
            , Promise.resolve([])
        )
    }
    const refresh = () => {
        handleGetMemberPermissions(token).then(value => { setPermissionsInfo(value as StorePermission[])}).catch(error => alert(error));
        handleGetStores(token).then((values : Store[]) => {
            const newOrders = values.map(store => {
                return handleGetStorePurchaseHistory(token, store.storeId).then(value => { return { id: store.storeId, orders: value as Order[] } })
            })
            waterfallWithRetry(newOrders).then(value => setOrders(value))         
            setStores(values as Store[])
        }).catch(error => alert(error));      
    };

    const addStore = (storeName: string) => {
        handleNewStore(token, storeName).then(() => setRefreshKey(oldKey => oldKey + 1)).catch(error => alert(error));
    };

    const closeStore = (storeId: number) => {
        handleCloseStore(token, storeId).then(() => setRefreshKey(oldKey => oldKey + 1)).catch(error => alert(error));
    };
    const openStore = (storeId: number) => {
        handleOpenStore(token, storeId).then(() => setRefreshKey(oldKey => oldKey + 1)).catch(error => alert(error));
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

    const addActionToManager = (nominee: string, storeId: number, action: string) => {
        handleAddActionToManager(token,nominee ,storeId, action)
            .catch(error => {
                alert(error)
            });
    }

    const getStorePurchaseHistory = (storeId: number) => {
        handleGetStorePurchaseHistory(token, storeId)
            .catch(error => {
                alert(error)
            });
    }

    useEffect(() => {
        refresh();
    }, [refreshKey])

    return (
        <Container>
            <AppBar sx={{ position: 'relative' }}>
                <Toolbar>
                    <IconButton
                        edge="start"
                        color="inherit"
                        onClick={() => (permissionsInfo.length > 0 && permissionsInfo[0].storeId === -1) ? routeChange('/admin', token)()  : routeChange('/member', token)()}
                        aria-label="close"
                    >
                        <CloseIcon />
                    </IconButton>
                    <Typography sx={{ ml: 2, flex: 1 }} variant="h6" component="div">
                        {token.membername + "'s profile"}
                    </Typography>
                    <Typography sx={{ ml: 2, flex: 1 }} variant="button" component="div">
                        {"userId("+ token.userId + ")"}
                    </Typography>
                    {AddStoreDialog(addStore)}
                    <IconButton autoFocus color="inherit" onClick={() => setRefreshKey(oldKey => oldKey + 1)}>
                        <RefreshIcon />
                    </IconButton>                 
                </Toolbar>
            </AppBar>
            <Typography sx={{ ml: 2, flex: 1 }} variant="h3" component="div">Your stores</Typography>
           <Grid container item spacing={3}>
                {stores.map(store => {
                    const permissions = permissionsById(store.storeId, permissionsInfo);
                    return (
                        
                        isManager(store.storeId, permissionsInfo) || permissions.length > 0 ?
                            <Grid item >
                                <StoreCard store={store} permissions={permissions} orders={getOrders(orders, store.storeId ) }
                                    closeStore={closeStore} openStore={openStore} addProduct={addProduct}
                                    removeProduct={removeProduct} updateProduct={updateProduct} reviewProduct={reviewProduct}
                                    addDiscount={addDiscount} addProductDiscount={addProductDiscount} addCategoryDiscount={addCategoryDiscount}
                                    addProductPurchasePolicy={addProductPurchasePolicy} addCategoryPurchasePolicy={addCategoryPurchasePolicy}
                                    addBagPurchasePolicy={addBagPurchasePolicy} addUserPurchasePolicy={addUserPurchasePolicy}
                                    nominateStoreOwner={nominateStoreOwner} nominateStoreManager={nominateStoreManager}
                                    removeStoreOwnerNomination={removeStoreOwnerNomination} addActionToManager={addActionToManager} getStorePurchaseHistory={getStorePurchaseHistory } 
                                />
                        </Grid> : null)})}
            </Grid>
        </Container>
    )
}


export default Profile;