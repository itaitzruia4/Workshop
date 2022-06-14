import * as React from 'react';
import { Menu, MenuItem } from "@mui/material"
import { MarketNotification } from "../Types/Notification"

export function NotificationsList(notifications: MarketNotification[], open: boolean, handleClose: () => void, anchor: any) {

    return (
        <Menu anchorEl={ anchor } open={open} onClose={handleClose}>
            {notifications.map((notification: MarketNotification) => (
                <MenuItem>
                    {`${notification.message}`}
                </MenuItem>
            ))}
        </Menu>
    )
}