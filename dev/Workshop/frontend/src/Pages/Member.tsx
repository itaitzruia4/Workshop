import React from 'react';
import { useNavigate, useLocation } from "react-router-dom";
import './Member.css';
import { Store, Stores } from "../Components/store"
import { Product } from "../Components/product"
import { memeberToken, userToken } from '../Components/roles';


// TODO import Store from Store component instead of defining it here

type Products = {
    products: Product[],
}

function Member() {
    const location = useLocation();
    const token = location.state as memeberToken;

    let navigate = useNavigate();
    const routeChange = (path: string, userId: number) =>
        () =>
            navigate(path, { state: { userId: userId } });

    const [stores, setStores] = React.useState<Stores>({ stores: []});
    const addStores = (storeName: string) => {
        setStores({
            stores: [
                { title: storeName, id: stores.stores.length + 1 },
                ...stores.stores
            ]
        });
    };

    const deleteStores = (id: number) => {
        setStores({
            stores: stores.stores.filter(t => t.id !== id),
        });
    };

    const [products, setProducts] = React.useState<Products>({ products: [{ tag: 'Product', id: 1, name: "candy", basePrice: 1000, description: "cool drug", quantity: 3 }] });
    const deleteProducts = (id: number) => {
        setProducts({
            products: products.products.filter(t => t.id !== id),
        });
    };

    function HandleLogout() {

        let url = "http://localhost:5165/api/authentication/logout";

        fetch(url, {
            method: 'POST',
            mode: 'cors',
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({
                userId: token.userId,
                membername: token.membername,
            })
        }).then((response) =>
            response.json()
                .then((data) => response.ok ? null :
                    alert(data.error))
                .then(routeChange("/home", token.userId))
        );
    }

    return (
        <div className="member_page">
            <div className="member_page_body">
                <ConfigStoresComponent addStores={addStores} />
                <hr />
                <div className="lists_section">
                    <StoresComponent
                        stores={stores}
                        deleteStores={deleteStores} />
                    <CartComponent
                        products={products}
                        deleteProducts={deleteProducts} />
                </div>
                <p className="member_control_btns">
                    <button className="member_logout_btn" onClick={HandleLogout}> Logout </button>
                    <button className="member_exit_btn" onClick={routeChange('/', token.userId)}> Exit Market </button>
                </p>
            </div>
        </div>
    );
}


const StoresComponent: React.FC<{
    stores: Stores,
    deleteStores: (id: number) => void
}> = ({ stores, deleteStores }) => {
    const deleteStore = (id: number) => {
        if (window.confirm(`Are you sure you want to delete this store?`)) {
            deleteStores(id);
        }
    }

    let navigate = useNavigate();
    const routeChange = (path: string) =>
        () => {
            navigate(path);
        } 

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

const ConfigStoresComponent = ({ addStores}: { addStores : (text: string) => void}) => {
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
        <div className="Config_Store_Btns">
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
                    onClick={add}>Search product
                </button>
                <input className="Member_Store_textbox"
                    value={store}
                    onChange={e => { setStore(e.target.value) }} />
            </form>
        </div>
    );
};


const CartComponent: React.FC<{
    products: Products,
    deleteProducts: (id: number) => void
}> = ({ products, deleteProducts }) => {
    const deleteProduct = (id: number) => {
        if (window.confirm(`Are you sure you want to remove this product from your shopping cart?`)) {
            deleteProducts(id);
        }
    }

    return (
        <div className="section__store">
            <h2 className="stores-header">Shopping cart</h2>
            {products.products.length ? <ul className="products">
                {products.products.map(product => (
                    <li key={product.id}>
                        <span>{product.name}</span>
                        <button
                            className="Member_Delete_Srore_Btn"
                            onClick={() => { deleteProduct(product.id) }}>X
                        </button>
                    </li>
                ))}
            </ul> : <div style={{ color: 'white' }} >Your shopping cart is empty</div>}
            <button
                className="Member_Buy_Btn"
                >Buy cart
            </button>
        </div>
    );

};


export default Member;