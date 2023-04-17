<script lang="ts">
	import SingleValueChart from '$lib/charts/SingleValueChart.svelte';
	import type { OsmosisDenominatedAmountDTO } from '$lib/models/DTOs/OsmosisDTOs';
	import { RealtimeFeedName } from '$lib/service/realtime-feed-definitions';
	import { RealtimeValueName } from '$lib/service/realtime-value-definitions';
	import {
		SocketSubscriptionBuilder,
		createRealtimeFeedListener,
		createRealtimeValueListener
	} from '$lib/service/subscriptions';
	import type { SeriesOption } from 'echarts';
	import { onDestroy, onMount } from 'svelte';
	import { writable } from 'svelte/store';

	const subscriptionBuilder = new SocketSubscriptionBuilder();

	onMount(async () => {
		await subscriptionBuilder.start();
	});
	onDestroy(() => {
		subscriptionBuilder.dispose();
	});

	const protoRevTotalRevenueChart = writable<SeriesOption | null>(null);
	const protoRevTotalRevenue = createRealtimeValueListener(
		subscriptionBuilder,
		RealtimeValueName.OsmosisProtoRevTotalRevenue,
		() => []
	);

	const protoRevTotalTrades = createRealtimeValueListener(
		subscriptionBuilder,
		RealtimeValueName.OsmosisProtoRevTotalTradeCount,
		() => []
	);

	const protoRevTxs = createRealtimeFeedListener(
		subscriptionBuilder,
		RealtimeFeedName.OsmosisProtoRevTransactions
	);

	$: makeProtoRevTotalRevenueChart($protoRevTotalRevenue);
	function makeProtoRevTotalRevenueChart(
		totalProtoRevRevenue: OsmosisDenominatedAmountDTO[] | null
	) {
		if (totalProtoRevRevenue == null) {
			return;
		}

		protoRevTotalRevenueChart.set({
			type: 'pie',
			data: totalProtoRevRevenue.map((x) => {
				return {
					name: x.denom,
					value: x.amount
				};
			})
		});
	}
</script>

<SingleValueChart series={$protoRevTotalRevenueChart} queryName={null} />

{#if $protoRevTxs != null}
	{#each $protoRevTxs as tx}
		<a
			rel="external noreferrer"
			href="https://www.mintscan.io/osmosis/txs/{tx.hash}"
			target="_blank">See on Mintscan</a
		>
	{/each}
{/if}
