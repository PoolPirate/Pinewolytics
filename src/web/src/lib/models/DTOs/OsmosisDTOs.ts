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
	decimals: number;

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

export interface OsmosisWalletPoolRankingDTO {
	poolId: number;
	lpTokenBalance: number;
	rank: number;
}

export interface OsmosisWalletRankingDTO {
	address: string;
	lastUpdatedAt: string;
	stakedAmount: number;
	stakedRank: number;
	balanceAmount: number;
	balanceRank: number;

	poolRankings: OsmosisWalletPoolRankingDTO[];
}

export interface OsmosisPoolInfoDTO {
	poolId: number;
	assets: string[];
}

export interface OsmosisEpochInfoDTO {
	identifier: string;
	startTime: Date;
	duration: string;
	currentEpoch: number;
	currentEpochStartTime: Date;
	epochCountingStarted: boolean;
	currentEpochStartHeight: number;
}