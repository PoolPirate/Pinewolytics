export interface OsmosisTransferDTO {
    blockTimestamp: string;
    amount: number;
    receiver: string;
    sender: string;
}

export interface OsmosisNetTransferDTO {
    amount: number;
    receiver: string;
    sender: string;
}

export interface OsmosisSwapDTO {
    blockTimestamp: string;
    trader: string;
    fromAmount: number;
    fromCurrency: string;

    toAmount: number;
    toCurrency: string;
}

export interface OsmosisIBCTransferDTO {
    blockTimestamp: string;
    amount: number;

    receiver: string;
    sender: string;

    transferType: IBCDirection;
}

export interface OsmosisLPJoinDTO {
    blockTimestamp: string;
    liquidityProviderAddress: string;
    amount: number;
}

export enum IBCDirection {
    IBC_IN,
    IBC_OUT
}

export interface OsmosisFlowSankeyDTO {
    decimals: number,

    netSwapOut: number;
    netSwapIn: number;

    netLPDeposit: number;
    netLPExit: number;

    netIBCIn: number;
    netIBCOut: number;

    netTransferIn: number;
    netTransferOut: number;

    netStakingRewards: number;

    netStaked: number;
    netUnstaked: number;
}