<script lang="ts">
	import SingleValueChart from '$lib/charts/SingleValueChart.svelte';
	import type { OsmosisDenominatedAmountDTO } from '$lib/models/DTOs/OsmosisDTOs';
	import { RealtimeValueName } from '$lib/service/realtime-value-definitions';
	import {
		SocketSubscriptionBuilder,
		createRealtimeValueListener
	} from '$lib/service/subscriptions';
	import type { SeriesOption } from 'echarts';
	import { onDestroy, onMount } from 'svelte';
	import { writable } from 'svelte/store';

	const subscriptionBuilder = new SocketSubscriptionBuilder();

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

	onMount(async () => {
		await subscriptionBuilder.start();
	});
	onDestroy(() => {
		subscriptionBuilder.dispose();
	});
</script>

<SingleValueChart series={$protoRevTotalRevenueChart} queryName={null} />
