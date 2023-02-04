<script lang="ts">
	import TimeSeriesChart from '$lib/charts/TimeSeriesChart.svelte';
	import { SocketSubscriptionBuilder } from '$lib/service/subscriptions';
	import type { SeriesOption } from 'echarts';
	import { getContext, onDestroy, onMount } from 'svelte';
	import type { Readable } from 'svelte/store';

	const subscriptionBuilder = new SocketSubscriptionBuilder();

	onMount(async () => {
		await subscriptionBuilder.start();
	});
	onDestroy(() => {
		subscriptionBuilder.dispose();
	});

	const TOTAL_SUPPLY = 1_000_000_000;
	const tokenomicsChart: SeriesOption[] = [
		{
			type: 'line',
			name: 'Community Pool',
			stackStrategy: 'all',
			areaStyle: {},
			data: [
				[new Date('2022-05-23'), TOTAL_SUPPLY * 0.2],
				[new Date(), TOTAL_SUPPLY * 0.2]
			]
		},
		{
			type: 'line',
			name: 'Developers',
			stackStrategy: 'all',
			areaStyle: {},
			data: [
				[new Date('2022-05-23'), TOTAL_SUPPLY * 0.1],
				[new Date(), TOTAL_SUPPLY * 0.1]
			]
		},
		{
			type: 'line',
			name: 'Post Attack USTC',
			stackStrategy: 'all',
			areaStyle: {},
			data: [
				[new Date('2022-05-23'), TOTAL_SUPPLY * 0.15 * 0.3],
				[new Date('2022-11-23'), TOTAL_SUPPLY * 0.15 * 0.3],
				[new Date(), TOTAL_SUPPLY * 0.15]
			]
		}
	];
</script>

<TimeSeriesChart queryName={null} series={tokenomicsChart} />
