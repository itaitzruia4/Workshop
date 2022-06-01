import * as React from 'react';

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
import { useState } from 'react';

import AddProductDialog from '../Components/Dialogs/addProductDialog';
import AddDiscountDialog from '../Components/Dialogs/AddDiscountDialog';

import { Store } from "../Types/store"
import { Product } from "../Types/product"


export default function StoresList(
    stores: Store[],
    addProduct: (storeId: number, productName: string, description: string, price: number, quantity: number, category: string) => void,
    closeStore: (storeId: number) => void,
    addDiscount: (storeId: number, discountJson: string) => void
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
                    <ListItemButton key={store.storeId} onClick={handleClick}>
                        <ListItemText primary={<Typography style={{ color: 'white' }}>{store.name}</Typography>} />
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
                        <ButtonGroup variant="outlined" aria-label="outlined button group">
                            {AddProductDialog(store.storeId, addProduct)}
                            {AddDiscountDialog(store.storeId, addDiscount)}
                            <div>
                                <Button onClick={e => closeStore(store.storeId)}>Close store</Button>
                            </div>
                        </ButtonGroup>
                        <List component='li' disablePadding key={store.storeId}>
                            {[].map((product: Product) => {
                                        return (
                                            <ListItem button key={product.id}>
                                                <ListItemIcon>
                                                    <ProductionQuantityLimitsIcon />
                                                </ListItemIcon>
                                                <ListItemText key={product.id} primary={<Typography style={{ color: 'white' }}>{product.name}</Typography>} />
                                            </ListItem>
                                        )
                                    })
                               }
                        </List>
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