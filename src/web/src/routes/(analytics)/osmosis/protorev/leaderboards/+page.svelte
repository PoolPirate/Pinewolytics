<script lang="ts">
	import type {
		OsmosisProtoRevAddressStatsDTO,
		OsmosisProtoRevSwapDTO,
		OsmosisProtoRevTransactionDTO
	} from '$lib/models/DTOs/OsmosisDTOs';
	import { QueryName } from '$lib/service/query-definitions';
	import { RealtimeValueName } from '$lib/service/realtime-value-definitions';
	import {
		SocketSubscriptionBuilder,
		createQueryListener,
		createRealtimeValueListener
	} from '$lib/service/subscriptions';
	import { isWeeklyModeStoreName } from '$lib/utils/Utils';
	import { getContext, hasContext, onDestroy, onMount } from 'svelte';
	import { writable, type Readable } from 'svelte/store';

	const subscriptionBuilder = new SocketSubscriptionBuilder();
	const isWeeklyModeStore = getContext<Readable<boolean>>(isWeeklyModeStoreName);

	onMount(async () => {
		await subscriptionBuilder.start();
	});
	onDestroy(() => {
		subscriptionBuilder.dispose();
	});

	enum TransactionOrderDirection {
		Timestamp,
		ProfitUSD
	}
	enum AddressOrderDirection {
		ProfitUSD
	}

	const transactionOrderDirection = writable<TransactionOrderDirection>(
		TransactionOrderDirection.ProfitUSD
	);
	const transactionReverseOrdering = writable<boolean>(false);

	const osmosisProtoRevBiggestProfitTransactions = createQueryListener(
		subscriptionBuilder,
		QueryName.OsmosisProtoRevBiggestProfitTransactions
	);
	const selectedTransactions = writable<OsmosisProtoRevTransactionDTO[]>([]);

	const addressOrderDirection = writable<AddressOrderDirection>(AddressOrderDirection.ProfitUSD);
	const addressReverseOrdering = writable<boolean>(false);

	const osmosisProtoRevBiggestProfitAddresses = createQueryListener(
		subscriptionBuilder,
		QueryName.OsmosisProtoRevBiggestProfitAddresses
	);
	const selectedAddresses = writable<OsmosisProtoRevAddressStatsDTO[]>([]);

	const allTokenInfos = createRealtimeValueListener(
		subscriptionBuilder,
		RealtimeValueName.OsmosisAllTokenInfos,
		() => []
	);

	$: selectTransactions(
		$osmosisProtoRevBiggestProfitTransactions,
		$transactionOrderDirection,
		$transactionReverseOrdering
	);
	function selectTransactions(
		transactions: OsmosisProtoRevTransactionDTO[] | null,
		orderDirection: TransactionOrderDirection,
		reverseOrdering: boolean
	) {
		if (transactions == null) {
			return;
		}

		selectedTransactions.set(
			transactions.sort((a, b) => {
				if (orderDirection == TransactionOrderDirection.Timestamp)
					return reverseOrdering
						? new Date(a.timestamp).getTime() - new Date(b.timestamp).getTime()
						: new Date(b.timestamp).getTime() - new Date(a.timestamp).getTime();
				if (orderDirection == TransactionOrderDirection.ProfitUSD)
					return reverseOrdering
						? sumProfits(a.swaps) - sumProfits(b.swaps)
						: sumProfits(b.swaps) - sumProfits(a.swaps);

				throw 'Bad Ordering';
			})
		);
	}

	$: selectAddresses(
		$osmosisProtoRevBiggestProfitAddresses,
		$addressOrderDirection,
		$addressReverseOrdering
	);
	function selectAddresses(
		addresses: OsmosisProtoRevAddressStatsDTO[] | null,
		orderDirection: AddressOrderDirection,
		reverseOrdering: boolean
	) {
		if (addresses == null) {
			return;
		}

		selectedAddresses.set(
			addresses.sort((a, b) => {
				if (orderDirection == AddressOrderDirection.ProfitUSD)
					return reverseOrdering
						? sumProfits(a.aggregatedSwaps) - sumProfits(b.aggregatedSwaps)
						: sumProfits(b.aggregatedSwaps) - sumProfits(a.aggregatedSwaps);

				throw 'Bad Ordering';
			})
		);
	}

	function sumProfits(swaps: OsmosisProtoRevSwapDTO[]) {
		return swaps.reduce((total, swap) => total + swap.profitUSD, 0);
	}

	function setTransactionOrdering(newOrderDirection: TransactionOrderDirection) {
		if ($transactionOrderDirection == newOrderDirection) {
			transactionReverseOrdering.set(!$transactionReverseOrdering);
		} else {
			transactionOrderDirection.set(newOrderDirection);
			transactionReverseOrdering.set(false);
		}
	}

	function setAddressOrdering(newOrderDirection: AddressOrderDirection) {
		if ($addressOrderDirection == newOrderDirection) {
			addressReverseOrdering.set(!$addressReverseOrdering);
		} else {
			addressOrderDirection.set(newOrderDirection);
			addressReverseOrdering.set(false);
		}
	}

	function getOrderingSuffix(
		currentOrderDirection: number,
		isReverse: boolean,
		orderDirection: number
	) {
		if (currentOrderDirection != orderDirection) {
			return '';
		}

		return isReverse ? '▴' : '▾';
	}
</script>

<div class="grid grid-cols-1 gap-4">
	<div class="flex flex-col transparent-background p-3 rounded-md gap-2">
		<h2 class="font-bold text-xl">Transactions with the most revenue</h2>
		<p class:hidden={$selectedTransactions != null}>Loading...</p>
		<table class:hidden={$selectedTransactions == null}>
			<thead>
				<tr>
					<th>Transaction</th>
					<th
						class="cursor-pointer"
						on:click={() => setTransactionOrdering(TransactionOrderDirection.Timestamp)}
						>Timestamp {getOrderingSuffix(
							$transactionOrderDirection,
							$transactionReverseOrdering,
							TransactionOrderDirection.Timestamp
						)}</th
					>
					<th
						class="cursor-pointer"
						on:click={() => setTransactionOrdering(TransactionOrderDirection.ProfitUSD)}
						>Revenue at tx time ($USD)
						{getOrderingSuffix(
							$transactionOrderDirection,
							$transactionReverseOrdering,
							TransactionOrderDirection.ProfitUSD
						)}
					</th>
				</tr>
			</thead>
			<tbody>
				{#each $selectedTransactions as tx}
					<tr>
						<td>
							<a
								class="text-blue-600 font-bold"
								target="_blank"
								rel="noreferrer external"
								href="https://www.mintscan.io/osmosis/txs/{tx.hash}"
								>{tx.hash.substring(0, 6)}...{tx.hash.substring(
									tx.hash.length - 6,
									tx.hash.length
								)}</a
							>
						</td>
						<td>
							{new Date(tx.timestamp).toLocaleDateString()}
						</td>
						<td>
							{#each tx.swaps as swap}
								<p>
									{swap.profit.amount /
										Math.pow(
											10,
											$allTokenInfos?.find((x) => x.denom == swap.profit.denom)?.exponent ?? 0
										)}
									- {$allTokenInfos?.find((x) => x.denom == swap.profit.denom)?.symbol}
									({Math.round(1000 * swap.profitUSD) / 1000} $)
								</p>
							{/each}
						</td>
					</tr>
				{/each}
			</tbody>
		</table>
	</div>

	<hr class="border-2 border-black" />

	<div class="flex flex-col transparent-background p-3 rounded-md gap-2">
		<h2 class="font-bold text-xl">Addresses generating the most revenue</h2>
		<p class:hidden={$selectedAddresses != null}>Loading...</p>
		<table class:hidden={$selectedTransactions == null}>
			<thead>
				<tr>
					<th>Address</th>
					<th
						class="cursor-pointer"
						on:click={() => setAddressOrdering(AddressOrderDirection.ProfitUSD)}
						>Revenue at tx time ($USD)
						{getOrderingSuffix(
							$addressOrderDirection,
							$addressReverseOrdering,
							AddressOrderDirection.ProfitUSD
						)}</th
					>
				</tr>
			</thead>
			<tbody>
				{#each $selectedAddresses as addressStats}
					<tr>
						<td>
							<a
								class="text-blue-600 font-bold"
								target="_blank"
								rel="noreferrer external"
								href="https://www.mintscan.io/osmosis/address/{addressStats.address}"
							>
								{addressStats.address}
							</a>
						</td>
						<td>
							{#each addressStats.aggregatedSwaps as swap}
								<p>
									{swap.profit.amount /
										Math.pow(
											10,
											$allTokenInfos?.find((x) => x.denom == swap.profit.denom)?.exponent ?? 0
										)}
									- {$allTokenInfos?.find((x) => x.denom == swap.profit.denom)?.symbol}
									({Math.round(1000 * swap.profitUSD) / 1000} $)
								</p>
							{/each}
						</td>
					</tr>
				{/each}
			</tbody>
		</table>
	</div>
</div>

<style>
	th,
	td {
		border: 2px solid black;
	}
</style>
