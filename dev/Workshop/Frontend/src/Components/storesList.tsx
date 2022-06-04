import * as React from 'react';
import { useState } from 'react';

import Button from '@mui/material/Button';
import ButtonGroup from '@mui/material/ButtonGroup';
import List from '@mui/material/List';
import ListItemIcon from '@mui/material/ListItemIcon';
import ListItemText from '@mui/material/ListItemText';
import Collapse from '@mui/material/Collapse';
import ExpandLess from '@mui/icons-material/ExpandLess';
import ExpandMore from '@mui/icons-material/ExpandMore';
import Typography from '@mui/material/Typography';
import ListItem from '@mui/material/ListItem';
import Divider from '@mui/material/Divider';
import StorefrontIcon from '@mui/icons-material/Storefront';
import ProductionQuantityLimitsIcon from '@mui/icons-material/ProductionQuantityLimits';
import ListItemButton from '@mui/material/ListItemButton';
import Popper from '@mui/material/Popper';
import Box from '@mui/material/Box';
import Paper from '@mui/material/Paper';


import AddProductDialog from '../Components/Dialogs/addProductDialog';
import AddDiscountDialog from '../Components/Dialogs/AddDiscountDialog';
import ProductDialog from '../Components/Dialogs/ProductDialog';
import AddToCartDialog from '../Components/Dialogs/addToCartDialog';

import { Store } from "../Types/store"
import { Product } from "../Types/product"


export default function StoresList(
    stores: Store[],
    addProduct: (storeId: number, productName: string, description: string, price: number, quantity: number, category: string) => void,
    removeProduct: (storeId: number, productId: number) => void,
    updateProduct: (storeId: number, productId: number, productName: string, price: number, quantity: number, category: string) => void,
    reviewProduct: (productId: number, review: string, rating: number) => void,
    closeStore: (storeId: number) => void,
    openStore: (storeId: number) => void,
    addDiscount: (storeId: number, discountJson: string) => void,
    addProductDiscount: (storeId: number, productId: number, discountJson: string) => void,
    addToCart: (storeId: number, productId: number, quantity: number ) => void
    )
    {
        const CustomizedListItem: React.FC<{
            store: Store
        }> = ({ store }) => {
            const [open, setOpen] = useState(false)
            const handleClick = () => {
                setOpen(!open)
            }

            return (
                <div>
                    <Divider />
                    <ListItemButton key={store.storeId} onClick={handleClick}>
                        <ListItemText primary={<Typography variant="h5" style={{ color: 'black' }}>{store.name}</Typography>} />
                        {open ? <ExpandLess /> : <ExpandMore />}
                        <ListItemIcon>
                            <StorefrontIcon />
                        </ListItemIcon>
                    </ListItemButton>
                    <Collapse
                        in={open}
                        timeout='auto'
                        unmountOnExit
                    >
                        <Paper elevation={12}>
                        <ButtonGroup variant="outlined" aria-label="outlined button group">
                            {AddProductDialog(store.storeId, addProduct)}
                            {AddDiscountDialog(store.storeId, addDiscount, addProductDiscount)}
                            <div>
                                <Button onClick={e => closeStore(store.storeId)}>Close store</Button>
                            </div>
                            <div>
                                <Button onClick={e => openStore(store.storeId)}>Open store</Button>
                            </div>
                        </ButtonGroup>
                        <List component='li' disablePadding key={store.storeId}>
                            {store.products.map( product=> {
                                return (
                                    <ListItem key={product.id} secondaryAction={AddToCartDialog(store, product, addToCart)} >
                                        {ProductDialog(store, product, removeProduct, updateProduct, reviewProduct)}
                                        </ListItem>
                                        )
                                    })
                               }
                            </List>
                            </Paper>
                    </Collapse>
                    <Divider />
                </div>
            )
        }
        return (
            <div>
                <List component='nav' aria-labelledby='nested-list-subheader'>
                    {stores.map(storeItem => {
                        return (storeItem.open ?

                            <CustomizedListItem store={storeItem} />
                         : null)
                    })}
                </List>
            </div>
        );
}