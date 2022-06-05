import * as React from 'react';
import Box from '@mui/material/Box';
import Chip from '@mui/material/Chip';
import Button from '@mui/material/Button';
import IconButton from '@mui/material/IconButton';
import Grid from '@mui/material/Grid';
import Stack from '@mui/material/Stack';
import Divider from '@mui/material/Divider';
import Typography from '@mui/material/Typography';
import TextField from '@mui/material/TextField';
import Dialog from '@mui/material/Dialog';
import DialogActions from '@mui/material/DialogActions';
import DialogContent from '@mui/material/DialogContent';
import DialogContentText from '@mui/material/DialogContentText';
import DialogTitle from '@mui/material/DialogTitle';
import AddShoppingCartIcon from '@mui/icons-material/AddShoppingCart';
import Fab from '@mui/material/Fab';
import Badge, { BadgeProps } from '@mui/material/Badge';
import { styled } from '@mui/material/styles';

import { Product } from '../../Types/product';
import { Store } from '../../Types/store';


const StyledBadge = styled(Badge)<BadgeProps>(({ theme }) => ({
    '& .MuiBadge-badge': {
        right: -5,
        top: -5,
        border: `2px solid ${theme.palette.background.paper}`,
        padding: '0 4px',
    },
}));

export default function AddToCartDialog(
    store: Store,
    product: Product,
    addToCart: (storeId: number, productId: number, quantity: number) => void
) {
    const [open, setOpen] = React.useState(false);
    const [quantity, setQuantity] = React.useState(0);
    const handleClickOpen = () => {
        setOpen(true);
    };

    const handleClose = () => {

        setOpen(false);
    };

    const handleAdd = () => {
        addToCart(store.storeId, product.id, quantity);
        setQuantity(0);
        handleClose();
    }
    return (
        <div>
        <IconButton onClick={handleClickOpen} edge="end">
                <Fab size="small">
                    <StyledBadge badgeContent={product.quantity} color="primary">
                        <AddShoppingCartIcon />
                    </StyledBadge >
            </Fab>
        </IconButton>
        <Dialog open={open} onClose={handleClose}>
            <Box sx={{ my: 3, mx: 2 }}>
                <Grid container alignItems="center">
                    <Grid item xs>
                            <Typography gutterBottom variant="h4" component="div">
                                {product.name}
                        </Typography>
                    </Grid>
                    <Grid item>
                            <Typography gutterBottom variant="h6" component="div">
                                {product.basePrice + '$'}
                        </Typography>
                    </Grid>
                </Grid>
                    <Typography color="text.secondary" variant="body2">
                        {product.description}
                </Typography>
            </Box>
            <Divider variant="middle" />
            <Box sx={{ m: 2 }}>
                    <Typography gutterBottom variant="body1">
                        {'Select quantity (' + product.quantity + ' are available)'}
                </Typography>
                    <TextField
                        value={quantity}
                        autoFocus
                        margin="dense"
                        id="quantity"
                        type="number"
                        fullWidth
                        variant="standard"
                        onChange={(e) => setQuantity(Number(e.target.value))}
                    />
            </Box>
            <Box sx={{ mt: 3, ml: 1, mb: 1 }}>
                    <Button onClick={e => handleAdd()} >Add to cart</Button>
            </Box>
            </Dialog>
         </div>
    );
}
