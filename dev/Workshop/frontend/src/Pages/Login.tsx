import React, { useState } from 'react'
import { useNavigate, useLocation } from "react-router-dom";
import { Grid, Paper, Avatar, TextField, Button, Typography, Link } from '@mui/material/';
import LockOutlinedIcon from '@mui/icons-material/LockOutlined';
import { userToken, token, makeMemberToken, makeUserToken } from '../Types/roles';
import { handleLogin } from '../Actions/AuthenticationActions';
import { MarketNotification } from '../Types/Notification';

const Login = () => {

    const location = useLocation();
    const token = location.state as userToken;

    let navigate = useNavigate();
    const routeChange = (path: string, token: token) =>
        () =>
            navigate(path, { state: token });

    const [membername, setMembername] = useState("");
    const [password, setPassword] = useState("");

    const paperStyle = { padding: 20, height: '70vh', width: 280, margin: "20px auto" }
    const avatarStyle = { backgroundColor: 'lightblue' }
    const btnstyle = { margin: '8px 0' }
    return (
        <Grid>
            <Paper elevation={10} style={paperStyle}>
                <Grid>
                    <Avatar style={avatarStyle}><LockOutlinedIcon /></Avatar>
                    <h2>Sign In</h2>
                </Grid>
                <TextField label='Username' placeholder='Enter username' fullWidth required onChange={e => setMembername(e.target.value)} />
                <TextField label='Password' placeholder='Enter password' type='password' fullWidth required onChange={e => setPassword(e.target.value)} />
                <Button type='submit' color='primary' variant="contained" style={btnstyle} fullWidth
                onClick={e =>
                    handleLogin(token, membername, password)
                        .then((data) => routeChange("/member", makeMemberToken(token.userId, membername, data.notifications))())
                        .catch(error => {
                            alert(error)
                        })
                } >Sign in</Button>
                <Button type='submit' color='primary' variant="contained" style={btnstyle}
                    onClick={routeChange("/guest", token)} fullWidth >Continue as guest</Button>
                <Typography > Don't have an account ? 
                    <Link
                        onClick={routeChange("/register", token)}>
                        Sign Up
                    </Link>
                </Typography>
            </Paper>
        </Grid>
    )
}

export default Login