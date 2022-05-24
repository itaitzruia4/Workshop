import React from 'react';
import { useNavigate, useLocation } from "react-router-dom";
import './Member.css';
import { Store, Stores } from "../Components/store"
import { Product } from "../Components/product"
import { memeberToken} from '../Components/roles';
import { handleLogout, handleExitMarket } from '../Actions/AuthenticationActions';
import { handleGetStores, handleNewStore } from '../Actions/StoreActions';


// TODO import Store from Store component instead of defining it here

type Products = {
    products: Product[],
}

function Member() {

    const textStyle = { color: 'white' }

    const location = useLocation();
    const token = location.state as memeberToken;

    let navigate = useNavigate();
    const routeChange = (path: string, token: memeberToken) =>
        () =>
            navigate(path, { state: token });

    const [stores, setStores] = React.useState<Stores>({ stores: [] });
    const refreshStores = () => {
        handleGetStores(token).then(value => setStores({ stores: value as Store[] })).catch(error => alert(error));
    };
    refreshStores();
    const addStores = (storeName: string) => {
        handleNewStore(token, storeName).then(() => handleGetStores(token).then(value => setStores({ stores: value as Store[] }))).catch(error => alert(error));
    };

    const deleteStores = (id: number) => {
        setStores({
            stores: stores.stores.filter(t => t.storeId !== id),
        });
    };

    const [products, setProducts] = React.useState<Products>({ products: [{ tag: 'Product', id: 1, name: "candy", basePrice: 1000, description: "cool drug", quantity: 3 }] });
    const deleteProducts = (id: number) => {
        setProducts({
            products: products.products.filter(t => t.id !== id),
        });
    };

    return (
        <div className="member_page" style={textStyle}> {"Welcome " + token.membername + "!"} 
            <div className="member_page_body" >
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
                    <button className="Member_Btn" onClick={e =>
                        handleLogout(token)
                            .then(routeChange("/home", token))
                            .catch(error => {
                                alert(error)
                            })
                    } > Logout </button>
                    <button className="Member_Btn" onClick={e =>
                        handleExitMarket(token)
                            .then(routeChange("/", token))
                            .catch(error => {
                                alert(error)
                            })
                    } > Exit Market </button>
                </p>
            </div>
        </div>
    );
}


const StoresComponent: React.FC<{
    stores: Stores,
    deleteStores: (id: number) => void
}> = ({ stores, deleteStores }) => {

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
                    <li key={store.storeId}>
                        <button
                            className="Member_Store_Btn"
                            onClick={routeChange('/Store')}>{store.name}
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
                    className="Member_Btn"
                    onClick={add}>Add store
                </button>
                <input className="Member_Store_textbox"
                    value={store}
                    onChange={e => { setStore(e.target.value) }} />
            </form>
            <form className="Member_Store_Form">
                <button
                    className="Member_Btn"
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