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

import ButtonGroup from '@mui/material/ButtonGroup';
import Rating from '@mui/material/Rating';
import DialogActions from '@mui/material/DialogActions';
import DialogContent from '@mui/material/DialogContent';
import DialogContentText from '@mui/material/DialogContentText';
import DialogTitle from '@mui/material/DialogTitle';
import TextField from '@mui/material/TextField';


export default function InputDialog<T extends string | number>(label: string, hook: T, setHook: React.Dispatch<React.SetStateAction<T>>, disable: boolean) {
    const [open, setOpen] = React.useState(false);
    const [input, setInput] = React.useState(hook);
    const handleClose = () => {
        setOpen(false);
    };

    const handleSave = () => {
        setHook(input);
        setOpen(false);
    }
    return (
        <div>
            <ListItem button onClick={() => setOpen(true)} disabled={disable}>
                <ListItemText
                    primary={"Product " + label}
                    secondary={hook} />
            </ListItem>
            <Dialog open={open} onClose={handleClose}>
                <DialogTitle>Subscribe</DialogTitle>
                <DialogContent>
                    <DialogContentText>
                        To subscribe to this website, please enter your email address here. We
                        will send updates occasionally.
                    </DialogContentText>
                    <TextField
                        autoFocus
                        margin="dense"
                        label={label}
                        value={input}
                        defaultValue={hook}
                        fullWidth
                        variant="standard"
                        onChange={(e) => setInput(e.target.value as T)}
                    />
                </DialogContent>
                <DialogActions>
                    <Button onClick={handleClose}>Cancel</Button>
                    <Button onClick={handleSave}>Save</Button>
                </DialogActions>
            </Dialog>
        </div>);
}