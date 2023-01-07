<script lang="ts">
	import TimeSeriesChart from '$lib/charts/TimeSeriesChart.svelte';
	import ZoomableChart from '$lib/charts/ZoomableChart.svelte';
	import type { OptimismAddressBalanceDTO } from '$lib/models/DTOs/OptimismDTO';
	import type { TerraAddressBalanceDTO } from '$lib/models/DTOs/TerraDTOs';
	import type { TimeSeriesEntryDTO } from '$lib/models/SharedDTOs';
	import {
		createQueryListener,
		QueryName,
		QuerySubscriptionBuilder
	} from '$lib/service/querysubscription';
	import { DaySeriesToWeekSeriesByAvg, type TimeSeriesEntry } from '$lib/service/transform';
	import { isWeeklyModeStoreName } from '$lib/utils/Utils';
	import type { XAXisComponentOption, SeriesOption } from 'echarts';
	import { getContext, onDestroy, onMount } from 'svelte';
	import { writable, type Readable } from 'svelte/store';

	const subscriptionBuilder = new QuerySubscriptionBuilder();
	const priceHistoryQuery = createQueryListener(
		subscriptionBuilder,
		QueryName.OptimismPriceHistory
	);

	onMount(async () => {
		await subscriptionBuilder.start();
	});
	onDestroy(() => {
		subscriptionBuilder.dispose();
	});

	const isWeeklyModeStore = getContext<Readable<boolean>>(isWeeklyModeStoreName);
	const priceHistoryChart = writable<SeriesOption[]>([]);

	$: makePriceHistoryChart($priceHistoryQuery, $isWeeklyModeStore);
	function makePriceHistoryChart(values: TimeSeriesEntryDTO[], isWeeklyMode: boolean) {
		if (values.length == 0) {
			return;
		}

		var opPriceSeries = values
			.map<TimeSeriesEntry>((x) => {
				return {
					timestamp: new Date(x.timestamp),
					value: x.value
				};
			})
			.sort((a, b) => a.timestamp.getTime() - b.timestamp.getTime());

		if (isWeeklyMode) {
			opPriceSeries = DaySeriesToWeekSeriesByAvg(opPriceSeries);
		}

		priceHistoryChart.set([
			{
				type: 'line',
				name: 'Price',
				data: opPriceSeries.map((x) => [x.timestamp, x.value]),
				sampling: 'lttb',
				markLine: {
					data: [
						{ type: 'max', name: 'Max', lineStyle: { color: 'lime' } },
						{ type: 'min', name: 'Min', lineStyle: { color: 'red' } }
					]
				}
			}
		]);
	}
</script>

<div class="w-full grid grid-cols-1">
	<TimeSeriesChart
		title={{ text: '$OP / USD' }}
		legend={{}}
		class="h-128"
		series={$priceHistoryChart}
	/>
</div>
