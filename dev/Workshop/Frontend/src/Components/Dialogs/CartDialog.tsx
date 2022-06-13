import * as React from 'react';
import Button from '@mui/material/Button';
import Dialog from '@mui/material/Dialog';
import ListItemText from '@mui/material/ListItemText';
import ListItem from '@mui/material/ListItem';
import List from '@mui/material/List';
import Divider from '@mui/material/Divider';
import AppBar from '@mui/material/AppBar';
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


import { Product } from '../../Types/product';
import { Store } from '../../Types/store';
import { Cart, Bag, getBagCost, getBagToStore } from '../../Types/shopping';

import InputDialog from './InputDialog'
import BuyCartDialog from './BuyCartDialog'

const Transition = React.forwardRef(function Transition(
    props: TransitionProps & {
        children: React.ReactElement;
    },
    ref: React.Ref<unknown>,
) {
    return <Slide direction="up" ref={ref} {...props} />;
});

function createData(
    name: string,
    calories: number,
    fat: number,
    carbs: number,
    protein: number,
    price: number,
) {
    return {
        name,
        calories,
        fat,
        carbs,
        protein,
        price,
        history: [
            {
                date: '2020-01-05',
                customerId: '11091700',
                amount: 3,
            },
            {
                date: '2020-01-02',
                customerId: 'Anonymous',
                amount: 1,
            },
        ],
    };
}

function Row(props: { store: Store, bag: Bag, removeProduct: (productId: number) => void, editProductAmount: (productId: number, amount: number) => void }) {
    const { store, bag, removeProduct, editProductAmount } = props;
    const [open, setOpen] = React.useState(false);

    return (
        <React.Fragment>
            <TableRow sx={{ '& > *': { borderBottom: 'unset' } }}>
                <TableCell>
                    <IconButton
                        aria-label="expand row"
                        size="large"
                        onClick={() => setOpen(!open)}
                    >
                        {open ? <KeyboardArrowUpIcon /> : <KeyboardArrowDownIcon />}
                    </IconButton>
                </TableCell>
                <TableCell component="th" scope="row">
                    {store.name}
                </TableCell>
                <TableCell >{bag.products.length}</TableCell>
                <TableCell >{getBagCost(bag)}</TableCell>
            </TableRow>
            <TableRow>
                <TableCell style={{ paddingBottom: 0, paddingTop: 0 }} colSpan={6}>
                    <Collapse in={open} timeout="auto" unmountOnExit>
                        <Paper elevation={8} variant="outlined" sx={{ margin: 1, border: '3px dashed' ,borderColor: '#cfd8dc'}}>
                            <Box >
                            <Typography variant="h6" gutterBottom component="div">
                                    {'Products from ' + store.name}
                            </Typography>
                            <Table size="medium" aria-label="purchases">
                                <TableHead>
                                    <TableRow>
                                        <TableCell >Name</TableCell>
                                        <TableCell >Amount</TableCell>
                                        <TableCell >Total price ($)</TableCell>
                                    </TableRow>
                                </TableHead>
                                <TableBody>
                                    {bag.products.map((product) => (
                                        <TableRow key={product.id}>
                                            <TableCell component="th" scope="row">
                                                {product.name}
                                            </TableCell>
                                            <TableCell >
                                                <TextField
                                                value={product.quantity}
                                                autoFocus
                                                margin="dense"
                                                id="quantity"
                                                label="quantity"
                                                type="number"
                                                fullWidth
                                                variant="standard"
                                                    onChange={(e) => editProductAmount(product.id,Number(e.target.value))}
                                                />
                                            </TableCell>
                                            <TableCell >
                                                {product.quantity * product.basePrice}
                                            </TableCell>
                                            <TableCell >
                                                <Button variant="contained" color="error" onClick={() => { removeProduct(product.id) }}>Remove</Button>
                                            </TableCell>

                                        </TableRow>
                                    ))}
                                </TableBody>
                            </Table>
                            </Box>
                            </Paper>
                    </Collapse>
                </TableCell>
            </TableRow>
        </React.Fragment>
    );
}

export default function CartDialog(
    editCart: (productId: number, quantity: number) => void,
    buyCart: (number: string, year: string, month: string, ccv: string, holder: string, id: string, name: string, address: string,
        city: string, country: string, zip: string) => void,
    cart: Cart,
    stores: Store[]
) {
    const [open, setOpen] = React.useState(false);
    const handleClose = () => {
        setOpen(false);
    };
    const handleRemoveProduct = (productId: number) => {
        editCart(productId, 0);
        setOpen(true);
    }
    const handleEditProductAmount = (productId: number, amount: number) => {
        editCart(productId, amount);
        setOpen(true);
    }
    return (
        <div>
            <IconButton size="large" color="inherit" onClick={e => setOpen(true)}>
                <Badge badgeContent={cart.shoppingBags.reduce((sum, bag) => sum + bag.products.length, 0)} color="error">
                <ShoppingCart />
            </Badge>
            </IconButton>
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
                            Your shopping cart
                        </Typography>
                        {BuyCartDialog(buyCart)}
                    </Toolbar>
                </AppBar>
        <TableContainer component={Paper}>
            <Table aria-label="collapsible table">
                <TableHead>
                    <TableRow>
                        <TableCell />
                        <TableCell>Store</TableCell>
                        <TableCell >Total products</TableCell>
                                <TableCell >Total price($)</TableCell>
                    </TableRow>
                </TableHead>
                <TableBody>
                            {getBagToStore(cart, stores).map(([bag, store]: [Bag, Store]) => (
                                <Row key={store.storeId} store={store} bag={bag} removeProduct={handleRemoveProduct} editProductAmount={handleEditProductAmount} />
                    ))}
                </TableBody>
            </Table>
                </TableContainer>
            </Dialog>
            </div>
    );
}