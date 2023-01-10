<script lang="ts">
	import TimeSeriesChart from '$lib/charts/TimeSeriesChart.svelte';
	import ZoomableChart from '$lib/charts/ZoomableChart.svelte';
	import type {
		OptimismAddressBalanceDTO,
		OptimismOPHolderMetricsDTO
	} from '$lib/models/DTOs/OptimismDTO';
	import type { TerraAddressBalanceDTO } from '$lib/models/DTOs/TerraDTOs';
	import {
		createQueryListener,
		QueryName,
		QuerySubscriptionBuilder
	} from '$lib/service/querysubscription';
	import {
		DaySeriesToWeekSeriesByLast,
		DaySeriesToWeekSeriesBySum,
		type TimeSeriesEntry
	} from '$lib/service/transform';
	import { isWeeklyModeStoreName } from '$lib/utils/Utils';
	import type { XAXisComponentOption, SeriesOption } from 'echarts';
	import { getContext, onDestroy, onMount } from 'svelte';
	import { writable, type Readable } from 'svelte/store';

	const subscriptionBuilder = new QuerySubscriptionBuilder();
	const opHolderMetricsHistoryQuery = createQueryListener(
		subscriptionBuilder,
		QueryName.OptimismOPHolderMetricsHistory
	);

	onMount(async () => {
		await subscriptionBuilder.start();
	});
	onDestroy(() => {
		subscriptionBuilder.dispose();
	});

	const isWeeklyModeStore = getContext<Readable<boolean>>(isWeeklyModeStoreName);

	const opHolderCountChart = writable<SeriesOption[]>([]);

	$: makeOPHolderCountChart($opHolderMetricsHistoryQuery, $isWeeklyModeStore);
	function makeOPHolderCountChart(values: OptimismOPHolderMetricsDTO[], isWeeklyMode: boolean) {
		if (values.length == 0) {
			return;
		}

		var opHolderCountSeries = values
			.sort((a, b) => a.holderCount - b.holderCount)
			.map<TimeSeriesEntry>((x) => {
				return {
					timestamp: new Date(x.timestamp),
					value: x.holderCount
				};
			})
			.slice(1);

		if (isWeeklyMode) {
			opHolderCountSeries = DaySeriesToWeekSeriesByLast(opHolderCountSeries);
		}

		var opHolderChangeSeries = opHolderCountSeries
			.map<TimeSeriesEntry>((x, i, arr) => {
				return {
					timestamp: new Date(x.timestamp),
					value: x.value - (i == 0 ? 0 : arr[i - 1].value)
				};
			})
			.slice(1);

		opHolderCountChart.set([
			{
				type: 'bar',
				name: 'Holder Count Change',
				data: opHolderChangeSeries.map((x) => [x.timestamp, x.value]),
				zlevel: 1,
				itemStyle: { color: 'green' }
			},
			{
				type: 'line',
				name: 'Addresses holding $OP',
				areaStyle: {},
				data: opHolderCountSeries.map((x) => [x.timestamp, x.value])
			}
		]);
	}
</script>

<div class="grid grid-cols-1 h-full transparent-background p-2 rounded-lg">
	<TimeSeriesChart
		title={{ text: 'Holder Count' }}
		queryName={QueryName.OptimismOPHolderMetricsHistory}
		series={$opHolderCountChart}
	/>
</div>

<style>
	.transparent-background {
		position: relative;
	}

	.transparent-background::before {
		content: ' ';
		position: absolute;
		left: 0;
		right: 0;
		top: 0;
		bottom: 0;
		background: white;
		opacity: 50%;
		border-radius: inherit;
		pointer-events: none;
		z-index: -1;
	}
</style>
