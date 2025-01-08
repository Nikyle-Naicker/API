import axios from "axios";
import { PortfolioGet, PortfolioPost } from "../Models/Portfolio";
import { handleError } from "../Helpers/ErrorHandler";

const api = "http://localhost:5169/api/portfolio/";

export const portfolioAddAPI = async (symbol: string) => {
    try{
        const data = axios.post<PortfolioPost>(api + `?symbol=${symbol}`)
        return data;
    }
    catch(error)
    {
        handleError(error);
    }
};

export const portfolioDeleteAPI = async (symbol: string) => {
    try{
        const data = axios.delete<PortfolioPost>(api + `?symbol=${symbol}`)
        return data;
    }
    catch(error)
    {
        handleError(error);
    }
};

export const portfolioGetAPI = async () => {
    try{
        const data = axios.get<PortfolioGet[]>(api)
        return data;
    }
    catch(error)
    {
        handleError(error);
    }
}