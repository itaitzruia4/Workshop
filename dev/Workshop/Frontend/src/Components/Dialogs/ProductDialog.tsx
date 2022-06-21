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
import Box from '@mui/material/Box';
import TextField from '@mui/material/TextField';
import Badge, { BadgeProps } from '@mui/material/Badge';
import { styled } from '@mui/material/styles';

import { Product } from '../../Types/product';
import { Store } from '../../Types/store';

import InputDialog from './InputDialog' 

const Transition = React.forwardRef(function Transition(
    props: TransitionProps & {
        children: React.ReactElement;
    },
    ref: React.Ref<unknown>,
) {
    return <Slide direction="up" ref={ref} {...props} />;
});

const StyledBadge = styled(Badge)<BadgeProps>(({ theme }) => ({
    '& .MuiBadge-badge': {
        right: -5,
        top: -5,
        border: `2px solid ${theme.palette.background.paper}`,
        padding: '0 4px',
    },
}));

export default function ProductDialog(
    props: {
        store: Store,
        product: Product,
        removeProduct: null | ((storeId: number, productId: number) => void),
        updateProduct: null | ((storeId: number, productId: number, productName: string, price: number, quantity: number, category: string) => void),
        reviewProduct: (productId: number, review: string, rating: number) => void
    }) {
    const {store, product, removeProduct, updateProduct, reviewProduct} = props
    const [open, setOpen] = React.useState(false);
    const [rating, setRating] = React.useState(0);
    const [review, setReview] = React.useState("");
    const [rated, setRated] = React.useState(false);

    const [name, setName] = React.useState(product.name);
    const [price, setPrice] = React.useState(product.basePrice);
    const [quantity, setQuantity] = React.useState(product.quantity);
    const [category, setCategory] = React.useState(product.category);


    const handleClickOpen = () => {
        setName(product.name)
        setPrice(product.basePrice)
        setQuantity(product.quantity)
        setCategory(product.category)
        setOpen(true);

    };

    const handleClose = () => {
        setOpen(false);
    };

    const handleSave = () => {
        if (updateProduct != null) {
            updateProduct(store.storeId, product.id, name, price, quantity, category);
        }
        if (rated) { reviewProduct(product.id, review, rating); }
        setOpen(false);
    }

    const handleRemove = () => {
        if (removeProduct != null) {
            removeProduct(store.storeId, product.id);
        }
        handleClose();
    }

    const disableInput = updateProduct === null

    return (
        <div>
            <ListItemButton key={product.id} onClick={handleClickOpen}>              
                    <ListItemIcon>
                        <StyledBadge badgeContent={product.id} color="primary">
                            <ProductionQuantityLimitsIcon />
                        </StyledBadge >
                    </ListItemIcon>
                <ListItemText key={product.id} primary={
                    <Typography style={{ color: 'black' }}>{product.name}</Typography>} />
            </ListItemButton>
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
                            {product.name}
                        </Typography>
                        <Button autoFocus color="inherit" onClick={handleSave}>
                            Save
                        </Button>
                        <Button autoFocus color="inherit" onClick={handleClose}>
                            Cancel
                        </Button>
                        <Button variant="contained" color="error" onClick={handleRemove} disabled={removeProduct === null}>
                            Delete product
                        </Button>
                    </Toolbar>
                </AppBar>
                <List>
                    {InputDialog("name", name, setName, disableInput)}
                    <Divider />
                    <ListItem button>
                        <ListItemText
                            primary="Product description"
                            secondary={product.description}
                        />
                    </ListItem>
                    <Divider />
                    {InputDialog("category", category, setCategory, disableInput)}
                    <Divider />
                    {InputDialog("price", price, setPrice, disableInput)}
                    <Divider />
                    {InputDialog("quantity", quantity, setQuantity, disableInput)}
                    <Typography component="legend">Rate this product</Typography>
                    <Rating
                        size="large"
                        precision={0.5}
                        name="simple-controlled"
                        value={rating}
                        onChange={(event, newValue) => {
                            setRated(true);
                            newValue ? setRating(newValue) : setRating(0);
                        }}
                    />
                    <Box
                        sx={{
                            maxWidth: '100%',
                        }}
                    >
                        <TextField
                            fullWidth
                            label="Review"
                            value={review}
                            onChange={(e) => {
                                setRated(true);
                                setReview(e.target.value);
                            }}
                        />
                    </Box>
                </List>
            </Dialog>
        </div>
    );
}
