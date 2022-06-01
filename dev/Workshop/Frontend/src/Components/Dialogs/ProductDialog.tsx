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
import ButtonGroup from '@mui/material/ButtonGroup';
import Rating from '@mui/material/Rating';

import { Product } from '../../Types/product';

const Transition = React.forwardRef(function Transition(
    props: TransitionProps & {
        children: React.ReactElement;
    },
    ref: React.Ref<unknown>,
) {
    return <Slide direction="up" ref={ref} {...props} />;
});

export default function ProductDialog(product : Product) {
    const [open, setOpen] = React.useState(false);
    const [rating, setRating] = React.useState(0);

    const handleClickOpen = () => {
        setOpen(true);
    };

    const handleClose = () => {
        setOpen(false);
    };

    return (
        <div>
            <ListItemButton key={product.id} onClick={handleClickOpen}>
                <ListItemIcon>
                    <ProductionQuantityLimitsIcon />
                </ListItemIcon>
                <ListItemText key={product.id} primary={<Typography style={{ color: 'black' }}>{product.name}</Typography>} />
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
                        <Button autoFocus color="inherit" onClick={handleClose}>
                            save
                        </Button>
                        <Button autoFocus color="inherit" onClick={handleClose}>
                            cancel
                        </Button>
                    </Toolbar>
                </AppBar>
                <List>
                    <ListItem button>
                        <ListItemText
                            primary="Product name"
                            secondary={product.name} />
                    </ListItem>
                    <Divider />
                    <ListItem button>
                        <ListItemText
                            primary="Product description"
                            secondary={product.description}
                        />
                    </ListItem>
                    <Divider />
                    <ListItem button>
                        <ListItemText
                            primary="Product category"
                            secondary={product.category}
                        />
                    </ListItem>
                    <Divider />
                    <ListItem button>
                        <ListItemText
                            primary="Product price"
                            secondary={product.basePrice}
                        />
                    </ListItem>

                    <Typography component="legend">Rate this product</Typography>
                    <Rating
                        size="large"
                        precision={0.5}
                        name="simple-controlled"
                        value={rating}
                        onChange={(event, newValue) => {
                            newValue ? setRating(newValue) : setRating(0);
                        }}
                    />
                </List>
            </Dialog>
        </div>
    );
}
