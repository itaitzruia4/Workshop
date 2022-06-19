import * as React from 'react';
import Box from '@mui/material/Box';
import Card from '@mui/material/Card';
import CardActions from '@mui/material/CardActions';
import CardContent from '@mui/material/CardContent';
import Button from '@mui/material/Button';
import Typography from '@mui/material/Typography';
import Switch from '@mui/material/Switch';
import Stack from '@mui/material/Stack';


import { memberToken, Actions, StorePermission } from '../Types/roles';
import { Store } from "../Types/store"
import { Product } from "../Types/product"
import { Cart, Bag } from '../Types/shopping';
import { MarketNotification } from '../Types/Notification';

import StoreAdjust from './Dialogs/StoreAdjust'

export default function StoreCard(
    props: {
        store: Store,
        actions: Actions[],
        closeStore: (storeId: number) => void,
        openStore: (storeId: number) => void,
        addProduct: (storeId: number, productName: string, description: string, price: number, quantity: number, category: string) => void,
        removeProduct: (storeId: number, productId: number) => void,
        updateProduct: (storeId: number, productId: number, productName: string, price: number, quantity: number, category: string) => void,
        reviewProduct: (productId: number, review: string, rating: number) => void,
    }) {
    const { store, actions, closeStore, openStore, addProduct ,removeProduct, updateProduct, reviewProduct } = props
    const [checked, setChecked] = React.useState(store.open);
    const handleChange = (event: React.ChangeEvent<HTMLInputElement>) => {
        console.log(checked)
        console.log(store.storeId)
        checked ? closeStore(store.storeId) : openStore(store.storeId);
        setChecked(event.target.checked);

    }; 
    return (
        <Card sx={{
            minWidth: 275, backgroundColor: '#e3f2fd' }}>
            <CardContent>
                <Typography sx={{ fontSize: 22 }} color="text.primary" gutterBottom>
                    {store.name }
                </Typography>
                <Typography sx={{ fontSize: 14 }} color="text.secondary" gutterBottom>
                    {'store id: ' + store.storeId}
                </Typography>
                <Typography sx={{ mb: 1.5 }} color="text.secondary">
                    {actions}
                </Typography>
                <Stack direction="row" spacing={1} alignItems="center">
                    <Typography>Close</Typography>
                    <Switch
                        checked={checked}
                        onChange={handleChange}
                    />
                    <Typography>Open</Typography>
                </Stack>
            </CardContent>
            <CardActions>
                <StoreAdjust store={store} actions={actions} addProduct={addProduct}
                    removeProduct={removeProduct} updateProduct={updateProduct} reviewProduct={reviewProduct} />
            </CardActions>
        </Card>
    )
}
