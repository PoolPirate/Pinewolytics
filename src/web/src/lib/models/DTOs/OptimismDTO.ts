export interface OptimismAddressBalanceDTO {
    address: string;

    balance: number;
}

export interface OptimismTransactionMetricsDTO {
    timestamp: string;

    transactionCount: number;
    blockCount: number;
}