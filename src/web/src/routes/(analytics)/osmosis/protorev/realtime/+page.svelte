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

	const totalProtoRevRevenueChart = writable<SeriesOption | null>(null);
	const totalProtoRevRevenue = createRealtimeValueListener(
		subscriptionBuilder,
		RealtimeValueName.OsmosisTotalProtoRevRevenue,
		() => []
	);

	$: makeTotalProtoRevRevenueChart($totalProtoRevRevenue);
	function makeTotalProtoRevRevenueChart(
		totalProtoRevRevenue: OsmosisDenominatedAmountDTO[] | null
	) {
		if (totalProtoRevRevenue == null) {
			return;
		}

		console.log(totalProtoRevRevenue);

		totalProtoRevRevenueChart.set({
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

<SingleValueChart series={$totalProtoRevRevenueChart} queryName={null} />
