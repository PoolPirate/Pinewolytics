<script lang="ts">
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

	const OsmosisProtoRevBiggestProfitTransactions = createQueryListener(
		subscriptionBuilder,
		QueryName.OsmosisProtoRevBiggestProfitTransactions
	);

	const allTokenInfos = createRealtimeValueListener(
		subscriptionBuilder,
		RealtimeValueName.OsmosisAllTokenInfos,
		() => []
	);

	const protoRevBiggestTx = writable<any>();
</script>

<div class="grid grid-cols-1">
	<div>
		<h2>Transactions with the most revenue</h2>
		<p class:hidden={$OsmosisProtoRevBiggestProfitTransactions != null}>Loading...</p>
		<table class:hidden={$OsmosisProtoRevBiggestProfitTransactions == null}>
			<thead>
				<tr>
					<th>Transaction</th>
					<th>Revenue at tx time ($USD)</th>
				</tr>
			</thead>
			<tbody>
				{#each $OsmosisProtoRevBiggestProfitTransactions as tx}
					<tr>
						<td>
							<a
								class="text-blue-400"
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
							{#each tx.swaps as swap}
								<p>{swap.profit.amount} {swap.profit.denom} ({swap.profitUSD}$)</p>
							{/each}
						</td>
					</tr>
				{/each}
			</tbody>
		</table>
	</div>
</div>
