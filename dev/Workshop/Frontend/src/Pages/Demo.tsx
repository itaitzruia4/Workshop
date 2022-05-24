import * as React from 'react';
import ReactDOM from 'react-dom';
import Button from '@mui/material/Button';
import ButtonGroup from '@mui/material/ButtonGroup';
import Appbar from './Appbar';
import StoresList from '../Components/storesList'
import { Stack } from '@mui/material';
import { ThemeProvider, createTheme } from '@mui/material/styles';


function Demo() {

    const darkTheme = createTheme({
        palette: {
            mode: 'dark',
        },
    });

    return (
        <ThemeProvider theme={darkTheme}>
        <p>
            <Appbar /> 
            <ButtonGroup variant="outlined" aria-label="outlined button group">
                <Button>Add store</Button>
                <Button>Two</Button>
                <Button>Three</Button>
            </ButtonGroup>
            {StoresList("test")}
            <Stack direction="row" spacing={2}>
                <Button variant='contained'>Logout </Button>
                <Button variant='contained'>Exit market </Button>
            </Stack>
            </p>
        </ThemeProvider>
    );

}

export default Demo;