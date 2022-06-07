import * as React from 'react';
import Button from '@mui/material/Button';
import TextField from '@mui/material/TextField';
import Dialog from '@mui/material/Dialog';
import DialogActions from '@mui/material/DialogActions';
import DialogContent from '@mui/material/DialogContent';
import DialogContentText from '@mui/material/DialogContentText';
import DialogTitle from '@mui/material/DialogTitle';
import { BagPurchaseSimpleTerm, CategoryPurchaseSimpleTerm, makeBagPurchaseSimpleTerm, makeCategoryPurchaseSimpleTerm, makeProductPurchaseSimpleTerm, ProductPurchaseSimpleTerm, UserPurchaseSimpleTerm, makeUserPurchaseSimpleTerm } from '../../Types/purchase';

export default function AddPurchaseDialog(
    storeId: number,
    addPurchase: (storeId: number, purchaseJson: string) => void,
    addProductPurchase: (storeId: number, productId: number, purchaseJson: string) => void,
    addCategoryPurchase: (storeId: number, category: string, purchaseJson: string) => void,
    addBagPurchase: (storeId: number, purchaseJson: string) => void,
    addUserPurchase: (storeId: number, purchaseJson: string) => void) {

    const [open, setOpen] = React.useState(false);
    const [productOpen, setProductOpen] = React.useState(false);
    const [categoryOpen, setCategoryOpen] = React.useState(false);
    const [bagPurchaseOpen, setBagPurchaseOpen] = React.useState(false);
    const [userPurchaseOpen, setUserPurchaseOpen] = React.useState(false);

    const [purchase, setPurchase] = React.useState("");
    const [type, setType] = React.useState("");
    const [action, setAction] = React.useState("");
    const [value, setValue] = React.useState("");
    const [productId, setProductId] = React.useState(0);
    const [category, setCategory] = React.useState("");
    const [age, setAge] = React.useState(0);

    const handleOpenPurchase = () => {
        setOpen(true);
    };

    const handleClosePurchase = () => {
        setOpen(false);
    };

    const handleOpenProductPurchase = () => {
        setOpen(false);
        setProductOpen(true);
    };

    const handleCloseProductPurchase = () => {
        setProductOpen(false);
        setOpen(true);
    };

    const handleOpenCategoryPurchase = () => {
        setOpen(false);
        setCategoryOpen(true);
    };

    const handleCloseCategoryPurchase = () => {
        setCategoryOpen(false);
        setOpen(true);
    };

    const handleOpenBagPurchase = () => {
        setOpen(false);
        setBagPurchaseOpen(true);
    };

    const handleCloseBagPurchase = () => {
        setBagPurchaseOpen(false);
        setOpen(true);
    };

    const handleAdd = (e: React.FormEvent<HTMLFormElement>) => {
        e.preventDefault();
        addPurchase(storeId, purchase);
        setPurchase("");
        handleClosePurchase();
    };

    const handleAddProductPurchase = (e: React.FormEvent<HTMLFormElement>) => {
        e.preventDefault();

        const productPurchase: ProductPurchaseSimpleTerm = makeProductPurchaseSimpleTerm(type, action, value, productId);

        addProductPurchase(storeId, productId, JSON.stringify(productPurchase));

        setProductId(0);
        setType("");
        setAction("");
        setValue("");

        handleCloseProductPurchase();
    };

    const handleAddCategoryPurchase = (e: React.FormEvent<HTMLFormElement>) => {
        e.preventDefault();

        const categoryPurchase: CategoryPurchaseSimpleTerm = makeCategoryPurchaseSimpleTerm(type, action, value, category);

        addCategoryPurchase(storeId, category, JSON.stringify(categoryPurchase));

        setCategory("");
        setType("");
        setAction("");
        setValue("");

        handleCloseCategoryPurchase();
    };

    const handleAddBagPurchase = (e: React.FormEvent<HTMLFormElement>) => {
        e.preventDefault();

        const bagPurchase: BagPurchaseSimpleTerm = makeBagPurchaseSimpleTerm(type, action, value);

        addBagPurchase(storeId, JSON.stringify(bagPurchase));

        setType("");
        setAction("");
        setValue("");

        handleCloseBagPurchase();
    };

    const handleAddUserPurchase = (e: React.FormEvent<HTMLFormElement>) => {
        e.preventDefault();

        const userPurchase: UserPurchaseSimpleTerm = makeUserPurchaseSimpleTerm(action, age);

        addBagPurchase(storeId, JSON.stringify(userPurchase));

        setAge(0);
        setAction("");

        handleCloseBagPurchase();
    };

    return (
        <div>
            <Button onClick={handleOpenPurchase}>
                Add Purchase
            </Button>
            <Dialog open={open} onClose={handleClosePurchase}>
                <DialogTitle>Add Purchase</DialogTitle>
                <DialogContent>
                    <Button onClick={handleOpenProductPurchase}>Add Product Purchase Term</Button>
                    <Button onClick={handleOpenCategoryPurchase}>Add Category Purchase Term</Button>
                    <Button onClick={handleOpenBagPurchase}>Add Store Purchase Term</Button>
                    <Button onClick={handleOpenUserPurchase}>Add User Purchase Term</Button>
                    <DialogContentText>
                        For complex Purchases, please insert Purchase JSON string below:
                    </DialogContentText>
                    <form onSubmit={handleAdd} id="PurchaseForm" >
                        <TextField
                            value={Purchase}
                            autoFocus
                            margin="dense"
                            id="name"
                            fullWidth
                            variant="standard"
                            onChange={(e) => setPurchase(e.target.value)}
                        />
                    </form>
                </DialogContent>
                <DialogActions>
                    <Button onClick={handleClosePurchase}>Cancel</Button>
                    <Button variant="contained" type="submit" form="PurchaseForm">Add</Button>
                </DialogActions>
            </Dialog>

            <Dialog open={productOpen} onClose={handleCloseProductPurchase}>
                <DialogTitle>Add Product Purchase</DialogTitle>
                <DialogContent>
                    <DialogContentText>
                        Please insert product Purchase details
                    </DialogContentText>
                    <form onSubmit={handleAddProductPurchase} id="productPurchaseForm" >
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
                            label="Purchase Percentage"
                            type="number"
                            fullWidth
                            variant="standard"
                            onChange={(e) => setPercentage(Number(e.target.value))}
                        />
                    </form>
                </DialogContent>
                <DialogActions>
                    <Button onClick={handleCloseProductPurchase}>Back</Button>
                    <Button variant="contained" type="submit" form="productPurchaseForm">Add</Button>
                </DialogActions>
            </Dialog>

            <Dialog open={categoryOpen} onClose={handleCloseCategoryPurchase}>
                <DialogTitle>Add Category Purchase</DialogTitle>
                <DialogContent>
                    <DialogContentText>
                        Please insert category Purchase details
                    </DialogContentText>
                    <form onSubmit={handleAddCategoryPurchase} id="categoryPurchaseForm" >
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
                            label="Purchase Percentage"
                            type="number"
                            fullWidth
                            variant="standard"
                            onChange={(e) => setPercentage(Number(e.target.value))}
                        />
                    </form>
                </DialogContent>
                <DialogActions>
                    <Button onClick={handleCloseCategoryPurchase}>Back</Button>
                    <Button variant="contained" type="submit" form="categoryPurchaseForm">Add</Button>
                </DialogActions>
            </Dialog>

            <Dialog open={storePurchaseOpen} onClose={handleCloseStorePurchase}>
                <DialogTitle>Add Store Purchase</DialogTitle>
                <DialogContent>
                    <DialogContentText>
                        Please insert store Purchase details
                    </DialogContentText>
                    <form onSubmit={handleAddStorePurchase} id="storePurchaseForm" >
                        <TextField
                            value={percentage}
                            autoFocus
                            margin="dense"
                            id="percentage"
                            label="Purchase Percentage"
                            type="number"
                            fullWidth
                            variant="standard"
                            onChange={(e) => setPercentage(Number(e.target.value))}
                        />
                    </form>
                </DialogContent>
                <DialogActions>
                    <Button onClick={handleCloseStorePurchase}>Back</Button>
                    <Button variant="contained" type="submit" form="storePurchaseForm">Add</Button>
                </DialogActions>
            </Dialog>
        </div>
    );
}
