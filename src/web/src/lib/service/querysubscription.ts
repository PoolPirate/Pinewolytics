import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import type {
    TerraAddressBalanceDTO,
	TerraContractMetricsDTO,
	TerraTotalFeeDTO,
	TerraTransactionMetricsDTO,
	TerraValidatorCountDTO,
	TerraWalletMetricsDTO
} from '$lib/models/DTOs/TerraDTOs';
import type { TimeSeriesEntryDTO, TimeSeriesEntryDTO2 } from '$lib/models/SharedDTOs';
import { writable } from 'svelte/store';
import { browser } from '$app/environment';
import type { OptimismAddressBalanceDTO, OptimismContractActivityDTO, OptimismContractMetricsDTO, OptimismGasMetricsDTO, OptimismL1SubmissionMetricsDTO, OptimismTransactionMetricsDTO, OptimismWalletMetricsDTO } from '$lib/models/DTOs/OptimismDTO';

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
	OptimismContractActvityHistory = "optimism-contract-activity-history"
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
	[QueryName.OptimismContractActvityHistory]: [] as OptimismContractActivityDTO[]
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
