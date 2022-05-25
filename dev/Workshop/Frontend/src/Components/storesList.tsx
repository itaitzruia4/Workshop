import * as React from 'react';
import ListSubheader from '@mui/material/ListSubheader';
import List from '@mui/material/List';
import ListItemButton from '@mui/material/ListItemButton';
import ListItemIcon from '@mui/material/ListItemIcon';
import ListItemText from '@mui/material/ListItemText';
import Collapse from '@mui/material/Collapse';
import InboxIcon from '@mui/icons-material/MoveToInbox';
import DraftsIcon from '@mui/icons-material/Drafts';
import SendIcon from '@mui/icons-material/Send';
import ExpandLess from '@mui/icons-material/ExpandLess';
import ExpandMore from '@mui/icons-material/ExpandMore';
import StarBorder from '@mui/icons-material/StarBorder';
import Typography from '@mui/material/Typography';
import ListItem from '@mui/material/ListItem';
import Divider from '@mui/material/Divider';
import StorefrontIcon from '@mui/icons-material/Storefront';
import ProductionQuantityLimitsIcon from '@mui/icons-material/ProductionQuantityLimits';
import { useState } from 'react';


export default function StoresList(name: string) {
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
                    key={store}
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
/*
    return (
<List sx={{ width: '100%', maxWidth: '100%', bgcolor: 'background.paper' }}
      component="nav"
      aria-labelledby="nested-list-subheader"
      subheader={
                <ListSubheader component="div" id="nested-list-subheader">
                    Stores
                </ListSubheader>
            }
        >
                {['drugs','candy','snacks'].map((store) => (
                    <ListItemButton>
                        <ListItemIcon>
                            <SendIcon />
                        </ListItemIcon>
                        <ListItemText primary={<Typography style={{ color: 'white' }}>{store}</Typography>} />
                    </ListItemButton>
                ))}

        </List>
    );
    */
    return (
        <div>
            <List component='nav' aria-labelledby='nested-list-subheader'>
                {["drugs", "snacks", "candy"].map(doc => {
                    return (
                        <CustomizedListItem store={doc} />
                    )
                })}
            </List>
        </div>
    );
}
