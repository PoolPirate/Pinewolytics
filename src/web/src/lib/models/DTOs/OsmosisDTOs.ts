export interface OsmosisDenominatedAmountDTO {
	denom: string;
	amount: number;
}

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

export interface OsmosisStakingRewardDTO {
	date: string;
	address: string;
	amount: number;
}

export interface OsmosisLPExitDTO {
	blockTimestamp: string;
	liquidityProviderAddress: string;
	amount: number;
}

export interface OsmosisDelegateDTO {
	blockTimestamp: string;
	address: string;
	amount: number;
}

export interface OsmosisUndelegateDTO {
	blockTimestamp: string;
	address: string;
	amount: number;
}

export interface OsmosisTotalDelegationsDTO {
	date: string;
	totalDelegated: number;
	totalUndelegated: number;
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
	startTime: string;
	duration: string;
	currentEpoch: number;
	currentEpochStartTime: string;
	epochCountingStarted: boolean;
	currentEpochStartHeight: number;
}

export interface OsmosisProtoRevRevenueDTO {
	date: string;
	currency: string;
	symbol: string;
	totalAmount: number;
	totalAmountUSD: number;
}

export interface OsmosisProtoRevRouteStatisticsDTO {
	profits: OsmosisDenominatedAmountDTO[],
	numberOfTrades: number;
	route: string[];
}

export interface OsmosisTokenInfoDTO {
	denom: string;
	symbol: string;
	price: number;
	exponent: number;
}

export interface OsmosisProtoRevTransactionDTO {
	hash: string;
	timestamp: string;
	txFrom: string;
	swaps: OsmosisProtoRevSwapDTO[]
}

export interface OsomsisProtoRevAssetRevenueDTO {
	currency: string;
	totalAsset: number;
	totalUSD: number;
}

export interface OsmosisProtoRevSwapDTO {
	profit: OsmosisDenominatedAmountDTO;
	profitUSD: number;
}