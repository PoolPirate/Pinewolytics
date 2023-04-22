<script lang="ts">
	import TokenList from '$lib/components/osmosis/TokenList.svelte';
	import type {
		OsmosisDenominatedAmountDTO,
		OsmosisProtoRevTransactionDTO
	} from '$lib/models/DTOs/OsmosisDTOs';
	import { getOsmosisProtoRevTransactions } from '$lib/service/queries';
	import { RealtimeValueName } from '$lib/service/realtime-value-definitions';
	import {
		SocketSubscriptionBuilder,
		createRealtimeFeedListener,
		createRealtimeValueListener
	} from '$lib/service/subscriptions';
	import { onlyUnique } from '$lib/service/transform';
	import { onDestroy, onMount } from 'svelte';
	import { writable } from 'svelte/store';

	const subscriptionBuilder = new SocketSubscriptionBuilder();

	onMount(async () => {
		selectedAddress = localStorage.getItem(selectedAddressStorageKey) ?? '';
		await subscriptionBuilder.start();
	});
	onDestroy(() => {
		subscriptionBuilder.dispose();
	});

	const selectedAddressStorageKey = 'protorev-search-address';
	var selectedAddress: string = '';

	const allTokenInfos = createRealtimeValueListener(
		subscriptionBuilder,
		RealtimeValueName.OsmosisAllTokenInfos,
		() => []
	);

	function loadProtoRevTransactions(address: string) {
		if (!isValidAddress(address)) {
			return [] as OsmosisProtoRevTransactionDTO[];
		}

		localStorage.setItem(selectedAddressStorageKey, selectedAddress);
		return getOsmosisProtoRevTransactions(address);
	}

	function isValidAddress(address: string) {
		return address.startsWith('osmo1') && address.length >= 43;
	}

	function extractDenominatedProfits(transactions: OsmosisProtoRevTransactionDTO[] | null) {
		if (transactions == null) {
			return null;
		}

		const swaps = transactions.flatMap((x) => x.swaps);
		const denoms = swaps.map((x) => x.profit.denom).filter(onlyUnique);

		return denoms.map<OsmosisDenominatedAmountDTO>((x) => {
			return {
				denom: x,
				amount: swaps
					.filter((y) => y.profit.denom == x)
					.reduce((total, swap) => total + swap.profit.amount, 0)
			};
		});
	}
</script>

<div class="grid grid-cols-1 gap-4">
	<div class="flex flex-col transparent-background p-4 gap-2">
		<h2 class="text-lg font-bold">Enter an Osmosis address</h2>
		<input class="p-2" type="text" bind:value={selectedAddress} placeholder="osmo1" />
	</div>

	<hr class="border-b-2 border-white" />

	{#await loadProtoRevTransactions(selectedAddress)}
		<p>Loading...</p>
	{:then protoRevTransactions}
		<h2 class:hidden={!isValidAddress(selectedAddress)} class="font-bold text-xl">
			ProtoRev Stats for {selectedAddress} (Last 180 Days)
		</h2>
		<h2 class:hidden={isValidAddress(selectedAddress)} class="font-bold text-red-600 text-xl">
			Invalid Address {selectedAddress}
		</h2>

		<ul class="grid grid-cols-2 gap-2">
			<li class="p-2 border-2 border-black transparent-background">
				<b>Total Revenue</b>
				<p>
					{Math.round(
						100 *
							protoRevTransactions.reduce(
								(total, tx) => total + tx.swaps.reduce((t, swap) => t + swap.profitUSD, 0),
								0
							)
					) / 100} $
				</p>
			</li>
			<li class="p-2 border-2 border-black transparent-background">
				<b>Total ProtoRev Usages</b>
				<p>{protoRevTransactions.length}</p>
			</li>
			<li />
		</ul>

		<div class="transparent-background p-3 rounded-md">
			<h2 class="text-xl font-bold">Profit Tokens</h2>
			<hr class="m-2 border-black border-t-2" />
			<TokenList
				balance={extractDenominatedProfits(protoRevTransactions)}
				allTokenInfos={$allTokenInfos}
			/>
		</div>

		<div class="flex flex-col transparent-background rounded-md p-3">
			<h2 class="text-xl font-bold">Transactions</h2>
			<hr class="m-2 border-black border-t-2" />
			<table class:hidden={protoRevTransactions == null}>
				<thead>
					<tr>
						<th>Transaction</th>
						<th>Timestamp</th>
						<th>Revenue at tx time ($USD) </th>
					</tr>
				</thead>
				<tbody>
					{#each protoRevTransactions as tx}
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
	{:catch}
		<p>There was an error</p>
	{/await}
</div>

<style>
	th,
	td {
		border: 2px solid black;
	}
</style>
