import * as React from 'react';
import Button from '@mui/material/Button';
import TextField from '@mui/material/TextField';
import Dialog from '@mui/material/Dialog';
import DialogActions from '@mui/material/DialogActions';
import DialogContent from '@mui/material/DialogContent';
import DialogContentText from '@mui/material/DialogContentText';
import DialogTitle from '@mui/material/DialogTitle';
import { makeProductPriceActionSimple, makeSimpleDiscount, SimpleDiscount } from '../../Types/discount';

export default function AddDiscountDialog(
    storeId: number,
    addDiscount: (storeId: number, discountJson: string) => void,
    addProductDiscount: (storeId: number, productId: number, discountJson: string) => void) {

    const [open, setOpen] = React.useState(false);
    const [productOpen, setProductOpen] = React.useState(false);
    const [categoryOpen, setCategoryOpen] = React.useState(false);
    const [storeOpen, setStoreOpen] = React.useState(false);
    const [discount, setDiscount] = React.useState("");
    const [percentage, setPercentage] = React.useState(0);
    const [productId, setProductId] = React.useState(0);
    const [category, setCategory] = React.useState("");

    const handleOpenDiscount = () => {
        setOpen(true);
    };

    const handleCloseDiscount = () => {
        setOpen(false);
    };

    const handleOpenProductDiscount = () => {
        setOpen(false);
        setProductOpen(true);
    };

    const handleCloseProductDiscount = () => {
        setProductOpen(false);
        setOpen(true);
    };

    const handleAdd = (e: React.FormEvent<HTMLFormElement>) => {
        e.preventDefault();
        addDiscount(storeId, discount);
        setDiscount("");
        handleCloseDiscount();
    };

    const handleAddProductDiscount = (e: React.FormEvent<HTMLFormElement>) => {
        e.preventDefault();

        const productDiscount: SimpleDiscount = makeSimpleDiscount(makeProductPriceActionSimple(percentage, productId));

        addProductDiscount(storeId, productId, JSON.stringify(productDiscount));
        setProductId(0);
        setPercentage(0);
        handleOpenProductDiscount();
    };

    return (
        <div>
            <Button onClick={handleOpenDiscount}>
                Add discount
            </Button>
            <Dialog open={open} onClose={handleCloseDiscount}>
                <DialogTitle>Add Discount</DialogTitle>
                <DialogContent>
                    <Button onClick={handleOpenProductDiscount}>Add Product Discount</Button>
                    <DialogContentText>
                        For complex discounts, please insert discount JSON string below:
                    </DialogContentText>
                    <form onSubmit={handleAdd} id="discountForm" >
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
                    <Button onClick={handleCloseDiscount}>Cancel</Button>
                    <Button variant="contained" type="submit" form="discountForm">Add</Button>
                </DialogActions>
            </Dialog>

            <Dialog open={productOpen} onClose={handleOpenProductDiscount}>
                <DialogTitle>Add Product Discount</DialogTitle>
                <DialogContent>
                    <DialogContentText>
                        Please insert product discount details
                    </DialogContentText>
                    <form onSubmit={handleAddProductDiscount} id="productDiscountForm" >
                        <TextField
                            value={productId}
                            autoFocus
                            margin="dense"
                            id="productId"
                            label="Product ID"
                            type="number"
                            fullWidth
                            variant="standard"
                            onChange={(e) => setProductId(Number(e.target.value))}
                        />
                        <TextField
                            value={percentage}
                            autoFocus
                            margin="dense"
                            id="percentage"
                            label="Discount Percentage"
                            type="number"
                            fullWidth
                            variant="standard"
                            onChange={(e) => setPercentage(Number(e.target.value))}
                        />
                    </form>
                </DialogContent>
                <DialogActions>
                    <Button onClick={handleCloseProductDiscount}>Cancel</Button>
                    <Button variant="contained" type="submit" form="productDiscountForm">Add</Button>
                </DialogActions>
            </Dialog>
        </div>
    );
}
