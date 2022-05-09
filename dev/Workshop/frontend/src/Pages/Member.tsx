import React from 'react';
import { useNavigate } from "react-router-dom";
import './Member.css';


// TODO import Store from Store component instead of defining it here

type Store = {
    id: number;
    title: string;
}

type Stores = {
    stores: Store[],
    filteredStores: Store[]
}


function Member() {

    const [stores, setStores] = React.useState<Stores>({ stores: [], filteredStores: [] });
    const [filteredStore, setFilterStore] = React.useState<Stores>({ stores: [], filteredStores: [] });
    const addStores = (storeName: string) => {
        setStores({
            stores: [
                { title: storeName, id: stores.stores.length + 1 },
                ...stores.stores
            ],
            filteredStores: filteredStore.filteredStores
        });
    };

    const filterStores = (storeName: string) => {
        setFilterStore({
            stores: stores.stores,
            filteredStores:
                stores.stores.filter(({ title }) =>
                    title.toLowerCase().includes(storeName))
        });
    };
    const deleteStores = (id: number) => {
        setStores({
            stores: stores.stores.filter(t => t.id !== id),
            filteredStores: stores.stores.filter(t => t.id !== id)
        });
    };
    return (
        <div className="App">
            <AddStoresComponent addStores={addStores}/>
            <hr />
            <StoresComponent
                stores={stores}
                deleteStores={deleteStores} />
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
            <h2 className="stores-header">stores</h2>
            {stores.filteredStores.length ? <ul className="stores">
                {stores.filteredStores.map(store => (
                    <li key={store.id}>
                        <span>{store.title}</span>
                        <button
                            className="Member_Delete_Srore_Btn"
                            onClick={() => { deleteStore(store.id) }}>X
                        </button>
                        <button
                            className="Member_Open_Store_Btn"
                            onClick={routeChange('/Store')}>Open store
                        </button>
                    </li>
                ))}
            </ul> : <div style={{ color: 'white' }} >No store have been created</div>}
        </div>
    );

};

const AddStoresComponent = ({ addStores }: { addStores: (text: string) => void }, { filterStores }: { filterStores : (text: string) => void}) => {
    const [store, setStore] = React.useState<string>("");
    const [filteredStore, setFilterStore] = React.useState<string>("");
    const add = (e: React.MouseEvent<HTMLButtonElement, MouseEvent>) => {
        e.preventDefault();
        if (!store) {
            alert("Please enter a store name");
        } else {
            addStores(store);
            setStore("");
        }
    };
    const filterStore = (e: React.MouseEvent<HTMLButtonElement, MouseEvent>) => {
        e.preventDefault();
        filterStores(filteredStore);
        setFilterStore("");
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
                    onClick={filterStore}>Search store
                </button>
                <input className="Member_Store_textbox"
                    value={filteredStore}
                    onChange={e => { setFilterStore(e.target.value) }} />
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