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
import ListItemButton from '@mui/material/ListItemButton';
import ListItemIcon from '@mui/material/ListItemIcon';
import ProductionQuantityLimitsIcon from '@mui/icons-material/ProductionQuantityLimits';
import { TransitionProps } from '@mui/material/transitions';
import Rating from '@mui/material/Rating';
import Table from '@mui/material/Table';
import TableBody from '@mui/material/TableBody';
import TableCell from '@mui/material/TableCell';
import TableContainer from '@mui/material/TableContainer';
import TableHead from '@mui/material/TableHead';
import TableRow from '@mui/material/TableRow';
import Paper from '@mui/material/Paper';
import KeyboardArrowDownIcon from '@mui/icons-material/KeyboardArrowDown';
import KeyboardArrowUpIcon from '@mui/icons-material/KeyboardArrowUp';
import Box from '@mui/material/Box';
import Collapse from '@mui/material/Collapse';
import ShoppingCart from '@mui/icons-material/ShoppingCart';
import Badge from '@mui/material/Badge';
import TextField from '@mui/material/TextField';
import Tabs from '@mui/material/Tabs';
import Tab from '@mui/material/Tab';

import ProductDialog from './ProductDialog';
import AddProductDialog from './addProductDialog';

import { memberToken, Actions, StorePermission } from '../../Types/roles';
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
        actions: Actions[],
        addProduct: (storeId: number, productName: string, description: string, price: number, quantity: number, category: string) => void,
        removeProduct: (storeId: number, productId: number) => void,
        updateProduct: (storeId: number, productId: number, productName: string, price: number, quantity: number, category: string) => void,
        reviewProduct: (productId: number, review: string, rating: number) => void,
    }) {
    const { store, actions, addProduct ,removeProduct, updateProduct, reviewProduct } = props
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
                        <ButtonGroup variant="outlined" aria-label="outlined button group">
                            <AddProductDialog storeId={store.storeId} addProduct = {addProduct }/>
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
                </Box>
            </Dialog>
        </div>
    );
}