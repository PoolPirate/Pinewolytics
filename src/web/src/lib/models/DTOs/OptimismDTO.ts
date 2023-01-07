export interface OptimismAddressBalanceDTO {
    address: string;

    balance: number;
}

export interface OptimismTransactionMetricsDTO {
    timestamp: string;

    transactionCount: number;
    blockCount: number;
}

export interface OptimismContractMetricsDTO {
    timestamp: string;

    newContracts: number;
    totalContracts: number;

    activeDevelopers: number;
    totalDevelopers: number;
}

export interface OptimismWalletMetricsDTO {
    timestamp: string;

    totalReceivers: number;
    totalSenders: number;
    totalSendersAndReceivers: number;

    senders: number;
    receivers: number;
}

export interface OptimismL1SubmissionMetricsDTO {
    timestamp: string;

    totalSubmissions: number;
}

export interface OptimismGasMetricsDTO {
    timestamp: string;

    totalL2GasFee: number;
    totalL1GasFee: number;
}