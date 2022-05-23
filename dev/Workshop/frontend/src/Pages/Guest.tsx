import React from 'react';
import { useNavigate, useLocation } from 'react-router-dom';
import './Guest.css';
import { Store, Stores } from "../Components/store"
import { Product } from "../Components/product"
import { token } from '../Components/roles';
import { handleExitMarket } from '../Actions/AuthenticationActions';



type Products = {
    products: Product[],
}

function Guest() {
    const location = useLocation();
    const token = location.state as token;

    let navigate = useNavigate();
    const routeChange = (path: string, token: token) =>
        () =>
            navigate(path, { state: token });

    const [stores, setStores] = React.useState<Stores>({ stores: [] });

    const [products, setProducts] = React.useState<Products>({ products: [{ tag: 'Product', id: 1, name: "candy", basePrice: 1000, description: "cool drug", quantity: 3 }] });
    const deleteProducts = (id: number) => {
        setProducts({
            products: products.products.filter(t => t.id !== id),
        });
    };
    return (
        <div className="guest_page">
            <div className="guest_page_body">
                <ConfigStoresComponent/>
                <hr />
                <div className="lists_section">
                    <StoresComponent
                        stores={stores}/>
                    <CartComponent
                        products={products}
                        deleteProducts={deleteProducts} />
                </div>
                <p className="guest_control_btns">
                    <button className="guest_exit_btn" onClick={e =>
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
}> = ({ stores }) => {

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
                        <span>{store.name}</span>
                        <button
                            className="guest_Open_Store_Btn"
                            onClick={routeChange('/Store')}>Open store
                        </button>
                    </li>
                ))}
            </ul> : <div style={{ color: 'white' }} >No store have been found</div>}
        </div>
    );

};

const ConfigStoresComponent = () => {
    const [store, setStore] = React.useState<string>("");
    return (
        <div className="Config_Store_Btns">
            <form className="guest_Store_Form">
                <button
                    className="guest_Store_Btn"
                   >Search product
                </button>
                <input className="guest_Store_textbox"
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
                            className="guest_Delete_Srore_Btn"
                            onClick={() => { deleteProduct(product.id) }}>X
                        </button>
                    </li>
                ))}
            </ul> : <div style={{ color: 'white' }} >Your shopping cart is empty</div>}
            <button
                className="guest_Buy_Btn"
            >Buy cart
            </button>
        </div>
    );

};


export default Guest;