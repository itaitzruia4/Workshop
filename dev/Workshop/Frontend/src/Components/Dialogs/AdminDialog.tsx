import * as React from 'react';
import Button from '@mui/material/Button';
import TextField from '@mui/material/TextField';
import Dialog from '@mui/material/Dialog';
import DialogActions from '@mui/material/DialogActions';
import DialogContent from '@mui/material/DialogContent';
import DialogContentText from '@mui/material/DialogContentText';
import DialogTitle from '@mui/material/DialogTitle';
import Table from '@mui/material/Table';
import TableBody from '@mui/material/TableBody';
import TableCell from '@mui/material/TableCell';
import TableContainer from '@mui/material/TableContainer';
import TableHead from '@mui/material/TableHead';
import TableRow from '@mui/material/TableRow';
import Paper from '@mui/material/Paper';


import { handleGetMemberInformation, getStorePurchaseHistory, removeMember, viewStatistics } from '../../Actions/AdminActions';
import { memberToken } from '../../Types/roles';

export default function AdminDialog(isOpen: boolean, token: memberToken) {

    const [open, setOpen] = React.useState(isOpen);
    const [removeMemberOpen, setRemoveMemberOpen] = React.useState(false);
    const [historyOpen, setHistoryOpen] = React.useState(false);
    const [memberInfoOpen, setMemberInfoOpen] = React.useState(false);
    const [incomeOpen, setIncomeOpen] = React.useState(false);
    const [statsOpen, setStatsOpen] = React.useState(false);

    const [storeId, setStoreId] = React.useState(0);
    const [membername, setMembername] = React.useState("");
    const [fromDate, setFromDate] = React.useState("");
    const [toDate, setToDate] = React.useState("");

    const [onlineMembers, setOnlineMembers] = React.useState([]);
    const [offlineMembers, setOfflineMembers] = React.useState([]);


    const handleOpenManager = () => {
        setOpen(true);
    };

    const handleCloseManager = () => {
        setOpen(false);
    };

    const handleOpenRemoveMember = () => {
        setOpen(false);
        setRemoveMemberOpen(true);
    };

    const handleCloseRemoveMember = () => {
        setRemoveMemberOpen(false);
        setOpen(true);
    };

    const handleOpenHistory = () => {
        setOpen(false);
        setHistoryOpen(true);
    };

    const handleCloseHistory = () => {
        setHistoryOpen(false);
        setOpen(true);
    };

    const handleOpenMemberInfo = () => {
        setOpen(false);
        handleGetMemberInfo();
        setMemberInfoOpen(true);
    };

    const handleCloseMemberInfo = () => {
        setMemberInfoOpen(false);
        setOpen(true);
    };

    const handleOpenIncome = () => {
        setOpen(false);
        setIncomeOpen(true);
    };

    const handleCloseIncome = () => {
        setIncomeOpen(false);
        setOpen(true);
    };

    const handleOpenStats = () => {
        setOpen(false);
        setStatsOpen(true);
    };

    const handleCloseStats = () => {
        setStatsOpen(false);
        setOpen(true);
    };

    const handleRemoveMember = (e: React.FormEvent<HTMLFormElement>) => {
        e.preventDefault();

        removeMember(token, membername);
        setMembername("");
        handleCloseRemoveMember();
    };

    const handleGetStorePurchaseHistory = (e: React.FormEvent<HTMLFormElement>) => {
        e.preventDefault();
        // TODO figure out how to load purchase history into the dialog
        getStorePurchaseHistory(token, storeId);
        setStoreId(0);
        handleCloseHistory();
    };

    const handleGetMemberInfo = () => {
        handleGetMemberInformation(token)
            .then(data => {
                setOnlineMembers(data.key);
                setOfflineMembers(data.value);
            })
            .catch(error => {
                alert(error)
            });
        handleCloseMemberInfo();
    };

    const handleGetDailyIncome = (e: React.FormEvent<HTMLFormElement>) => {
        e.preventDefault();
        // TODO implement
        handleCloseIncome();
    };

    const handleViewStats = (e: React.FormEvent<HTMLFormElement>) => {
        e.preventDefault();
        viewStatistics(token, fromDate, toDate);
        setFromDate("");
        setToDate("");
        handleCloseStats();
    };

    return (
        <div>
            <Button onClick={handleOpenManager}>
                Admin Menu
            </Button>
            <Dialog open={open} onClose={handleCloseManager}>
                <DialogTitle>Admin Menu</DialogTitle>
                <DialogContent>
                    <Button onClick={handleOpenRemoveMember}>Remove Member</Button>
                    <Button onClick={handleOpenHistory}>View Store Purchase History</Button>
                    <Button onClick={handleOpenMemberInfo}>Members Information</Button>
                    <Button onClick={handleOpenIncome}>Daily Market Income</Button>
                    <Button onClick={handleOpenStats}>Market Statistics</Button>
                </DialogContent>
                <DialogActions>
                    <Button onClick={handleCloseManager}>Close</Button>
                </DialogActions>
            </Dialog>

            <Dialog open={removeMemberOpen} onClose={handleCloseRemoveMember}>
                <DialogTitle>Remove Member</DialogTitle>
                <DialogContent>
                    <DialogContentText>
                        Please insert the username of the member to be removed
                    </DialogContentText>
                    <form onSubmit={handleRemoveMember} id="removeMemberForm" >
                        <TextField
                            value={membername}
                            autoFocus
                            margin="dense"
                            id="membername"
                            label="Member Name"
                            fullWidth
                            variant="standard"
                            onChange={(e) => setMembername(e.target.value)}
                        />
                    </form>
                </DialogContent>
                <DialogActions>
                    <Button onClick={handleCloseRemoveMember}>Back</Button>
                    <Button variant="contained" type="submit" form="removeMemberForm">OK</Button>
                </DialogActions>
            </Dialog>

            <Dialog open={historyOpen} onClose={handleCloseHistory}>
                <DialogTitle>Store Purchase History</DialogTitle>
                <DialogContent>
                    <DialogContentText>
                        Purchase History
                    </DialogContentText>
                    <form onSubmit={handleGetStorePurchaseHistory} id="historyForm" >
                        <TextField
                            value={storeId}
                            autoFocus
                            margin="dense"
                            id="storeId"
                            label="Store ID"
                            type="number"
                            fullWidth
                            variant="standard"
                            onChange={(e) => setStoreId(Number(e.target.value))}
                        />
                    </form>
                </DialogContent>
                <DialogActions>
                    <Button onClick={handleCloseHistory}>Back</Button>
                    <Button variant="contained" type="submit" form="historyForm">OK</Button>
                </DialogActions>
            </Dialog>

            <Dialog open={memberInfoOpen} onClose={handleCloseMemberInfo}>
                <DialogTitle>Members Information</DialogTitle>
                <DialogContent>
                    <TableContainer component={Paper}>
                        <Table sx={{ minWidth: 100 }} aria-label="online table">
                            <TableHead>
                                <TableRow>
                                    <TableCell align="center" size="medium">ONLINE</TableCell>
                                </TableRow>
                            </TableHead>
                            <TableBody>
                                {onlineMembers.map((row) => (
                                    <TableRow
                                        key={row}
                                        sx={{ '&:last-child td, &:last-child th': { border: 0 } }}
                                    >
                                        <TableCell component="th" scope="row">
                                            {row}
                                        </TableCell>
                                    </TableRow>
                                ))}
                            </TableBody>
                        </Table>
                        <br/>
                        <Table sx={{ minWidth: 100 }} aria-label="offline table">
                            <TableHead>
                                <TableRow>
                                    <TableCell align="center" size="medium">OFFLINE</TableCell>
                                </TableRow>
                            </TableHead>
                            <TableBody>
                                {offlineMembers.map((row) => (
                                    <TableRow
                                        key={row}
                                        sx={{ '&:last-child td, &:last-child th': { border: 0 } }}
                                    >
                                        <TableCell component="th" scope="row">
                                            {row}
                                        </TableCell>
                                    </TableRow>
                                ))}
                            </TableBody>
                        </Table>
                    </TableContainer>
                </DialogContent>
                <DialogActions>
                    <Button onClick={handleCloseMemberInfo}>Back</Button>
                </DialogActions>
            </Dialog>

            <Dialog open={statsOpen} onClose={handleCloseStats}>
                <DialogTitle>Daily Statistics</DialogTitle>
                <DialogContent>
                    <DialogContentText>
                        Please insert start and end date
                    </DialogContentText>
                    <form onSubmit={handleViewStats} id="statsForm" >
                        <TextField
                            value={fromDate}
                            autoFocus
                            margin="dense"
                            id="fromdate"
                            label="Start Date"
                            fullWidth
                            variant="standard"
                            onChange={(e) => setFromDate(e.target.value)}
                        />
                        <TextField
                            value={toDate}
                            autoFocus
                            margin="dense"
                            id="todate"
                            label="End Date"
                            fullWidth
                            variant="standard"
                            onChange={(e) => setToDate(e.target.value)}
                        />
                    </form>
                </DialogContent>
                <DialogActions>
                    <Button onClick={handleCloseStats}>Back</Button>
                    <Button variant="contained" type="submit" form="statsForm">OK</Button>
                </DialogActions>
            </Dialog>
        </div>
    );
}
