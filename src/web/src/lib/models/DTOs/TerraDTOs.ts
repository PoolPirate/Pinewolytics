export interface TerraValidatorCountDTO {
    timestamp: string;
    count: number;
}

export interface TerraTransactionMetricsDTO {
    timestamp :string;
    minimumFee: number;
    averageFee: number;
    medianFee: number;
    maximumFee: number;
    totalFee: number;

    transactionCount: number;
    blockCount: number;
}

export interface TerraWalletMetricsDTO {
    timestamp :string;

    totalReceivers: number;
    totalSenders: number;
    totalSendersAndReceivers: number;
}