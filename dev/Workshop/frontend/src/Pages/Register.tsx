import React, { useState } from 'react'
import * as moment from 'moment';
import { useNavigate, useLocation } from "react-router-dom";
import { Grid, Paper, Avatar, TextField, Button, Typography, Link } from '@mui/material/';
import { userToken, token } from '../Types/roles';
import { handleRegister } from '../Actions/AuthenticationActions';
import AssignmentIndIcon from '@mui/icons-material/AssignmentInd';

const Register = () => {

    const location = useLocation();
    const token = location.state as userToken;

    let navigate = useNavigate();
    const routeChange = (path: string, token: token) =>
        () =>
            navigate(path, { state: token });

    const [membername, setMembername] = useState("");
    const [password, setPassword] = useState("");
    const [birthDate, setBirthDate] = useState("");   

    const paperStyle = { padding: 20, height: '70vh', width: 280, margin: "20px auto" }
    const avatarStyle = { backgroundColor: 'lightblue' }
    const btnstyle = { margin: '8px 0' }
    return (
        <Grid>
            <Paper elevation={10} style={paperStyle}>
                <Grid>
                    <Avatar style={avatarStyle}><AssignmentIndIcon /></Avatar>
                    <h2>Sign Up</h2>
                </Grid>
                <TextField label='Username' placeholder='Enter username' fullWidth required onChange={e => setMembername(e.target.value)} />
                <TextField label='Password' placeholder='Enter password' type='password' fullWidth required onChange={e => setPassword(e.target.value)} />
                <TextField label='Birth Date' placeholder='DD/MM/YYYY' fullWidth required onChange={e => setBirthDate(e.target.value)} />
                <Button type='submit' color='primary' variant="contained" style={btnstyle} fullWidth
                    onClick={e =>
                        handleRegister(token, membername, password, birthDate)
                            .then(routeChange("/login", { userId: token.userId, membername: membername }))
                            .catch(error => {
                                alert(error)
                            })
                    } >Sign up</Button>
                <Button type='submit' color='primary' variant="contained" style={btnstyle} fullWidth onClick={routeChange('/login', { userId: token.userId })}
                >Back to login screen</Button>
            </Paper>
        </Grid>
    )
}

export default Register