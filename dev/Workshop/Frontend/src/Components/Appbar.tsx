import * as React from 'react';
import { styled, alpha } from '@mui/material/styles';
import AppBar from '@mui/material/AppBar';
import Box from '@mui/material/Box';
import Toolbar from '@mui/material/Toolbar';
import IconButton from '@mui/material/IconButton';
import Typography from '@mui/material/Typography';
import InputBase from '@mui/material/InputBase';
import Badge from '@mui/material/Badge';
import MenuItem from '@mui/material/MenuItem';
import Menu from '@mui/material/Menu';
import MenuIcon from '@mui/icons-material/Menu';
import SearchIcon from '@mui/icons-material/Search';
import AccountCircle from '@mui/icons-material/AccountCircle';
import ShoppingCart from '@mui/icons-material/ShoppingCart';
import NotificationsIcon from '@mui/icons-material/Notifications';
import AccountCircleIcon from '@mui/icons-material/AccountCircle';
import MoreIcon from '@mui/icons-material/MoreVert';

import CartDialog from './Dialogs/CartDialog'

import { Store } from "../Types/store"
import { Product } from "../Types/product"
import { Cart, Bag } from '../Types/shopping';
import { isMemberToken, token, memberToken } from '../Types/roles';
import { MarketNotification } from '../Types/Notification';
import { NotificationsList } from './NotificationsList';
import AdminDialog from './Dialogs/AdminDialog';
import { isAdmin } from '../Actions/AdminActions';


const Search = styled('div')(({ theme }) => ({
    position: 'relative',
    borderRadius: theme.shape.borderRadius,
    backgroundColor: alpha(theme.palette.common.white, 0.15),
    '&:hover': {
        backgroundColor: alpha(theme.palette.common.white, 0.25),
    },
    marginRight: theme.spacing(2),
    marginLeft: 0,
    width: '100%',
    [theme.breakpoints.up('sm')]: {
        marginLeft: theme.spacing(3),
        width: 'auto',
    },
}));

const SearchIconWrapper = styled('div')(({ theme }) => ({
    padding: theme.spacing(0, 2),
    height: '100%',
    position: 'absolute',
    pointerEvents: 'none',
    display: 'flex',
    alignItems: 'center',
    justifyContent: 'center',
}));

const StyledInputBase = styled(InputBase)(({ theme }) => ({
    color: 'inherit',
    '& .MuiInputBase-input': {
        padding: theme.spacing(1, 1, 1, 0),
        // vertical padding + font size from searchIcon
        paddingLeft: `calc(1em + ${theme.spacing(4)})`,
        transition: theme.transitions.create('width'),
        width: '100%',
        [theme.breakpoints.up('md')]: {
            width: '20ch',
        },
    },
}));

export default function Appbar(
    routeChange: (path: string, token: token) => () => void,
    token: token,
    name: string,
    stores: Store[],
    cart: Cart,
    nots: MarketNotification[],
    editCart: (productId: number, quantity: number) => void,
    buyCart: (number: string, year: string, month: string, ccv: string, holder: string, id: string, name: string, address: string,
                city: string, country: string, zip: string) => void) {
    const [anchorEl, setAnchorEl] = React.useState<null | HTMLElement>(null);
    const [mobileMoreAnchorEl, setMobileMoreAnchorEl] =
        React.useState<null | HTMLElement>(null);

    const [notificationsAnchorElem, setNotificationsAnchorElem] =
        React.useState<null | HTMLElement>(null);

    const [adminOpen, setAdminOpen] = React.useState<boolean>(false);

    const isMenuOpen = Boolean(anchorEl);
    const isMobileMenuOpen = Boolean(mobileMoreAnchorEl);

    const handleProfileMenuOpen = (event: React.MouseEvent<HTMLElement>) => {
        setAnchorEl(event.currentTarget);
    };

    const handleMobileMenuClose = () => {
        setMobileMoreAnchorEl(null);
    };

    const handleMobileMenuOpen = (event: React.MouseEvent<HTMLElement>) => {
        setMobileMoreAnchorEl(event.currentTarget);
    };

    const openNotifications = Boolean(notificationsAnchorElem);

    const handleCloseNotifications = () => {
        setNotificationsAnchorElem(null);
    }

    const handleOpenNotifications = (event: React.MouseEvent<HTMLElement>) => {
        if (nots.length > 0) {
            setNotificationsAnchorElem(event.currentTarget);
        }
    }


    const menuId = 'primary-search-account-menu';
    const mobileMenuId = 'primary-search-account-menu-mobile';
    return (
        <Box sx={{ flexGrow: 1 }}>
            {AdminDialog(adminOpen, token as memberToken) }
            <AppBar position="static">
                <Toolbar>
                    <IconButton
                        size="large"
                        edge="start"
                        color="inherit"
                        aria-label="open drawer"
                        sx={{ mr: 2 }}
                        onClick={(event: React.MouseEvent<HTMLElement>) => setAdminOpen(isAdmin(token as memberToken))}
                    >
                        <MenuIcon />
                    </IconButton>
                    <Typography
                        variant="h6"
                        noWrap
                        component="div"
                        sx={{ display: { xs: 'none', sm: 'block' } }}
                    >
                        { name }
                    </Typography>
                    <Search>
                        <SearchIconWrapper>
                            <SearchIcon />
                        </SearchIconWrapper>
                        <StyledInputBase
                            placeholder="Search…"
                            inputProps={{ 'aria-label': 'search' }}
                        />
                    </Search>
                    <Box sx={{ flexGrow: 1 }} />
                    <Box sx={{ display: { xs: 'none', md: 'flex' } }}>
                        {isMemberToken(token) ?
                            <IconButton
                                size="large"
                                aria-label="show notifications"
                                color="inherit"
                                onClick={routeChange('/profile', token as memberToken)}
                                aria-hashpopup="true"
                            >
                                <AccountCircleIcon />
                            </IconButton> : null
                        }

                        {CartDialog(editCart,buyCart, cart, stores)}

                        <IconButton
                            size="large"
                            aria-label="show notifications"
                            color="inherit"
                            onClick={handleOpenNotifications}
                            aria-hashpopup="true"
                        >
                            <Badge badgeContent={nots.length} color="error">
                                <NotificationsIcon />
                            </Badge>
                        </IconButton>

                        {NotificationsList(nots, openNotifications, handleCloseNotifications, notificationsAnchorElem) }
                    </Box>
                    <Box sx={{ display: { xs: 'flex', md: 'none' } }}>
                        <IconButton
                            size="large"
                            aria-label="show more"
                            aria-controls={mobileMenuId}
                            aria-haspopup="true"
                            onClick={handleMobileMenuOpen}
                            color="inherit"
                        >
                            <MoreIcon />
                        </IconButton>
                    </Box>
                </Toolbar>
            </AppBar>
        </Box>
    );
}
