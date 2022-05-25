import React, { useState } from 'react';
import { useNavigate, useLocation } from "react-router-dom";
import './AddDiscount.css';
import { StoreToken, token } from '../Components/roles';
import { Discount, makeCategoryDiscountSimpleTerm, makeCategoryPriceActionSimple, makeEmptyDiscount, makeProductPriceActionSimple, makeSimpleDiscount, SimpleDiscount } from '../Components/discount';
import { FormControl, FormControlLabel, FormLabel, Radio, RadioGroup } from '@mui/material';

function AddDiscount() {

    const textStyle = { color: 'white' }

    const location = useLocation();
    const token = location.state as StoreToken;

    let navigate = useNavigate();
    const routeChange = (path: string, token: token) =>
        () =>
            navigate(path, { state: token });

    // const [discount, setDiscount] = useState<Discount>(makeEmptyDiscount());
    //const [tempDiscount, setTempDiscount] = useState<Discount>(makeEmptyDiscount());
    const [category, setCategory] = useState<string>("");
    const [productId, setProductId] = useState<string>("");
    const [percentage, setPercentage] = useState<string>("");

    function handleAddDiscount(event: React.MouseEvent<HTMLButtonElement, MouseEvent>): Promise<any> {
        if ((productId === "" && category === "") || (productId !== "" && category !== "")) {
            alert('Must fill either category name or product ID');
            return Promise.reject();
        }

        let percentageNum = Number(percentage);
        if (percentageNum === NaN || percentageNum < 0 || percentageNum > 100) {
            alert('Discount Percentage must be a number between 0 and 100');
            return Promise.reject();
        }

        let simpleDiscount: SimpleDiscount =
            category ?
            makeSimpleDiscount(makeCategoryPriceActionSimple(percentageNum, category)) :
            makeSimpleDiscount(makeProductPriceActionSimple(percentageNum, Number(productId)));


        const baseUrl = "http://localhost:5165/api/discount";
        const endUrl = category ? "/category" : "/product";
        const url = baseUrl + endUrl;
        return fetch(url, {
            method: 'POST',
            mode: 'cors',
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({
                userId: token.userId,
                memberName: token.membername,
                storeId: token.storeId,
                jsonDiscount: simpleDiscount,
                category: category,
                productId: productId
            })
        }).then(async response => {
            const data = await response.json();
            if (!response.ok) {
                return Promise.reject(data.error);
            }
            return Promise.resolve(data);
        })

        /*
         * <FormControl>
                <FormLabel id="demo-radio-buttons-group-label">Gender</FormLabel>
                <RadioGroup
                    aria-labelledby="demo-radio-buttons-group-label"
                    defaultValue="female"
                    name="radio-buttons-group"
                >
                    <FormControlLabel value="female" control={<Radio />} label="Female" />
                    <FormControlLabel value="male" control={<Radio />} label="Male" />
                    <FormControlLabel value="other" control={<Radio />} label="Other" />
                </RadioGroup>
            </FormControl>
         */ 
    }

    //<button className="add_discount_back_btn" onClick={routeChange('/member', { userId: token.userId })}> Back to member page </button> 

    return (
        <div className="add_discount">
            <div className="add_discount_title" style={textStyle}> Add Discount </div>
            <p className="login_userInput">
                <div style={textStyle}> Category Name: </div>
                <input className="add_discount_category_textbox" type="text" onChange={e => setCategory(e.target.value)} />
                <div style={textStyle}> Product ID: </div>
                <input className="add_discount_product_textbox" type="number" onChange={e => setProductId(e.target.value)} />
                <div style={textStyle}> Discount %: </div>
                <input className="add_discount_percentage" type="number" onChange={e => setPercentage(e.target.value)} />
            </p>
            <p className="add_discount_buttons">
                <button className="add_discount_ok_btn" type="submit"
                    onClick={e =>
                        handleAddDiscount(e)
                            .then(routeChange("/member", { userId: token.userId, membername: token.membername }))
                            .catch((err) => alert(err))
                    }> OK </button>
            </p>
        </div>
    )
}


export default AddDiscount;