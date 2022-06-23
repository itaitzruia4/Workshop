import * as React from 'react';
import Button from '@mui/material/Button';
import TextField from '@mui/material/TextField';
import Dialog from '@mui/material/Dialog';
import DialogActions from '@mui/material/DialogActions';
import DialogContent from '@mui/material/DialogContent';
import DialogContentText from '@mui/material/DialogContentText';
import DialogTitle from '@mui/material/DialogTitle';
import Radio from '@mui/material/Radio';
import RadioGroup from '@mui/material/RadioGroup';
import FormControlLabel from '@mui/material/FormControlLabel';
import FormControl from '@mui/material/FormControl';
import FormLabel from '@mui/material/FormLabel';
import { makeCategoryPriceActionSimple, makeProductPriceActionSimple, makeSimpleDiscount, makeStorePriceActionSimple, SimpleDiscount } from '../../Types/discount';
import { memberToken, Actions, StorePermission, actionNames } from '../../Types/roles';



export default function StoreRolesDialog(
    props: {
        storeId: number,
        nominateStoreOwner: (storeId: number, nominee: string) => void,
        nominateStoreManager: (storeId: number, nominee: string) => void,
        removeStoreOwnerNomination: (storeId: number, nominee: string) => void
        addActionToManager: (nominee: string, storeId: number, action: string) => void
    }){
    const { storeId, nominateStoreOwner, nominateStoreManager, removeStoreOwnerNomination, addActionToManager } = props
    const [open, setOpen] = React.useState(false);
    const [nominateStoreOwnerOpen, setNominateStoreOwnerOpen] = React.useState(false);
    const [nominateStoreManagerOpen, setNominateStoreManagerOpen] = React.useState(false);
    const [removeStoreOwnerNominationOpen, setRemoveStoreOwnerNominationOpen] = React.useState(false);
    const [addActionToManagerOpen, setAddActionToManagerOpen] = React.useState(false);

    const [nominee, setNominee] = React.useState("");
    const [action, setAction] = React.useState("");

    const actions = actionNames();

    const handleOpenStoreRoles = () => {
        setOpen(true);
    };

    const handleCloseStoreRoles = () => {
        setOpen(false);
    };

    const handleOpenNominateStoreOwner = () => {
        setOpen(false);
        setNominateStoreOwnerOpen(true);
    };

    const handleCloseNominateStoreOwner = () => {
        setNominateStoreOwnerOpen(false);
        setOpen(true);
    };

    const handleOpenNominateStoreManager = () => {
        setOpen(false);
        setNominateStoreManagerOpen(true);
    };

    const handleCloseNominateStoreManager = () => {
        setNominateStoreManagerOpen(false);
        setOpen(true);
    };

    const handleOpenRemoveStoreOwnerNomination = () => {
        setOpen(false);
        setRemoveStoreOwnerNominationOpen(true);
    };

    const handleCloseRemoveStoreOwnerNomination = () => {
        setRemoveStoreOwnerNominationOpen(false);
        setOpen(true);
    };

    const handleOpenAddActionToManager = () => {
        setOpen(false);
        setAddActionToManagerOpen(true);
    };

    const handleCloseAddActionToManager = () => {
        setAddActionToManagerOpen(false);
        setOpen(true);
    };






    const handleNominateStoreOwner = (e: React.FormEvent<HTMLFormElement>) => {
        e.preventDefault();

        nominateStoreOwner(storeId, nominee);
        setNominee("");
        handleCloseNominateStoreOwner();
    };

    const handleNominateStoreManager = (e: React.FormEvent<HTMLFormElement>) => {
        e.preventDefault();

        nominateStoreManager(storeId, nominee);
        setNominee("");
        handleCloseNominateStoreManager();
    };

    const handleRemoveStoreOwnerNomination = (e: React.FormEvent<HTMLFormElement>) => {
        e.preventDefault();

        removeStoreOwnerNomination(storeId, nominee);
        setNominee("");
        handleCloseRemoveStoreOwnerNomination();
    };

    const handleAddActionToManager = (e: React.FormEvent<HTMLFormElement>) => {
        e.preventDefault();
        
        addActionToManager(nominee, storeId, action);
        setNominee("");
        handleCloseAddActionToManager();
    };

    return (
        <div>
            <Button onClick={handleOpenStoreRoles}>
                Manage Store Roles
            </Button>
            <Dialog open={open} onClose={handleCloseStoreRoles}>
                <DialogTitle>Manage Store Roles</DialogTitle>
                <DialogContent>
                    <Button onClick={handleOpenNominateStoreOwner}>Nominate Store Owner</Button>
                    <Button onClick={handleOpenNominateStoreManager}>Nominate Store Manager</Button>
                    <Button onClick={handleOpenRemoveStoreOwnerNomination}>Remove Store Owner Nomination</Button>
                    <Button onClick={handleOpenAddActionToManager}>Add action to Manager</Button>
                </DialogContent>
                <DialogActions>
                    <Button onClick={handleCloseStoreRoles}>Close</Button>
                </DialogActions>
            </Dialog>

            <Dialog open={nominateStoreOwnerOpen} onClose={handleCloseNominateStoreOwner}>
                <DialogTitle>Nominate Store Owner</DialogTitle>
                <DialogContent>
                    <DialogContentText>
                        Please insert member name to be nominated
                    </DialogContentText>
                    <form onSubmit={handleNominateStoreOwner} id="nominateOwnerForm" >
                        <TextField
                            value={nominee}
                            autoFocus
                            margin="dense"
                            id="owner_nominee"
                            label="Member to be nominated to store owner:"
                            fullWidth
                            variant="standard"
                            onChange={(e) => setNominee(e.target.value)}
                        />
                    </form>
                </DialogContent>
                <DialogActions>
                    <Button onClick={handleCloseNominateStoreOwner}>Back</Button>
                    <Button variant="contained" type="submit" form="nominateOwnerForm">Nominate</Button>
                </DialogActions>
            </Dialog>

            <Dialog open={nominateStoreManagerOpen} onClose={handleCloseNominateStoreManager}>
                <DialogTitle>Nominate Store Manager</DialogTitle>
                <DialogContent>
                    <DialogContentText>
                        Please insert member name to be nominated
                    </DialogContentText>
                    <form onSubmit={handleNominateStoreManager} id="nominateManagerForm" >
                        <TextField
                            value={nominee}
                            autoFocus
                            margin="dense"
                            id="owner_nominee"
                            label="Member to be nominated to store manager:"
                            fullWidth
                            variant="standard"
                            onChange={(e) => setNominee(e.target.value)}
                        />
                    </form>
                </DialogContent>
                <DialogActions>
                    <Button onClick={handleCloseNominateStoreManager}>Back</Button>
                    <Button variant="contained" type="submit" form="nominateManagerForm">Nominate</Button>
                </DialogActions>
            </Dialog>

            <Dialog open={removeStoreOwnerNominationOpen} onClose={handleCloseRemoveStoreOwnerNomination}>
                <DialogTitle>Remove Store Owner Nomination</DialogTitle>
                <DialogContent>
                    <DialogContentText>
                        Please insert store owner name to be removed:
                    </DialogContentText>
                    <form onSubmit={handleRemoveStoreOwnerNomination} id="removeOwnerForm" >
                        <TextField
                            value={nominee}
                            autoFocus
                            margin="dense"
                            id="owner_nominee"
                            label="Store owner to be removed:"
                            fullWidth
                            variant="standard"
                            onChange={(e) => setNominee(e.target.value)}
                        />
                    </form>
                </DialogContent>
                <DialogActions>
                    <Button onClick={handleCloseRemoveStoreOwnerNomination}>Back</Button>
                    <Button variant="contained" type="submit" form="removeOwnerForm">Remove Nomination</Button>
                </DialogActions>
            </Dialog>

            <Dialog open={addActionToManagerOpen} onClose={handleAddActionToManager}>
                <DialogTitle>Add action to Member</DialogTitle>
                <DialogContent>
                    <DialogContentText>
                        Please insert store manager name to be nominated and action:
                    </DialogContentText>
                    <form onSubmit={handleAddActionToManager} id="removeOwnerForm" >
                        <TextField
                            value={nominee}
                            autoFocus
                            margin="dense"
                            id="owner_nominee"
                            label="Store manager's name to be nominated:"
                            fullWidth
                            variant="standard"
                            onChange={(e) => setNominee(e.target.value)}
                        />
                        <FormControl>
                            <FormLabel>Action</FormLabel>
                            <RadioGroup
                                aria-labelledby="demo-controlled-radio-buttons-group"
                                name="controlled-radio-buttons-group"
                                value={action}
                                onChange={(event: React.ChangeEvent<HTMLInputElement>) => {
                                    setAction((event.target as HTMLInputElement).value);
                                }}
                            >
                                {actions.map(action => {
                                    return (<FormControlLabel value={action} control={<Radio />} label={action } />);
                                    })
                                }
                            </RadioGroup>
                        </FormControl>
                    </form>
                </DialogContent>
                <DialogActions>
                    <Button onClick={handleCloseAddActionToManager}>Back</Button>
                    <Button variant="contained" type="submit" form="removeOwnerForm">Add action</Button>
                </DialogActions>
            </Dialog>

        </div>
    );
}
