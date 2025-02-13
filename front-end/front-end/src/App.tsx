import { Outlet } from 'react-router';
import './App.css';
import Navbar from './Components/Navbar/Navbar';
import "react-toastify/dist/ReactToastify.css"
import { ToastContainer } from 'react-toastify';
import { UserProvider } from './Context/UseAuth';


function App() {
  


  return (
    <div className="App">
      <UserProvider>
      <Navbar/>
      <Outlet/>
      <ToastContainer/>
      </UserProvider>
    </div>
  );
}

export default App;
