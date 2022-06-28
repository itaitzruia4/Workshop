import * as React from 'react';
import Button from '@mui/material/Button';
import TextField from '@mui/material/TextField';
import Dialog from '@mui/material/Dialog';
import DialogActions from '@mui/material/DialogActions';
import DialogContent from '@mui/material/DialogContent';
import DialogContentText from '@mui/material/DialogContentText';
import DialogTitle from '@mui/material/DialogTitle';
import { makeCategoryPriceActionSimple, makeProductPriceActionSimple, makeSimpleDiscount, makeStorePriceActionSimple, SimpleDiscount } from '../../Types/discount';
import { memberToken, Actions, StorePermission, actionNames, hasPermission } from '../../Types/roles';

export default function AddDiscountDialog(
    props: {
        storeId: number,
        permissions: Actions[],
        addDiscount: (storeId: number, discountJson: string) => void,
        addProductDiscount: (storeId: number, productId: number, discountJson: string) => void,
        addCategoryDiscount: (storeId: number, category: string, discountJson: string) => void
    }) {

    const { storeId, permissions ,addDiscount, addProductDiscount, addCategoryDiscount } = props;
    const [open, setOpen] = React.useState(false);
    const [productOpen, setProductOpen] = React.useState(false);
    const [categoryOpen, setCategoryOpen] = React.useState(false);
    const [storeDiscountOpen, setStoreDiscountOpen] = React.useState(false);

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

    const handleOpenCategoryDiscount = () => {
        setOpen(false);
        setCategoryOpen(true);
    };

    const handleCloseCategoryDiscount = () => {
        setCategoryOpen(false);
        setOpen(true);
    };

    const handleOpenStoreDiscount = () => {
        setOpen(false);
        setStoreDiscountOpen(true);
    };

    const handleCloseStoreDiscount = () => {
        setStoreDiscountOpen(false);
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
        handleCloseProductDiscount();
    };

    const handleAddCategoryDiscount = (e: React.FormEvent<HTMLFormElement>) => {
        e.preventDefault();

        const categoryDiscount: SimpleDiscount = makeSimpleDiscount(makeCategoryPriceActionSimple(percentage, category));

        addCategoryDiscount(storeId, category, JSON.stringify(categoryDiscount));
        setCategory("");
        setPercentage(0);
        handleCloseCategoryDiscount();
    };

    const handleAddStoreDiscount = (e: React.FormEvent<HTMLFormElement>) => {
        e.preventDefault();

        const storeDiscount: SimpleDiscount = makeSimpleDiscount(makeStorePriceActionSimple(percentage));

        addDiscount(storeId, JSON.stringify(storeDiscount));
        setPercentage(0);
        handleCloseCategoryDiscount();
    };

    return (
        <div>
            <Button disabled={!hasPermission(Actions.AddDiscount, permissions) } onClick={handleOpenDiscount}>
                Add discount
            </Button>
            <Dialog open={open} onClose={handleCloseDiscount}>
                <DialogTitle>Add Discount</DialogTitle>
                <DialogContent>
                    <Button onClick={handleOpenProductDiscount}>Add Product Discount</Button>
                    <Button onClick={handleOpenCategoryDiscount}>Add Category Discount</Button>
                    <Button onClick={handleOpenStoreDiscount}>Add Store Discount</Button>
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

            <Dialog open={productOpen} onClose={handleCloseProductDiscount}>
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
                    <Button onClick={handleCloseProductDiscount}>Back</Button>
                    <Button variant="contained" type="submit" form="productDiscountForm">Add</Button>
                </DialogActions>
            </Dialog>

            <Dialog open={categoryOpen} onClose={handleCloseCategoryDiscount}>
                <DialogTitle>Add Category Discount</DialogTitle>
                <DialogContent>
                    <DialogContentText>
                        Please insert category discount details
                    </DialogContentText>
                    <form onSubmit={handleAddCategoryDiscount} id="categoryDiscountForm" >
                        <TextField
                            value={category}
                            autoFocus
                            margin="dense"
                            id="category"
                            label="Category Name"
                            fullWidth
                            variant="standard"
                            onChange={(e) => setCategory(e.target.value)}
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
                    <Button onClick={handleCloseCategoryDiscount}>Back</Button>
                    <Button variant="contained" type="submit" form="categoryDiscountForm">Add</Button>
                </DialogActions>
            </Dialog>

            <Dialog open={storeDiscountOpen} onClose={handleCloseStoreDiscount}>
                <DialogTitle>Add Store Discount</DialogTitle>
                <DialogContent>
                    <DialogContentText>
                        Please insert store discount details
                    </DialogContentText>
                    <form onSubmit={handleAddStoreDiscount} id="storeDiscountForm" >
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
                    <Button onClick={handleCloseStoreDiscount}>Back</Button>
                    <Button variant="contained" type="submit" form="storeDiscountForm">Add</Button>
                </DialogActions>
            </Dialog>
        </div>
    );
}
