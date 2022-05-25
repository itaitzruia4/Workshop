import * as React from 'react';
import Button from '@mui/material/Button';
import TextField from '@mui/material/TextField';
import Dialog from '@mui/material/Dialog';
import DialogActions from '@mui/material/DialogActions';
import DialogContent from '@mui/material/DialogContent';
import DialogContentText from '@mui/material/DialogContentText';
import DialogTitle from '@mui/material/DialogTitle';

export default function AddStoreDialog(
    stores: string[],
    setStores: React.Dispatch<React.SetStateAction<string[]>>) {

    const [open, setOpen] = React.useState(false);
    const [name, setName] = React.useState("");

    const handleClickOpen = () => {
        setOpen(true);
    };

    const handleClose = () => {
        setOpen(false);
    };

    const handleAdd = (e: React.FormEvent<HTMLFormElement>) => {
        e.preventDefault();
        setStores([name, ...stores]);
        setName("");
        handleClose();
    };

    return (
        <div>
            <Button onClick={handleClickOpen}>
                Add store
            </Button>
            <Dialog open={open} onClose={handleClose}>
                <DialogTitle>Subscribe</DialogTitle>
                <DialogContent>
                    <DialogContentText>
                        Please input your new store's name.
                    </DialogContentText>
                    <form onSubmit={handleAdd} id="storeNameForm" >
                        <TextField
                            value={name}
                            autoFocus
                            margin="dense"
                            id="name"
                            fullWidth
                            variant="standard"
                            onChange={(e) => setName(e.target.value)}
                            />
                    </form>
                </DialogContent>
                <DialogActions>
                    <Button onClick={handleClose}>Cancel</Button>
                    <Button variant="contained" type="submit" form="storeNameForm">Add</Button>
                </DialogActions>
            </Dialog>
        </div>
    );
}
