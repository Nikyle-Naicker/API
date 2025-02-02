import axios from "axios";
import { CommentGet, CommentPost } from "../Models/Comment";
import { handleError } from "../Helpers/ErrorHandler";

const api = "https://delightful-island-0a0d63a1e.4.azurestaticapps.net/comment/";

export const commentPostAPI = async (title: string, content: string, symbol: string) => {
    try{
        const data = await axios.post<CommentPost>(api + `${symbol}`, {
            title: title,
            content: content
        })
        return data;
    }
    catch(error)
    {
        handleError(error);
    }
}

export const commentGetAPI = async (symbol: string) => {
    try{
        const data = await axios.get<CommentGet[]>(api + `?Symbol=${symbol}`)
        return data;
    }
    catch(error)
    {
        handleError(error);
    }
}