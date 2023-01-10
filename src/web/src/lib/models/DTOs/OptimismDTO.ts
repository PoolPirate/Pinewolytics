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

    dailySenders: number;
    dailyReceivers: number;

    weeklySenders: number;
    weeklyReceivers: number;
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

export interface OptimismContractActivityDTO {
    timestamp: string;

    dailyCalledContracts: number;
    weeklyCalledContracts: number;

    dailyUsedContracts: number;
    weeklyUsedContracts: number;
}

export interface OptimismDAppUsageDTO {
    projectName: string;
    
    txCount: number;
    addressCount: number;
}

export interface OptimismOPHolderMetricsDTO {
    timestamp: string;

    holderCount: number;

    medianBalance: number;
    averageBalance: number;
    minimumBalance: number;
    maximumBalance: number;
    percentile20thBalance: number;
    percentile80thBalance: number;
}