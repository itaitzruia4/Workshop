import * as React from 'react';
import ReactDOM from 'react-dom';
import Button from '@mui/material/Button';
import ButtonGroup from '@mui/material/ButtonGroup';
import Appbar from './Appbar';
import StoresList from '../Components/storesList'
import { Stack } from '@mui/material';
import { ThemeProvider, createTheme } from '@mui/material/styles';
import { useNavigate, useLocation} from "react-router-dom";
import AddStoreDialog from '../Components/Dialogs/addStoreDialog';
import { useState, useEffect } from 'react';

import { handleLogout, handleExitMarket } from '../Actions/AuthenticationActions';
import { handleGetStores, handleNewStore, handleAddProduct, handleCloseStore } from '../Actions/StoreActions';

import { memberToken } from '../Types/roles';
import { Store } from "../Types/store"
import { Product } from "../Types/product"


function Member() {
    const [refreshKey, setRefreshKey] = useState(0);

    const location = useLocation();
    const token = location.state as memberToken;

    let navigate = useNavigate();
    const routeChange = (path: string, token: memberToken) =>
        () =>
            navigate(path, { state: token });

    const [stores, setStores] = useState<Store[]>([])

    const refresh = () => {
        handleGetStores(token).then(value => setStores(value as Store[])).catch(error => alert(error));
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
    const closeStore = (storeId: number) => {
        handleCloseStore(token, storeId).then(() => setRefreshKey(oldKey => oldKey + 1)).catch(error => alert(error));
    }

    return (
            <div>
            <Appbar />
            <ButtonGroup variant="outlined" aria-label="outlined button group">
                {AddStoreDialog(addStore)}
            </ButtonGroup>
            {StoresList(stores, addProduct, closeStore)}
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