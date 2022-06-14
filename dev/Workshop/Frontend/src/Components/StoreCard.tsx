import * as React from 'react';
import Box from '@mui/material/Box';
import Card from '@mui/material/Card';
import CardActions from '@mui/material/CardActions';
import CardContent from '@mui/material/CardContent';
import Button from '@mui/material/Button';
import Typography from '@mui/material/Typography';


import { memberToken, Actions, StorePermission } from '../Types/roles';
import { Store } from "../Types/store"
import { Product } from "../Types/product"
import { Cart, Bag } from '../Types/shopping';
import { MarketNotification } from '../Types/Notification';


export default function BasicCard(store: Store, permissions: Actions[]) {
    return (
        <Card sx={{
            minWidth: 275, backgroundColor: '#e3f2fd' }}>
            <CardContent>
                <Typography sx={{ fontSize: 14 }} color="text.secondary" gutterBottom>
                    {store.name }
                </Typography>
                <Typography sx={{ mb: 1.5 }} color="text.secondary">
                    {permissions}
                </Typography>
            </CardContent>
            <CardActions>
                <Button size="small">Learn More</Button>
            </CardActions>
        </Card>
    );
}
