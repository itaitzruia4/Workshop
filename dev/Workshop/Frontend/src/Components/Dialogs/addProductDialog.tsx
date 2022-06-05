import * as React from 'react';
import Button from '@mui/material/Button';
import TextField from '@mui/material/TextField';
import Dialog from '@mui/material/Dialog';
import DialogActions from '@mui/material/DialogActions';
import DialogContent from '@mui/material/DialogContent';
import DialogContentText from '@mui/material/DialogContentText';
import DialogTitle from '@mui/material/DialogTitle';

export default function AddProductDialog(
    storeId : number,
    addProduct: (storeId: number, productName: string, description: string, price: number, quantity: number, category: string) => void ) {

    const [open, setOpen] = React.useState(false);
    const [name, setName] = React.useState("");
    const [description, setDescription] = React.useState("");
    const [price, setPrice] = React.useState(0);
    const [quantity, setQuantity] = React.useState(0);
    const [category, setCategory] = React.useState("");

    const handleClickOpen = () => {
        setOpen(true);
    };

    const handleClose = () => {
        setOpen(false);
    };

    const handleAdd = (e: React.FormEvent<HTMLFormElement>) => {
        e.preventDefault();
        addProduct(storeId, name, description, price, quantity, category);
        setName("");
        handleClose();
    };

    return (
        <div>
            <Button onClick={handleClickOpen}>
                Add Product
            </Button>
            <Dialog open={open} onClose={handleClose}>
                <DialogTitle>Add Product</DialogTitle>
                <DialogContent>
                    <DialogContentText>
                        Please input the products's configs.
                    </DialogContentText>
                    <form onSubmit={handleAdd} id="newProductForm" >
                        <TextField
                            value={name}
                            autoFocus
                            margin="dense"
                            id="name"
                            label='name'
                            fullWidth
                            variant="standard"
                            onChange={(e) => setName(e.target.value)}
                        />
                        <TextField
                            value={description}
                            autoFocus
                            margin="dense"
                            id="description"
                            label="description"
                            fullWidth
                            variant="standard"
                            onChange={(e) => setDescription(e.target.value)}
                        />
                        <TextField
                            value={price}
                            autoFocus
                            margin="dense"
                            id="price"
                            label="price"
                            type="number"
                            fullWidth
                            variant="standard"
                            onChange={(e) => setPrice(Number(e.target.value))}
                        />
                        <TextField
                            value={quantity}
                            autoFocus
                            margin="dense"
                            id="quantity"
                            label="quantity"
                            type="number"
                            fullWidth
                            variant="standard"
                            onChange={(e) => setQuantity(Number(e.target.value))}
                        />
                        <TextField
                            value={category}
                            autoFocus
                            margin="dense"
                            id="category"
                            label="category"
                            fullWidth
                            variant="standard"
                            onChange={(e) => setCategory(e.target.value)}
                        />
                    </form>
                </DialogContent>
                <DialogActions>
                    <Button onClick={handleClose}>Cancel</Button>
                    <Button variant="contained" type="submit" form="newProductForm">Add</Button>
                </DialogActions>
            </Dialog>
        </div>
    );
}
