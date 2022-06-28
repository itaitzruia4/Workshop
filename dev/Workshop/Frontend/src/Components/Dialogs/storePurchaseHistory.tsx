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
import { Store, Order, getAddressString } from '../../Types/store';
import { Cart, Bag, getBagCost, getBagToStore } from '../../Types/shopping';

const Transition = React.forwardRef(function Transition(
    props: TransitionProps & {
        children: React.ReactElement;
    },
    ref: React.Ref<unknown>,
) {
    return <Slide direction="up" ref={ref} {...props} />;
});


function Row(props: { store: Store, order: Order }) {
    const { store, order } = props;
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
                <TableCell >{order.buyerName}</TableCell>
                <TableCell >{getAddressString(order.address)}</TableCell>
                <TableCell >{order.date}</TableCell>
                <TableCell >{order.price}</TableCell>
            </TableRow>
            <TableRow>
                <TableCell style={{ paddingBottom: 0, paddingTop: 0 }} colSpan={6}>
                    <Collapse in={open} timeout="auto" unmountOnExit>
                        <Paper elevation={8} variant="outlined" sx={{ margin: 1, border: '3px dashed', borderColor: '#cfd8dc' }}>
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
                                        {order.products.map((product) => (
                                            <TableRow key={product.id}>
                                                <TableCell component="th" scope="row">
                                                    {product.name}
                                                </TableCell>
                                                <TableCell >
                                                    {product.quantity}
                                                </TableCell>       
                                                <TableCell >
                                                    {product.quantity * product.basePrice}
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

export default function storePurchaseHistory(props: { store: Store, orders: Order[] , hasPermission: boolean }) {
    const { store, orders, hasPermission } = props;
    return (   
        hasPermission ? 
        <TableContainer component={Paper}>
            <Table aria-label="collapsible table">
                <TableHead>
                    <TableRow>
                        <TableCell />
                        <TableCell>Buyers name</TableCell>
                        <TableCell>Address</TableCell>
                        <TableCell>Date</TableCell>
                        <TableCell>Total price($)</TableCell>
                    </TableRow>
                </TableHead>
                <TableBody>
                    {orders.map(order => (
                        <Row key={order.id} store={store} order={order} />
                    ))}
                </TableBody>
            </Table>
            </TableContainer> : <Typography sx={{ fontSize: 14 }} color="text.secondary" gutterBottom>
                You dont have permission to view this store's purchase history
            </Typography>
    );
}