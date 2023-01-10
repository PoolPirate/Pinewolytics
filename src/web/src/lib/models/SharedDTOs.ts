export interface TimeSeriesEntryDTO {
    timestamp: string;
    value: number;
}

export interface TimeSeriesEntryDTO2 {
    timestamp: string;
    value1: number;
    value2: number;
}

export interface MarketDataDTO {
    price: number;
    marketCap: number;

    totalSupply: number;
    circulatingSupply: number;    
}