import { Outlet } from 'react-router';
import './App.css';
import Navbar from './Components/Navbar/Navbar';
import "react-toastify/dist/ReactToastify.css"
import { ToastContainer } from 'react-toastify';

function App() {
  

  return (
    <div className="App">
      <Navbar/>
      <Outlet/>
      <ToastContainer/>
    </div>
  );
}

export default App;
