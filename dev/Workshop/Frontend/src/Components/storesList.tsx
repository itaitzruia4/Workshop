import * as React from 'react';
import ListSubheader from '@mui/material/ListSubheader';
import List from '@mui/material/List';
import ListItemButton from '@mui/material/ListItemButton';
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
import { useState } from 'react';


export default function StoresList(stores: string[]) {
    const [open, setOpen] = React.useState(true);

    const handleClick = () => {
        setOpen(!open);
    };

    const CustomizedListItem: React.FC<{
        store: string
    }> = ({ store }) => {
        const [open, setOpen] = useState(false)
        const handleClick = () => {
            setOpen(!open)
        }

        return (
            <div>
                <ListItem button key={store} onClick={handleClick}>
                    <ListItemText primary={<Typography style={{ color: 'white' }}>{store}</Typography>} />
                    {open ? <ExpandLess /> : <ExpandMore />}
                    <ListItemIcon>
                        <StorefrontIcon />
                    </ListItemIcon>
                </ListItem>
                <Collapse
                    in={open}
                    timeout='auto'
                    unmountOnExit
                >
                    <List component='li' disablePadding key={store}>
                        {store.split('').map(sheet => {
                            return (
                                <ListItem button key={sheet}>
                                    <ListItemIcon>
                                        <ProductionQuantityLimitsIcon />
                                    </ListItemIcon>
                                    <ListItemText key={sheet} primary={<Typography style={{ color: 'white' }}>{sheet}</Typography>} />
                                </ListItem>
                            )
                        })}
                    </List>
                </Collapse>
                <Divider />
            </div>
        )
    }
    return (
        <div>
            <List component='nav' aria-labelledby='nested-list-subheader'>
                {stores.map(doc => {
                    return (
                        <CustomizedListItem store={doc} />
                    )
                })}
            </List>
        </div>
    );
}
