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
import {DataGrid, GridColDef} from "@mui/x-data-grid"; 


import { handleGetMemberInformation, removeMember, getDailyIncome, handleViewStatistics } from '../../Actions/AdminActions';
import { memberToken } from '../../Types/roles';
import { Order } from '../../Types/store';
import { handleGetStorePurchaseHistory } from '../../Actions/StoreActions';

interface Statistics { id: number, date: string, marketManagers: number, guests: number, members: number, storeOwners: number, storeManagers: number }
interface APIStatistics { date: string, marketManagers: number, guests: number, members: number, storeOwners: number, storeManagers: number }

export default function AdminDialog(isOpen: boolean, token: memberToken) {

    const [open, setOpen] = React.useState(isOpen);
    const [removeMemberOpen, setRemoveMemberOpen] = React.useState(false);
    const [historyInputOpen, setHistoryInputOpen] = React.useState(false);
    const [historyOpen, setHistoryOpen] = React.useState(false);
    const [memberInfoOpen, setMemberInfoOpen] = React.useState(false);
    const [statsInputOpen, setStatsInputOpen] = React.useState(false);
    const [statsOpen, setStatsOpen] = React.useState(false);

    const [storeId, setStoreId] = React.useState(0);
    const [membername, setMembername] = React.useState("");
    const [fromDate, setFromDate] = React.useState("");
    const [toDate, setToDate] = React.useState("");

    const [statistics, setStatistics] = React.useState<Statistics[] | null>(null);

    const statsColumns: GridColDef[] = [
        { field: "date", headerName: "Date", flex: 2 },
        { field: "marketManagers", headerName: "Market Managers", flex: 1 },
        { field: "storeOwners", headerName: "Store Owners", flex: 1 },
        { field: "storeManagers", headerName: "Store Managers", flex: 1 },
        { field: "members", headerName: "Members", flex: 1 },
        { field: "guests", headerName: "Guests", flex: 1 }
    ];

    const orderColumns: GridColDef[] = [
        { field: "id", headerName: "Order ID", flex: 2 },
        { field: "buyerName", headerName: "Buyer Name", flex: 1 },
        { field: "date", headerName: "Order Date", flex: 1 },
        { field: "price", headerName: "Order Date", flex: 1 }
    ];

    const [orders, setOrders] = React.useState<Order[]>([])

    const updateStats = (oldStats: Statistics[], role: string): void => {
        console.log(oldStats);
        if (oldStats !== null) {
            const todayIdx = oldStats.length - 1;
            switch (role) {
                case "GUEST":
                    setStatistics(oldStats.slice(0, todayIdx).concat([{
                        id: oldStats[todayIdx].id,
                        date: oldStats[todayIdx].date,
                        marketManagers: oldStats[todayIdx].marketManagers,
                        guests: oldStats[todayIdx].guests + 1,
                        members: oldStats[todayIdx].members,
                        storeOwners: oldStats[todayIdx].storeOwners,
                        storeManagers: oldStats[todayIdx].storeManagers
                    }]));
                    break;
                case "MEMBER":
                    setStatistics(oldStats.slice(0, todayIdx).concat([{
                        id: oldStats[todayIdx].id,
                        date: oldStats[todayIdx].date,
                        marketManagers: oldStats[todayIdx].marketManagers,
                        guests: oldStats[todayIdx].guests - 1,
                        members: oldStats[todayIdx].members + 1,
                        storeOwners: oldStats[todayIdx].storeOwners,
                        storeManagers: oldStats[todayIdx].storeManagers
                    }]));
                    break;
                case "STOREMANAGER":
                    setStatistics(oldStats.slice(0, todayIdx).concat([{
                        id: oldStats[todayIdx].id,
                        date: oldStats[todayIdx].date,
                        marketManagers: oldStats[todayIdx].marketManagers,
                        guests: oldStats[todayIdx].guests - 1,
                        members: oldStats[todayIdx].members,
                        storeOwners: oldStats[todayIdx].storeOwners,
                        storeManagers: oldStats[todayIdx].storeManagers + 1
                    }]));
                    break;
                case "STOREOWNER":
                    setStatistics(oldStats.slice(0, todayIdx).concat([{
                        id: oldStats[todayIdx].id,
                        date: oldStats[todayIdx].date,
                        marketManagers: oldStats[todayIdx].marketManagers,
                        guests: oldStats[todayIdx].guests - 1,
                        members: oldStats[todayIdx].members,
                        storeOwners: oldStats[todayIdx].storeOwners + 1,
                        storeManagers: oldStats[todayIdx].storeManagers
                    }]));
                    break;
                case "MARKETMANAGER":
                    setStatistics(oldStats.slice(0, todayIdx).concat([{
                        id: oldStats[todayIdx].id,
                        date: oldStats[todayIdx].date,
                        marketManagers: oldStats[todayIdx].marketManagers + 1,
                        guests: oldStats[todayIdx].guests,
                        members: oldStats[todayIdx].members,
                        storeOwners: oldStats[todayIdx].storeOwners,
                        storeManagers: oldStats[todayIdx].storeManagers
                    }]));
            }
        }
    }

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

    const handleOpenHistoryInput = () => {
        setOpen(false);
        setHistoryInputOpen(true);
    };

    const handleCloseHistoryInput = () => {
        setHistoryInputOpen(false);
        setOpen(true);
    };

    const handleGetStorePurchaseHist = (e: React.FormEvent<HTMLFormElement>) => {
        e.preventDefault();
        handleGetStorePurchaseHistory(token, storeId)
            .then(value => { 
                setOrders(value);
                setHistoryOpen(true);
            })
            .catch(error => {
                alert(error)
            });
        setStoreId(0);
        handleCloseHistory();
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

    const handleOpenStatsInput = () => {
        setOpen(false);
        setStatsInputOpen(true);
    };

    const handleCloseStatsInput = () => {
        setStatsInputOpen(false);
        setOpen(true);
    };

    const handleOpenStats = (e: React.FormEvent<HTMLFormElement>) => {
        e.preventDefault();
        setStatsInputOpen(false);
        viewStatistics();
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

    const handleGetDailyIncome = (e: React.MouseEvent<HTMLElement>) => {
        getDailyIncome(token)
            .then(data => {
                alert(`Daily market income is ${data}`);
            })
            .catch(error => {
                alert(error)
            });
    };

    const addStatsIds = (stats: APIStatistics[]): Statistics[] =>
        stats.map(
            (stat, idx) => ({ id: idx, date: stat.date, marketManagers: stat.marketManagers, storeOwners: stat.storeOwners, storeManagers: stat.storeManagers, members: stat.members, guests: stat.guests})
        )

    const viewStatistics = () => {
        handleViewStatistics(token, fromDate, toDate)
            .then(stats => {
                const statsWithIds = addStatsIds(stats);
                setStatistics(statsWithIds);
                setFromDate("");
                setToDate("");

                const url = `ws://127.0.0.1:8800/${token.membername}-live_view`;
                const conn = new WebSocket(url);
                conn.addEventListener("message", (ev: any) => updateStats(statsWithIds, ev.data))
            })
            .catch(error => {
                alert(error)
            });
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
                    <Button onClick={handleOpenHistoryInput}>View Store Purchase History</Button>
                    <Button onClick={handleOpenMemberInfo}>Members Information</Button>
                    <Button onClick={handleGetDailyIncome}>Daily Market Income</Button>
                    <Button onClick={handleOpenStatsInput}>Market Statistics</Button>
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

            <Dialog open={historyInputOpen} onClose={handleCloseHistoryInput}>
                <DialogTitle>Store Purchase History</DialogTitle>
                <DialogContent>
                    <DialogContentText>
                        Store Purchase History
                    </DialogContentText>
                    <form onSubmit={handleGetStorePurchaseHist} id="historyForm">
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
                    <Button onClick={handleCloseHistoryInput}>Back</Button>
                    <Button variant="contained" type="submit" form="historyForm">OK</Button>
                </DialogActions>
            </Dialog>

            <Dialog open={historyOpen} onClose={handleCloseHistory} fullScreen>
                <DialogTitle>Store Purchase History</DialogTitle>
                <DialogContent>
                    <DataGrid
                        rows={orders}
                        columns={orderColumns}
                    />
                </DialogContent>
                <DialogActions>
                    <Button onClick={handleCloseHistory}>Back</Button>
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
                    </TableContainer>
                    <br/>
                    <TableContainer component={Paper}>
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

            <Dialog open={statsInputOpen} onClose={handleCloseStatsInput}>
                <DialogTitle>Daily Statistics</DialogTitle>
                <DialogContent>
                    <DialogContentText>
                        Please insert start and end date
                    </DialogContentText>
                    <form onSubmit={handleOpenStats} id="statsForm" >
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
                    <Button onClick={handleCloseStatsInput}>Back</Button>
                    <Button variant="contained" type="submit" form="statsForm">OK</Button>
                </DialogActions>
            </Dialog>

            <Dialog open={statsOpen} onClose={handleCloseStats} fullScreen>
                <DialogTitle>Market Statistics</DialogTitle>
                <DialogContent>
                    <DataGrid 
                        rows={statistics !== null? statistics: []}
                        columns={statsColumns}
                    />
                </DialogContent>
                <DialogActions>
                    <Button onClick={handleCloseStats}>Back</Button>
                </DialogActions>
            </Dialog>

        </div>
    );
}
