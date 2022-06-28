import * as React from 'react';
import Box from '@mui/material/Box';
import Card from '@mui/material/Card';
import CardActions from '@mui/material/CardActions';
import CardContent from '@mui/material/CardContent';
import Button from '@mui/material/Button';
import Typography from '@mui/material/Typography';
import Switch from '@mui/material/Switch';
import Stack from '@mui/material/Stack';


import { memberToken, Actions, StorePermission, hasPermission } from '../Types/roles';
import { Store , Order} from "../Types/store"
import { Product } from "../Types/product"
import { Cart, Bag } from '../Types/shopping';
import { MarketNotification } from '../Types/Notification';

import StoreAdjust from './Dialogs/StoreAdjust'

export default function StoreCard(
    props: {
        store: Store,
        permissions: Actions[],
        orders: Order[]
        closeStore: (storeId: number) => void,
        openStore: (storeId: number) => void,
        addProduct: (storeId: number, productName: string, description: string, price: number, quantity: number, category: string) => void,
        removeProduct: (storeId: number, productId: number) => void,
        updateProduct: (storeId: number, productId: number, productName: string, price: number, quantity: number, category: string) => void,
        reviewProduct: (productId: number, review: string, rating: number) => void,
        addDiscount: (storeId: number, discountJson: string) => void,
        addProductDiscount: (storeId: number, productId: number, discountJson: string) => void,
        addCategoryDiscount: (storeId: number, category: string, discountJson: string) => void,
        addProductPurchasePolicy: (storeId: number, productId: number, purchaseJson: string) => void,
        addCategoryPurchasePolicy: (storeId: number, category: string, purchaseJson: string) => void,
        addBagPurchasePolicy: (storeId: number, purchaseJson: string) => void,
        addUserPurchasePolicy: (storeId: number, purchaseJson: string) => void,
        nominateStoreOwner: (storeId: number, nominee: string) => void,
        nominateStoreManager: (storeId: number, nominee: string) => void,
        removeStoreOwnerNomination: (storeId: number, nominee: string) => void,
        addActionToManager: (nominee: string, storeId: number, action: string) => void,
        getStorePurchaseHistory: (storeId: number) => void
    }) {
    const { store, permissions, orders, closeStore, openStore, addProduct, removeProduct, updateProduct, reviewProduct,
        addDiscount, addProductDiscount, addCategoryDiscount, addProductPurchasePolicy, addCategoryPurchasePolicy,
        addBagPurchasePolicy, addUserPurchasePolicy, nominateStoreOwner, nominateStoreManager, removeStoreOwnerNomination, addActionToManager, getStorePurchaseHistory } = props
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
                <Stack direction="row" spacing={1} alignItems="center">
                    <Typography>Close</Typography>
                    <Switch
                        checked={checked}
                        onChange={handleChange}
                        disabled={checked ? !hasPermission(Actions.CloseStore, permissions) : !hasPermission(Actions.OpenStore, permissions) }
                    />
                    <Typography>Open</Typography>
                </Stack>
            </CardContent>
            <CardActions>
                <StoreAdjust store={store} permissions={permissions} orders={orders } addProduct={addProduct}
                    removeProduct={removeProduct} updateProduct={updateProduct} reviewProduct={reviewProduct}
                    addDiscount={addDiscount} addProductDiscount={addProductDiscount} addCategoryDiscount={addCategoryDiscount}
                    addProductPurchasePolicy={addProductPurchasePolicy} addCategoryPurchasePolicy={addCategoryPurchasePolicy}
                    addBagPurchasePolicy={addBagPurchasePolicy} addUserPurchasePolicy={addUserPurchasePolicy}
                    nominateStoreOwner={nominateStoreOwner} nominateStoreManager={nominateStoreManager}
                    removeStoreOwnerNomination={removeStoreOwnerNomination} addActionToManager={addActionToManager} getStorePurchaseHistory={getStorePurchaseHistory }
                    />
            </CardActions>
        </Card>
    )
}
