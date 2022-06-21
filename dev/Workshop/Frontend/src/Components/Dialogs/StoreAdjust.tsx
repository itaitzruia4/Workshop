import * as React from 'react';
import Button from '@mui/material/Button';
import Dialog from '@mui/material/Dialog';
import ListItemText from '@mui/material/ListItemText';
import ListItem from '@mui/material/ListItem';
import List from '@mui/material/List';
import Divider from '@mui/material/Divider';
import AppBar from '@mui/material/AppBar';
import ButtonGroup from '@mui/material/ButtonGroup';
import Toolbar from '@mui/material/Toolbar';
import IconButton from '@mui/material/IconButton';
import Typography from '@mui/material/Typography';
import CloseIcon from '@mui/icons-material/Close';
import Slide from '@mui/material/Slide';
import { TransitionProps } from '@mui/material/transitions';
import Box from '@mui/material/Box';
import Tabs from '@mui/material/Tabs';
import Tab from '@mui/material/Tab';

import ProductDialog from './ProductDialog';
import AddProductDialog from './addProductDialog';
import AddDiscountDialog from './AddDiscountDialog';
import AddPurchaseDialog from './AddPurchaseDialog';
import StoreRolesDialog from './StoreRolesDialog';

import { memberToken, Actions, StorePermission, hasPermission } from '../../Types/roles';
import { Store } from "../../Types/store"
import { Product } from "../../Types/product"
import { Cart, Bag } from '../../Types/shopping';
import { MarketNotification } from '../../Types/Notification';


interface TabPanelProps {
    children?: React.ReactNode;
    index: number;
    value: number;
}

const Transition = React.forwardRef(function Transition(
    props: TransitionProps & {
        children: React.ReactElement;
    },
    ref: React.Ref<unknown>,
) {
    return <Slide direction="up" ref={ref} {...props} />;
});

function TabPanel(props: TabPanelProps) {
    const { children, value, index, ...other } = props;

    return (
        <div
            role="tabpanel"
            hidden={value !== index}
            id={`simple-tabpanel-${index}`}
            aria-labelledby={`simple-tab-${index}`}
            {...other}
        >
            {value === index && (
                <Box sx={{ p: 3 }}>
                    {children}
                </Box>
            )}
        </div>
    );
}

function a11yProps(index: number) {
    return {
        id: `simple-tab-${index}`,
        'aria-controls': `simple-tabpanel-${index}`,
    };
}



export default function StoreAdjust( 
    props: {
        store: Store,
        permissions: Actions[],
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
        addActionToManager: (nominee: string, storeId: number, action: string) => void
    }) {
    const { store, permissions, addProduct, removeProduct, updateProduct, reviewProduct,
        addDiscount, addProductDiscount, addCategoryDiscount, addProductPurchasePolicy, addCategoryPurchasePolicy,
        addBagPurchasePolicy, addUserPurchasePolicy, nominateStoreOwner, nominateStoreManager, removeStoreOwnerNomination, addActionToManager } = props
    const [open, setOpen] = React.useState(false);
    const handleClose = () => {
        setOpen(false);
    };
    const [value, setValue] = React.useState(0);

    const handleChange = (event: React.SyntheticEvent, newValue: number) => {
        setValue(newValue);
    };


    return (
        <div>
            <Button
                size="small"
                onClick={e => setOpen(true)}
            >View more options    
            </Button>
            <Dialog
                fullScreen
                open={open}
                onClose={handleClose}
                TransitionComponent={Transition}
            >
                <AppBar sx={{ position: 'relative' }}>
                    <Toolbar>
                        <IconButton
                            edge="start"
                            color="inherit"
                            onClick={handleClose}
                            aria-label="close"
                        >
                            <CloseIcon />
                        </IconButton>
                        <Typography sx={{ ml: 2, flex: 1 }} variant="h6" component="div">
                            {store.name}
                        </Typography>
                    </Toolbar>
                </AppBar>
                <Box sx={{ width: '100%' }}>
                    <Box sx={{ borderBottom: 1, borderColor: 'divider' }}>
                        <Tabs value={value} onChange={handleChange} aria-label="basic tabs example" centered>
                            <Tab label="Products" {...a11yProps(0)} />
                            <Tab label="Members" {...a11yProps(1)} />
                            <Tab label="Discounts" {...a11yProps(2)} />
                        </Tabs>
                    </Box>
                    <TabPanel value={value} index={0}>
                        <ButtonGroup variant="outlined" aria-label="outlined button group" disabled={!hasPermission(Actions.AddProduct, permissions)}>
                            <AddProductDialog storeId={store.storeId} addProduct={addProduct}/>
                        </ButtonGroup>
                        <List component='li' disablePadding key={store.storeId}>
                            {store.products.map(product => {
                                return (
                                    <ListItem key={product.id}>
                                        <ProductDialog store={store} product={product} removeProduct={removeProduct} updateProduct={updateProduct} reviewProduct={reviewProduct} />
                                    </ListItem>
                                )
                            })
                            }
                        </List>
                    </TabPanel>
                    <TabPanel value={value} index={1}>
                        <ButtonGroup variant="outlined" aria-label="outlined button group">
                            <StoreRolesDialog storeId={store.storeId} nominateStoreOwner={nominateStoreOwner}
                                nominateStoreManager={nominateStoreManager} removeStoreOwnerNomination={removeStoreOwnerNomination} addActionToManager={addActionToManager} />
                        </ButtonGroup>
                    </TabPanel>
                    <TabPanel value={value} index={2}>
                        <ButtonGroup variant="outlined" aria-label="outlined button group">
                            <AddDiscountDialog storeId={store.storeId} addDiscount={addDiscount} addProductDiscount={addProductDiscount} addCategoryDiscount={addCategoryDiscount } />
                            <AddPurchaseDialog storeId={store.storeId} addProductPurchase={addProductPurchasePolicy} addCategoryPurchase={addCategoryPurchasePolicy}
                                addBagPurchase={addBagPurchasePolicy} addUserPurchase={addUserPurchasePolicy } />
                        </ButtonGroup>
                    </TabPanel>
                </Box>
            </Dialog>
        </div>
    );
}