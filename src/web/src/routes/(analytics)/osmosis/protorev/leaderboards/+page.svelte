<script lang="ts">
	import { QueryName } from '$lib/service/query-definitions';
	import { SocketSubscriptionBuilder, createQueryListener } from '$lib/service/subscriptions';
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

	const protoRevBiggestSwaps = createQueryListener(
		subscriptionBuilder,
		QueryName.OsmosisProtoRevBiggestSwaps
	);

	const protoRevBiggestTx = writable<any>();
</script>

<div class="grid grid-cols-1">
	<div>
		<h2>Transactions with the most revenue</h2>
		<p class:hidden={$protoRevBiggestSwaps != null}>Loading...</p>
		<table class:hidden={$protoRevBiggestSwaps == null}>
			<thead>
				<tr>
					<th>Transaction</th>
					<th>Revenue at tx time ($USD)</th>
				</tr>
			</thead>
			<tbody>
				{#each $protoRevBiggestSwaps as swap}
					<tr>
						<td>
							<a
								class="text-blue-400"
								rel="noreferrer external"
								href="https://www.mintscan.io/osmosis/txs/{swap.hash}"
								>{swap.hash.substring(0, 6)}...{swap.hash.substring(
									swap.hash.length - 6,
									swap.hash.length
								)}</a
							>
						</td>
						<td>
							{Math.round(swap.amountUSD)} $USD
						</td>
					</tr>
				{/each}
			</tbody>
		</table>
	</div>
</div>
