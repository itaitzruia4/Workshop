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
    props: {
        storeId: number,
        addProductPurchase: (storeId: number, productId: number, purchaseJson: string) => void,
        addCategoryPurchase: (storeId: number, category: string, purchaseJson: string) => void,
        addBagPurchase: (storeId: number, purchaseJson: string) => void,
        addUserPurchase: (storeId: number, purchaseJson: string) => void
    }) {
    const { storeId, addProductPurchase, addCategoryPurchase, addBagPurchase, addUserPurchase } = props;
    const PURCHASE_TYPES = ["p", "q", "h", "d"]
    const NON_USER_PURCHASE_ACTIONS = ["<", ">", "=", "!=", ">=", "<="]
    const USER_PURCHASE_ACTIONS = [">", "!=", ">="]

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

    const handleOpenUserPurchase = () => {
        setOpen(false);
        setUserPurchaseOpen(true);
    };

    const handleCloseUserPurchase = () => {
        setUserPurchaseOpen(false);
        setOpen(true);
    };

    const handleAdd = (e: React.FormEvent<HTMLFormElement>) => {
        e.preventDefault();
        addBagPurchase(storeId, purchase);
        setPurchase("");
        handleClosePurchase();
    };

    const handleAddProductPurchase = (e: React.FormEvent<HTMLFormElement>) => {
        e.preventDefault();

        if (!PURCHASE_TYPES.includes(type)) {
            alert(`Purchase type must be one of the following: ${JSON.stringify(PURCHASE_TYPES)}`);
            return;
        }

        if (!NON_USER_PURCHASE_ACTIONS.includes(action)) {
            alert(`Purchase action must be one of the following: ${JSON.stringify(NON_USER_PURCHASE_ACTIONS)}`);
            return;
        }

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

        if (!PURCHASE_TYPES.includes(type)) {
            alert(`Purchase type must be one of the following: ${JSON.stringify(PURCHASE_TYPES)}`);
            return;
        }

        if (!NON_USER_PURCHASE_ACTIONS.includes(action)) {
            alert(`Purchase action must be one of the following: ${JSON.stringify(NON_USER_PURCHASE_ACTIONS)}`);
            return;
        }

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

        if (!PURCHASE_TYPES.includes(type)) {
            alert(`Purchase type must be one of the following: ${JSON.stringify(PURCHASE_TYPES)}`);
            return;
        }

        if (!NON_USER_PURCHASE_ACTIONS.includes(action)) {
            alert(`Purchase action must be one of the following: ${JSON.stringify(NON_USER_PURCHASE_ACTIONS)}`);
            return;
        }

        const bagPurchase: BagPurchaseSimpleTerm = makeBagPurchaseSimpleTerm(type, action, value);

        addBagPurchase(storeId, JSON.stringify(bagPurchase));

        setType("");
        setAction("");
        setValue("");

        handleCloseBagPurchase();
    };

    const handleAddUserPurchase = (e: React.FormEvent<HTMLFormElement>) => {
        e.preventDefault();

        if (!USER_PURCHASE_ACTIONS.includes(action)) {
            alert(`Purchase action must be one of the following: ${JSON.stringify(USER_PURCHASE_ACTIONS)}`);
            return;
        }

        const userPurchase: UserPurchaseSimpleTerm = makeUserPurchaseSimpleTerm(action, age);

        addUserPurchase(storeId, JSON.stringify(userPurchase));

        setAge(0);
        setAction("");

        handleCloseUserPurchase();
    };

    return (
        <div>
            <Button onClick={handleOpenPurchase}>
                Add Purchase Policy
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
                    <form onSubmit={handleAdd} id="purchaseForm" >
                        <TextField
                            value={purchase}
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
                    <Button variant="contained" type="submit" form="purchaseForm">Add</Button>
                </DialogActions>
            </Dialog>

            <Dialog open={productOpen} onClose={handleCloseProductPurchase}>
                <DialogTitle>Add Product Purchase Policy</DialogTitle>
                <DialogContent>
                    <DialogContentText>
                        Please insert product purchase details
                    </DialogContentText>
                    <form onSubmit={handleAddProductPurchase} id="productPurchaseForm" >
                        <TextField
                            value={type}
                            autoFocus
                            margin="dense"
                            id="type"
                            label="Purchase Policy Type (p, q, h, d)"
                            fullWidth
                            variant="standard"
                            onChange={(e) => setType(e.target.value)}
                        />
                        <TextField
                            value={action}
                            autoFocus
                            margin="dense"
                            id="action"
                            label="Purchase Policy Action (<, >, =, !=, >=, <=)"
                            fullWidth
                            variant="standard"
                            onChange={(e) => setAction(e.target.value)}
                        />
                        <TextField
                            value={value}
                            autoFocus
                            margin="dense"
                            id="value"
                            label="Purchase Policy Value"
                            fullWidth
                            variant="standard"
                            onChange={(e) => setValue(e.target.value)}
                        />
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
                    </form>
                </DialogContent>
                <DialogActions>
                    <Button onClick={handleCloseProductPurchase}>Back</Button>
                    <Button variant="contained" type="submit" form="productPurchaseForm">Add</Button>
                </DialogActions>
            </Dialog>

            <Dialog open={categoryOpen} onClose={handleCloseCategoryPurchase}>
                <DialogTitle>Add Category Purchase Policy</DialogTitle>
                <DialogContent>
                    <DialogContentText>
                        Please insert category Purchase details
                    </DialogContentText>
                    <form onSubmit={handleAddCategoryPurchase} id="categoryPurchaseForm" >
                        <TextField
                            value={type}
                            autoFocus
                            margin="dense"
                            id="type"
                            label="Purchase Policy Type (p, q, h, d)"
                            fullWidth
                            variant="standard"
                            onChange={(e) => setType(e.target.value)}
                        />
                        <TextField
                            value={action}
                            autoFocus
                            margin="dense"
                            id="action"
                            label="Purchase Policy Action (<, >, =, !=, >=, <=)"
                            fullWidth
                            variant="standard"
                            onChange={(e) => setAction(e.target.value)}
                        />
                        <TextField
                            value={value}
                            autoFocus
                            margin="dense"
                            id="value"
                            label="Purchase Policy Value"
                            fullWidth
                            variant="standard"
                            onChange={(e) => setValue(e.target.value)}
                        />
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
                    </form>
                </DialogContent>
                <DialogActions>
                    <Button onClick={handleCloseCategoryPurchase}>Back</Button>
                    <Button variant="contained" type="submit" form="categoryPurchaseForm">Add</Button>
                </DialogActions>
            </Dialog>

            <Dialog open={bagPurchaseOpen} onClose={handleCloseBagPurchase}>
                <DialogTitle>Add Bag Purchase Policy</DialogTitle>
                <DialogContent>
                    <DialogContentText>
                        Please insert bag purchase policy details
                    </DialogContentText>
                    <form onSubmit={handleAddBagPurchase} id="bagPurchaseForm" >
                        <TextField
                            value={type}
                            autoFocus
                            margin="dense"
                            id="type"
                            label="Purchase Policy Type (p, q, h, d)"
                            fullWidth
                            variant="standard"
                            onChange={(e) => setType(e.target.value)}
                        />
                        <TextField
                            value={action}
                            autoFocus
                            margin="dense"
                            id="action"
                            label="Purchase Policy Action (<, >, =, !=, >=, <=)"
                            fullWidth
                            variant="standard"
                            onChange={(e) => setAction(e.target.value)}
                        />
                        <TextField
                            value={value}
                            autoFocus
                            margin="dense"
                            id="value"
                            label="Purchase Policy Value"
                            fullWidth
                            variant="standard"
                            onChange={(e) => setValue(e.target.value)}
                        />
                    </form>
                </DialogContent>
                <DialogActions>
                    <Button onClick={handleCloseBagPurchase}>Back</Button>
                    <Button variant="contained" type="submit" form="bagPurchaseForm">Add</Button>
                </DialogActions>
            </Dialog>


            <Dialog open={userPurchaseOpen} onClose={handleCloseUserPurchase}>
                <DialogTitle>Add User Purchase Policy</DialogTitle>
                <DialogContent>
                    <DialogContentText>
                        Please insert user purchase policy details
                    </DialogContentText>
                    <form onSubmit={handleAddUserPurchase} id="userPurchaseForm" >
                        <TextField
                            value={action}
                            autoFocus
                            margin="dense"
                            id="action"
                            label="Purchase Policy Action (>, !=, >=)"
                            fullWidth
                            variant="standard"
                            onChange={(e) => setAction(e.target.value)}
                        />
                        <TextField
                            value={age}
                            autoFocus
                            margin="dense"
                            id="age"
                            label="User Age"
                            type="number"
                            fullWidth
                            variant="standard"
                            onChange={(e) => setAge(Number(e.target.value))}
                        />
                    </form>
                </DialogContent>
                <DialogActions>
                    <Button onClick={handleCloseUserPurchase}>Back</Button>
                    <Button variant="contained" type="submit" form="userPurchaseForm">Add</Button>
                </DialogActions>
            </Dialog>

        </div>
    );
}
