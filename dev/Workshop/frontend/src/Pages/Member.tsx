import React from 'react';
import { useNavigate } from "react-router-dom";
import './Member.css';
import { Store , Stores} from "../Components/store"


// TODO import Store from Store component instead of defining it here



function Member() {

    let navigate = useNavigate();
    const routeChange = (path: string) =>
        () => {
            navigate(path);
        } 

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
    return (
        <div className="member_page">
            <div className="member_page_body">
                <AddStoresComponent addStores={addStores} />
                <hr />
                <StoresComponent
                    stores={stores}
                    deleteStores={deleteStores} />
                <p className="member_control_btns">
                    <button className="member_logout_btn" onClick={routeChange('/guest')}> Logout </button>
                    <button className="member_exit_btn" onClick={routeChange('/')}> Exit Market </button>
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

const AddStoresComponent = ({ addStores}: { addStores : (text: string) => void}) => {
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

export default Member;