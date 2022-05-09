import React from 'react';
import { useNavigate } from "react-router-dom";
import './Member.css';

type IStore = {
    id: number;
    title: string;
}

type IStores = {
    stores: IStore[],
}


function Member() {
 
    const [stores, setStores] = React.useState<IStores>({ stores: [] });
    const addStores = (title: string) => {
        setStores({
            stores: [
                { title, id: stores.stores.length + 1 },
                ...stores.stores
            ]
        });
    };
    const deleteStores = (id: number) => {
        setStores({
            stores: stores.stores.filter(t => t.id !== id)
        });
    };
    const toggleStores = (id: number) => {
        setStores({
            stores: stores.stores.map(stores => stores.id === id ? { ...stores } : stores)
        });
    }

    return (
        <div className="App">
            <AddStoresComponent addStores={addStores} />
            <hr />
            <StoresComponent
                stores={stores}
                toggleStores={toggleStores}
                deleteStores={deleteStores} />
        </div>
    );
}


const StoresComponent: React.FC<{
    stores: IStores,
    toggleStores: (id: number) => void,
    deleteStores: (id: number) => void
}> = ({ stores, toggleStores, deleteStores }) => {
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
            {stores.stores.length ? <ul className="stores">
                {stores.stores.map(store => (
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

const AddStoresComponent = ({ addStores }: { addStores: (text: string) => void }) => {
    const [store, setStore] = React.useState<string>("");
    const submit = (e: React.MouseEvent<HTMLButtonElement, MouseEvent>) => {
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
            <form>
                <button
                    className="Member_Add_Store_Btn"
                    onClick={submit}>Add
                </button>
                <input
                    value={store}
                    onChange={e => { setStore(e.target.value) }} />
            </form>
        </div>
    );
};

export default Member;