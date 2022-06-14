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

import AddStoreDialog from '../Components/Dialogs/addStoreDialog';
import StoreCard from '../Components/StoreCard';

import { memberToken, Actions, StorePermission, permissionsById } from '../Types/roles';
import { Store, storeById } from "../Types/store"
import { Product } from "../Types/product"
import { Cart, Bag } from '../Types/shopping';
import { MarketNotification } from '../Types/Notification';


import {
    handleGetStores, handleNewStore, handleAddProduct, handleCloseStore, handleOpenStore, handleRemoveProduct, handleAddDiscount,
    handleAddProductDiscount, handleAddCategoryDiscount, handleNominateStoreOwner, handleNominateStoreManager, handleRemoveStoreOwnerNomination,
    handleAddProductPurchasePolicy, handleAddCategoryPurchasePolicy, handleAddStorePurchasePolicy, handleAddUserPurchasePolicy
} from '../Actions/StoreActions';
import { handleAddToCart, handleViewCart, handleBuyCart, handleReviewProduct, handleUpdateNotifications, handleEditCart, handleGetMemberPermissions } from '../Actions/UserActions';
import { handleChangeProductCategory, handleChangeProductName, handleChangeProductPrice, handleChangeProductQuantity } from '../Actions/ProductActions';

function Profile() {
    const [refreshKey, setRefreshKey] = useState(0);
    const textStyle = { color: 'black' }

    const [stores, setStores] = useState<Store[]>([])
    const [permissionsInfo, setPermissionsInfo] = useState<StorePermission[]>([])
    const [notifications, setNotifications] = useState<MarketNotification[]>([]);

    const location = useLocation();
    const token = location.state as memberToken;

    let navigate = useNavigate();
    const routeChange = (path: string, token: memberToken) =>
        () =>
            navigate(path, { state: token });

    const refresh = () => {
        handleGetMemberPermissions(token).then(value => { setPermissionsInfo(value as StorePermission[])}).catch(error => alert(error));
        handleGetStores(token).then(value => setStores(value as Store[])).catch(error => alert(error));
    };

    const addStore = (storeName: string) => {
        handleNewStore(token, storeName).then(() => setRefreshKey(oldKey => oldKey + 1)).catch(error => alert(error));
    };

    useEffect(() => {
        refresh();
    }, [refreshKey])

    return (
        <p className="welcome">
            <AppBar sx={{ position: 'relative' }}>
                <Toolbar>
                    <IconButton
                        edge="start"
                        color="inherit"
                        onClick={routeChange('/member', token)}
                        aria-label="close"
                    >
                        <CloseIcon />
                    </IconButton>
                    <Typography sx={{ ml: 2, flex: 1 }} variant="h6" component="div">
                        {token.membername + "'s profile"}
                    </Typography>
                    {AddStoreDialog(addStore)}
                    <IconButton autoFocus color="inherit" onClick={() => setRefreshKey(oldKey => oldKey + 1)}>
                        <RefreshIcon />
                    </IconButton>                 
                </Toolbar>
            </AppBar>
            <p className="welcome_buttons">
                <Button variant="contained" onClick={() => console.log(permissionsInfo) }> Test </Button>
            </p>
            <Grid container item spacing={3}>
                {stores.map(store => {
                    return ( 
                        <Grid item xs={4} sx={{ margin: 1}}>
                            {StoreCard(store, permissionsById(store.storeId, permissionsInfo).permissions)}
                        </Grid>)})}
            </Grid>
        </p>
    )
}


export default Profile;