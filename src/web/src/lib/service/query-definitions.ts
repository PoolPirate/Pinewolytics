import type { OptimismAddressBalanceDTO, OptimismContractActivityDTO, OptimismContractMetricsDTO, OptimismDAppUsageDTO, OptimismGasMetricsDTO, OptimismL1SubmissionMetricsDTO, OptimismOPHolderMetricsDTO, OptimismTransactionMetricsDTO, OptimismWalletMetricsDTO } from "$lib/models/DTOs/OptimismDTO";
import type { OsmosisLPJoinDTO, OsmosisLPExitDTO, OsmosisIBCTransferDTO, OsmosisTransferDTO, OsmosisSwapDTO, OsmosisDelegateDTO, OsmosisUndelegateDTO, OsmosisStakingRewardDTO, OsmosisTotalDelegationsDTO, OsmosisProtoRevRevenueDTO } from "$lib/models/DTOs/OsmosisDTOs";
import type { TerraAddressBalanceDTO, TerraContractMetricsDTO, TerraTotalFeeDTO, TerraTransactionMetricsDTO, TerraValidatorCountDTO, TerraWalletMetricsDTO } from "$lib/models/DTOs/TerraDTOs";
import type { StringPrimitiveObject, TimeSeriesEntryDTO } from "$lib/models/SharedDTOs";

export enum QueryName {
	TerraValidatorCountHistory = 'terra-validator-count-history',
	TerraTransactionMetricsHistory = 'terra-transaction-metrics-history',
	TerraWalletMetricsHistory = 'terra-wallet-metrics-history',
	TerraContractMetricsHistory = 'terra-contract-metrics-history',
	TerraTotalFeesHistory = 'terra-total-fees-history',
    TerraRichList = "terra-richlist",

	OptimismRichList = "optimism-richlist",
	OptimismPriceHistory = "optimism-price-history",
	OptimismTransactionMetricsHistory = "optimism-transaction-metrics-history",
	OptimismContractMetricsHistory = 'optimism-contract-metrics-history',
	OptimismWalletMetricsHistory = 'optimism-wallet-metrics-history',
	OptimismL1SubmissionsHistory = "optimism-l1-submissions-history",
	OptimismGasMetricsHistory = "optimism-gas-metrics-history",
	OptimismContractActvityHistory = "optimism-contract-activity-history",
	OptimismDAppLeaderboard = "optimism-dapp-leaderboard",
	OptimismOPHolderMetricsHistory = "optimism-op-holder-metrics-history",

	OsmosisL0DevWallets = "osmosis-dev-wallet-0",
	OsmosisL5DevWallets = "osmosis-dev-wallet-5",
	OsmosisL5DevLPJoins = "osmosis-dev-wallet-5-lp-joins",
	OsmosisL5DevLPExits = "osmosis-dev-wallet-5-lp-exits",
	OsmosisL5DevIBCTransfers = "osmosis-dev-wallet-5-ibc-transfers",
	OsmosisL5DevTransfers = "osmosis-dev-wallet-5-transfers",
	OsmosisL5DevSwaps = "osmosis-dev-wallet-5-swaps",
	OsmosisL0DevTransfers = "osmosis-dev-wallet-0-transfers",
	OsmosisL5DevDelegations = "osmosis-dev-wallet-5-delegate",
	OsmosisL5DevUndelegations = "osmosis-dev-wallet-5-undelegate",
	OsmosisL5DevStakingRewards = "osmosis-dev-wallets-5-staking-rewards",
    OsmosisTotalDelegationsHistory = "osmosis-total-delegations-history",
	OsmosisL5DevTotalDelegationsHistory = "osmosis-dev-wallets-5-total-delegations-history",

	OsmosisProtoRevRevenueHistory = "osmosis-protorev-revenue-history"
}

export const queryTypes = {
	[QueryName.TerraValidatorCountHistory]: [] as TerraValidatorCountDTO[],
	[QueryName.TerraTransactionMetricsHistory]: [] as TerraTransactionMetricsDTO[],
	[QueryName.TerraWalletMetricsHistory]: [] as TerraWalletMetricsDTO[],
	[QueryName.TerraContractMetricsHistory]: [] as TerraContractMetricsDTO[],
	[QueryName.TerraTotalFeesHistory]: [] as TerraTotalFeeDTO[],
    [QueryName.TerraRichList]: [] as TerraAddressBalanceDTO[],

	[QueryName.OptimismTransactionMetricsHistory]: [] as OptimismTransactionMetricsDTO[],
	[QueryName.OptimismRichList]: [] as OptimismAddressBalanceDTO[],
	[QueryName.OptimismPriceHistory]: [] as TimeSeriesEntryDTO[],
	[QueryName.OptimismContractMetricsHistory]: [] as OptimismContractMetricsDTO[],
	[QueryName.OptimismWalletMetricsHistory]: [] as OptimismWalletMetricsDTO[],
	[QueryName.OptimismL1SubmissionsHistory]: [] as OptimismL1SubmissionMetricsDTO[],
	[QueryName.OptimismGasMetricsHistory]: [] as OptimismGasMetricsDTO[],
	[QueryName.OptimismContractActvityHistory]: [] as OptimismContractActivityDTO[],
	[QueryName.OptimismDAppLeaderboard]: [] as OptimismDAppUsageDTO[],
	[QueryName.OptimismOPHolderMetricsHistory]: [] as OptimismOPHolderMetricsDTO[],

	[QueryName.OsmosisL0DevWallets]: [] as StringPrimitiveObject[],
	[QueryName.OsmosisL5DevWallets]: [] as StringPrimitiveObject[],
	[QueryName.OsmosisL5DevLPJoins]: [] as OsmosisLPJoinDTO[],
	[QueryName.OsmosisL5DevLPExits]: [] as OsmosisLPExitDTO[],
	[QueryName.OsmosisL5DevIBCTransfers]: [] as OsmosisIBCTransferDTO[],
	[QueryName.OsmosisL5DevTransfers]: [] as OsmosisTransferDTO[],
	[QueryName.OsmosisL5DevSwaps]: [] as OsmosisSwapDTO[],
	[QueryName.OsmosisL0DevTransfers]: [] as OsmosisTransferDTO[],
	[QueryName.OsmosisL5DevDelegations]: [] as OsmosisDelegateDTO[],
	[QueryName.OsmosisL5DevUndelegations]: [] as OsmosisUndelegateDTO[],
	[QueryName.OsmosisL5DevStakingRewards]: [] as OsmosisStakingRewardDTO[],
    [QueryName.OsmosisTotalDelegationsHistory]: [] as TimeSeriesEntryDTO[],
	[QueryName.OsmosisL5DevTotalDelegationsHistory]: [] as OsmosisTotalDelegationsDTO[],
	[QueryName.OsmosisProtoRevRevenueHistory]: [] as OsmosisProtoRevRevenueDTO[]
};