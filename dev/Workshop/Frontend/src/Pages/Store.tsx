import React from 'react';
import { useNavigate } from "react-router-dom";
import './Store.css';
import { Product, Products } from "../Components/product"

function Store() {
    const textStyle = { color: 'white' }

    let navigate = useNavigate();
    const routeChange = (path: string) =>
        () => {
            navigate(path);
        }

    return (
        <div style={{ color: 'white'}}>Hi!</div>
    )
}

const ProductsComponent: React.FC<{
    products: Products
}>
    return (
        <div className="section__store">
            <h2 className="stores-header">Stores</h2>
            {stores.stores.length ? <ul className="stores">
                {stores.stores.map(store => (
                    <li key={store.id}>
                        <span>{store.title}</span>
                        <button
                            className="Member_Open_Store_Btn"
                            onClick={routeChange('/Store')}>Open store
                        </button>
                        <button
                            className="Member_Delete_Srore_Btn"
                            onClick={() => { deleteStore(store.id) }}>X
                        </button>
                    </li>
                ))}
            </ul> : <div style={{ color: 'white' }} >No store have been found</div>}
        </div>
    );

};



const StoresConfigComponent = ({ addStores }: { addStores: (text: string) => void }) => {
    const [store, setStore] = React.useState<string>("");
    const add = (e: React.MouseEvent<HTMLButtonElement, MouseEvent>) => {
        e.preventDefault();
        if (!store) {
            alert("Please enter a store name");
        } else {
            addStores(store);
            setStore("");
        }
    };
    return (
        <div className="AddStore">
            <form className="Member_Store_Form">
                <button
                    className="Member_Store_Btn"
                    onClick={add}>Add store
                </button>
                <input className="Member_Store_textbox"
                    value={store}
                    onChange={e => { setStore(e.target.value) }} />
            </form>
            <form className="Member_Store_Form">
                <button
                    className="Member_Store_Btn"
                    onClick={add}>Search store
                </button>
                <input className="Member_Store_textbox"
                    value={store}
                    onChange={e => { setStore(e.target.value) }} />
            </form>
            <form className="Member_Store_Form">
                <button
                    className="Member_Store_Btn"
                    onClick={add}>Search product
                </button>
                <input className="Member_Store_textbox"
                    value={store}
                    onChange={e => { setStore(e.target.value) }} />
            </form>
        </div>
    );
};


export default Store;