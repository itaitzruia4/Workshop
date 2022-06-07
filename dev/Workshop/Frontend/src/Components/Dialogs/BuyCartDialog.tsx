import * as React from 'react';
import Box from '@mui/material/Box';
import TextField from '@mui/material/TextField';
import Dialog from '@mui/material/Dialog';
import Button from '@mui/material/Button';
import DialogActions from '@mui/material/DialogActions';
import DialogContent from '@mui/material/DialogContent';
import DialogContentText from '@mui/material/DialogContentText';
import DialogTitle from '@mui/material/DialogTitle';

type inputField = {
    value: string
    setValue: React.Dispatch<React.SetStateAction<string>>
    label: string
}
const makeInputField = (value: string, setValue: React.Dispatch<React.SetStateAction<string>> , label: string) : inputField => {
    return ({ value: value, setValue: setValue, label: label });
}
export default function BuyCartDialog(
    buyCart: (number: string, year: string, month: string, ccv: string, holder: string, id: string, name: string, address: string,
        city: string, country: string, zip: string) => void
) {

    const [open, setOpen] = React.useState(false);
    const [number, setNumber] = React.useState("");
    const [year, setYear] = React.useState("");
    const [month, setMonth] = React.useState("");
    const [ccv, setCcv] = React.useState("");
    const [holder, setHolder] = React.useState("");
    const [id, setId] = React.useState("");
    const [name, setName] = React.useState("");
    const [address, setAddress] = React.useState("");
    const [city, setCity] = React.useState("");
    const [country, setCountry] = React.useState("");
    const [zip, setZip] = React.useState("");

    const inputs: inputField[] =
        [makeInputField(number, setNumber, "number"), makeInputField(year, setYear, "year"),
        makeInputField(month, setMonth, "month"),
        makeInputField(ccv, setCcv, "ccv"), makeInputField(holder, setHolder, "holder"),
        makeInputField(id, setId, "id"), makeInputField(name, setName, "name"),
        makeInputField(address, setAddress, "address"),
        makeInputField(city, setCity, "city"), makeInputField(country, setCountry, "country"),
        makeInputField(zip, setZip, "zip")]

    const handleClickOpen = () => {
        setOpen(true);
    };

    const handleClose = () => {
        setOpen(false);
    };

    const handleBuy = (e: React.FormEvent<HTMLFormElement>) => {
        e.preventDefault();
        buyCart(number,year,month,ccv,holder,id,name,address,city,country,zip)
        handleClose();
    };


    return (
        <div>
            <Button variant="contained" color="success" onClick={handleClickOpen}>
                Buy cart
            </Button>
        <Dialog open={open} onClose={handleClose}>
                <DialogTitle>Buy cart</DialogTitle>
                <DialogContent>
                    <DialogContentText>
                        Please input your info.
                    </DialogContentText>
                    <form onSubmit={handleBuy} id="buyForm" >
                        {inputs.map(field => {
                            return (
                                <TextField
                                    value={field.value}
                                    autoFocus
                                    margin="dense"
                                    label={field.label}
                                    fullWidth
                                    variant="standard"
                                    onChange={(e) => field.setValue(e.target.value)}
                                />)
                        })}
                    </form>
                </DialogContent>
                <DialogActions>
                    <Button onClick={handleClose}>Cancel</Button>
                    <Button variant="contained" color="success" type="submit" form="buyForm">Buy</Button>
                </DialogActions>
        </Dialog>
    </div>
    );
}