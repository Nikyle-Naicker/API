export type PortfolioGet = {
    id: string;
    symbol: string;
    companyName: string;
    purchase: number;
    lastDiv: number;
    industry: string;
    marketCap: number;
    comments: any;
}

export type PortfolioPost = {
    symbol: string;
}