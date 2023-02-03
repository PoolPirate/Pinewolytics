import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import type {
    TerraAddressBalanceDTO,
	TerraContractMetricsDTO,
	TerraTotalFeeDTO,
	TerraTransactionMetricsDTO,
	TerraValidatorCountDTO,
	TerraWalletMetricsDTO
} from '$lib/models/DTOs/TerraDTOs';
import type { StringPrimitiveObject, TimeSeriesEntryDTO, TimeSeriesEntryDTO2 } from '$lib/models/SharedDTOs';
import { writable } from 'svelte/store';
import { browser } from '$app/environment';
import type { OptimismAddressBalanceDTO, OptimismContractActivityDTO, OptimismContractMetricsDTO, OptimismDAppUsageDTO, OptimismGasMetricsDTO, OptimismL1SubmissionMetricsDTO, OptimismOPHolderMetricsDTO, OptimismTransactionMetricsDTO, OptimismWalletMetricsDTO } from '$lib/models/DTOs/OptimismDTO';
import type { OsmosisDelegateDTO, OsmosisIBCTransferDTO, OsmosisLPExitDTO, OsmosisLPJoinDTO, OsmosisSwapDTO, OsmosisTransferDTO, OsmosisUndelegateDTO } from '$lib/models/DTOs/OsmosisDTOs';

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
	OsmosisL5Delegates = "osmosis-dev-wallet-5-delegate",
	OsmosisL5DevUndelegates = "osmosis-dev-wallet-5-undelegate"
}

const queryTypes = {
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
	[QueryName.OsmosisL5Delegates]: [] as OsmosisDelegateDTO[],
	[QueryName.OsmosisL5DevUndelegates]: [] as OsmosisUndelegateDTO[],
};

interface QuerySubscription {
	queryName: QueryName;
	handler: (arg0: any) => void;
}

export class QuerySubscriptionBuilder {
	private connection: HubConnection | null;

	private subscriptions: QuerySubscription[];

	constructor() {
		this.subscriptions = [];

		if (!browser) {
			this.connection = null!;
			return;
		}

		this.connection = new HubConnectionBuilder()
			.withUrl('/api/hub/query')
			.withAutomaticReconnect()
			.build();

		this.connection.on('SendQueryResult', (queryName, result) => {
			this.subscriptions
				.filter((x) => x.queryName == queryName)
				.forEach((x) => {
					x.handler(result);
				});
		});
	}

	async start() {
        if (this.connection == null) {
            return;
        }

		await this.connection.start();

		this.subscriptions.forEach(async (name) => {
			await this.connection!.send('GetAndSubscribe', name.queryName);
		});
	}

    dispose() {
        if (this.connection == null) {
            return;
        }
        this.connection.stop();
    }

	addQuery<T extends QueryName, R extends typeof queryTypes[T]>(
		name: T,
		handler: (data: R) => void
	) {
		this.subscriptions.push({
			queryName: name,
			handler: handler
		});
		return this;
	}
}

export function createQueryListener<T extends QueryName, R extends typeof queryTypes[T]>(
	builder: QuerySubscriptionBuilder,
	queryName: T
) {
	const { subscribe, set } = writable<R>(queryTypes[queryName] as any);

	builder.addQuery(queryName, (value) => {
		set(value as any);
	});

	return {
		subscribe
	};
}
