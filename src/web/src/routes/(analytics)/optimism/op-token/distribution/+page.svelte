<script lang="ts">
	import TimeSeriesChart from '$lib/charts/TimeSeriesChart.svelte';
	import type { OptimismOPHolderMetricsDTO } from '$lib/models/DTOs/OptimismDTO';
	import {
		createQueryListener,
		QueryName,
		QuerySubscriptionBuilder
	} from '$lib/service/querysubscription';
	import {
		DaySeriesToWeekSeriesByAvg,
		DaySeriesToWeekSeriesByLast,
		DaySeriesToWeekSeriesByMax,
		DaySeriesToWeekSeriesByMin,
		type TimeSeriesEntry
	} from '$lib/service/transform';
	import { isWeeklyModeStoreName } from '$lib/utils/Utils';
	import type { SeriesOption } from 'echarts';
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
	const opHolderBalanceDistributionChart = writable<SeriesOption[]>([]);

	$: makeOPHolderCountChart($opHolderMetricsHistoryQuery, $isWeeklyModeStore);
	function makeOPHolderCountChart(values: OptimismOPHolderMetricsDTO[], isWeeklyMode: boolean) {
		if (values.length == 0) {
			return;
		}

		var opHolderCountSeries = values
			.sort((a, b) => new Date(a.timestamp).getTime() - new Date(b.timestamp).getTime())
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

	$: makeOPHolderBalanceDistributionChart($opHolderMetricsHistoryQuery, $isWeeklyModeStore);
	function makeOPHolderBalanceDistributionChart(
		values: OptimismOPHolderMetricsDTO[],
		isWeeklyMode: boolean
	) {
		if (values.length == 0) {
			return;
		}
		const sortedValues = values.sort(
			(a, b) => new Date(a.timestamp).getTime() - new Date(b.timestamp).getTime()
		);

		var opHolderBalanceMinimumSeries = sortedValues.map<TimeSeriesEntry>((x) => {
			return {
				timestamp: new Date(x.timestamp),
				value: x.minimumBalance
			};
		});
		var opHolderBalanceMaximumSeries = sortedValues.map<TimeSeriesEntry>((x) => {
			return {
				timestamp: new Date(x.timestamp),
				value: x.maximumBalance
			};
		});
		var opHolderBalanceAverageSeries = sortedValues.map<TimeSeriesEntry>((x) => {
			return {
				timestamp: new Date(x.timestamp),
				value: x.averageBalance
			};
		});
		var opHolderBalanceMedianSeries = sortedValues.map<TimeSeriesEntry>((x) => {
			return {
				timestamp: new Date(x.timestamp),
				value: x.medianBalance
			};
		});
		var opHolderBalancePercentile20eries = sortedValues.map<TimeSeriesEntry>((x) => {
			return {
				timestamp: new Date(x.timestamp),
				value: x.percentile20thBalance
			};
		});
		var opHolderBalancePercentile80Series = sortedValues.map<TimeSeriesEntry>((x) => {
			return {
				timestamp: new Date(x.timestamp),
				value: x.percentile80thBalance
			};
		});

		if (isWeeklyMode) {
			opHolderBalanceMinimumSeries = DaySeriesToWeekSeriesByMin(opHolderBalanceMinimumSeries);
			opHolderBalanceMaximumSeries = DaySeriesToWeekSeriesByMax(opHolderBalanceMaximumSeries);
			opHolderBalanceAverageSeries = DaySeriesToWeekSeriesByAvg(opHolderBalanceAverageSeries);
			opHolderBalanceMedianSeries = DaySeriesToWeekSeriesByAvg(opHolderBalanceMedianSeries);
			opHolderBalancePercentile20eries = DaySeriesToWeekSeriesByAvg(
				opHolderBalancePercentile20eries
			);
			opHolderBalancePercentile80Series = DaySeriesToWeekSeriesByAvg(
				opHolderBalancePercentile80Series
			);
		}

		opHolderBalanceDistributionChart.set([
			{
				type: 'line',
				name: 'Average Balance',
				data: opHolderBalanceAverageSeries.map((x) => [x.timestamp, x.value])
			},
			{
				type: 'line',
				name: 'Median Balance',
				data: opHolderBalanceMedianSeries.map((x) => [x.timestamp, x.value])
			},
			{
				type: 'line',
				name: '20th Percentile Balance',
				data: opHolderBalancePercentile20eries.map((x) => [x.timestamp, x.value])
			},
			{
				type: 'line',
				name: '80th Percentile Balance',
				data: opHolderBalancePercentile80Series.map((x) => [x.timestamp, x.value])
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
	<TimeSeriesChart
		title={{ text: 'Holder Balance Distribution' }}
		yAxis={{ type: 'log', min: 0.01 }}
		queryName={QueryName.OptimismOPHolderMetricsHistory}
		series={$opHolderBalanceDistributionChart}
	/>
</div>
