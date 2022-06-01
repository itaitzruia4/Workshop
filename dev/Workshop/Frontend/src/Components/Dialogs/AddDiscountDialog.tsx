import * as React from 'react';
import Button from '@mui/material/Button';
import TextField from '@mui/material/TextField';
import Dialog from '@mui/material/Dialog';
import DialogActions from '@mui/material/DialogActions';
import DialogContent from '@mui/material/DialogContent';
import DialogContentText from '@mui/material/DialogContentText';
import DialogTitle from '@mui/material/DialogTitle';

export default function AddDiscountDialog(
    storeId: number,
    addDiscount: (storeId: number, discountJson: string) => void) {

    const [open, setOpen] = React.useState(false);
    const [discount, setDiscount] = React.useState("");

    const handleClickOpen = () => {
        setOpen(true);
    };

    const handleClose = () => {
        setOpen(false);
    };

    const handleAdd = (e: React.FormEvent<HTMLFormElement>) => {
        e.preventDefault();
        addDiscount(storeId, discount);
        setDiscount("");
        handleClose();
    };

    return (
        <div>
            <Button onClick={handleClickOpen}>
                Add discount
            </Button>
            <Dialog open={open} onClose={handleClose}>
                <DialogTitle>New Store</DialogTitle>
                <DialogContent>
                    <DialogContentText>
                        Please insert discount details in JSON format
                    </DialogContentText>
                    <form onSubmit={handleAdd} id="storeNameForm" >
                        <TextField
                            value={discount}
                            autoFocus
                            margin="dense"
                            id="name"
                            fullWidth
                            variant="standard"
                            onChange={(e) => setDiscount(e.target.value)}
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
