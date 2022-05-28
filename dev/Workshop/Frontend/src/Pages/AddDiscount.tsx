import * as React from 'react';
import Button from '@mui/material/Button';
import TextField from '@mui/material/TextField';
import Dialog from '@mui/material/Dialog';
import DialogActions from '@mui/material/DialogActions';
import DialogContent from '@mui/material/DialogContent';
import DialogContentText from '@mui/material/DialogContentText';
import DialogTitle from '@mui/material/DialogTitle';

export default function AddDiscount(
    priceActions: string[],
    setPriceActions: React.Dispatch<React.SetStateAction<string[]>>) {

    const [percentage, setPercentage] = React.useState("");
    const [identifier, setIdentifier] = React.useState("");
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
        setPriceActions([name, ...priceActions]);
        setName("");
    };

    return (
        <div>
            <Button onClick={handleClickOpen}>
                OK
            </Button>
            <Dialog open={open} onClose={handleClose}>
                <DialogTitle>New Store</DialogTitle>
                <DialogContent>
                    <DialogContentText>
                        Please insert discount details:
                    </DialogContentText>
                    <form onSubmit={handleAdd} id="priceActionForm" >
                        <TextField
                            value={name}
                            autoFocus
                            margin="dense"
                            id="name"
                            fullWidth
                            variant="standard"
                            onChange={(e) => setName(e.target.value)}
                        />
                        <TextField
                            value={percentage}
                            autoFocus
                            margin="dense"
                            id="percentage"
                            fullWidth
                            variant="standard"
                            onChange={(e) => setPercentage(e.target.value)}
                        />
                        <TextField
                            value={identifier}
                            autoFocus
                            margin="dense"
                            id="identifier"
                            fullWidth
                            variant="standard"
                            onChange={(e) => setIdentifier(e.target.value)}
                        />
                    </form>
                </DialogContent>
                <DialogActions>
                    <Button onClick={handleClose}>Cancel</Button>
                    <Button variant="contained" type="submit" form="priceActionForm">Add</Button>
                </DialogActions>
            </Dialog>
        </div>
    );
}
