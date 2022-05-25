import * as React from 'react';
import ReactDOM from 'react-dom';
import Button from '@mui/material/Button';
import ButtonGroup from '@mui/material/ButtonGroup';
import Appbar from './Appbar';
import StoresList from '../Components/storesList'
import { Stack } from '@mui/material';
import { ThemeProvider, createTheme } from '@mui/material/styles';

import AddStoreDialog from '../Components/Dialogs/addStoreDialog';

import { useState } from 'react';


function Demo() {

    const darkTheme = createTheme({
        palette: {
            mode: 'dark',
        },
    });
    const [stores, setStores] = useState(["snacks","candy","meds"])
    return (
        <ThemeProvider theme={darkTheme}>
            <Appbar />
            <ButtonGroup variant="outlined" aria-label="outlined button group">
                {AddStoreDialog(stores, setStores)}
            </ButtonGroup>
            {StoresList(stores)}
            <Stack direction="row" spacing={2}>
                <Button variant='contained'>Logout </Button>
                <Button variant='contained'>Exit market </Button>
            </Stack>
        </ThemeProvider>
    );

}

export default Demo;