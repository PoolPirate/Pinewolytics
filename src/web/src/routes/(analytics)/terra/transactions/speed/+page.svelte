<script lang="ts">
	import SingleValueChart from '$lib/charts/SingleValueChart.svelte';
	import TimeSeriesChart from '$lib/charts/TimeSeriesChart.svelte';
	import type { TerraTransactionMetricsDTO } from '$lib/models/DTOs/TerraDTOs';
	import { QueryName } from '$lib/service/query-definitions';
	import { createQueryListener, SocketSubscriptionBuilder } from '$lib/service/subscriptions';
	import {
		DaySeriesToWeekSeriesByLast,
		DaySeriesToWeekSeriesBySum,
		type TimeSeriesEntry
	} from '$lib/service/transform';
	import { isWeeklyModeStoreName } from '$lib/utils/Utils';
	import type { SeriesOption } from 'echarts';
	import { getContext, onDestroy, onMount } from 'svelte';
	import { writable, type Readable } from 'svelte/store';

	const subscriptionBuilder = new SocketSubscriptionBuilder();

	onMount(async () => {
		await subscriptionBuilder.start();
	});
	onDestroy(() => {
		subscriptionBuilder.dispose();
	});

	const txMetricQuery = createQueryListener(
		subscriptionBuilder,
		QueryName.TerraTransactionMetricsHistory
	);
	const newTxCountChart = writable<SeriesOption[]>([]);
	const AverageTPSChart = writable<SeriesOption>();
	const averageBlockTimeChart = writable<SeriesOption>();

	const isWeeklyModeStore = getContext<Readable<boolean>>(isWeeklyModeStoreName);

	$: makeNewTxCountSeries($txMetricQuery, $isWeeklyModeStore);
	function makeNewTxCountSeries(values: TerraTransactionMetricsDTO[], isWeeklyMode: boolean) {
		if (values.length == 0) {
			return;
		}

		var newTxCountSeries = values
			.sort((a, b) => new Date(a.timestamp).getTime() - new Date(b.timestamp).getTime())
			.map<TimeSeriesEntry>((x) => {
				return {
					timestamp: new Date(x.timestamp),
					value: x.transactionCount
				};
			});

		if (isWeeklyMode) {
			newTxCountSeries = DaySeriesToWeekSeriesBySum(newTxCountSeries);
		}

		newTxCountChart.set([
			{
				type: 'line',
				name: 'Transaction Count',
				data: newTxCountSeries.map((x) => [x.timestamp, x.value]),
				areaStyle: {}
			}
		]);
	}

	$: makeTPSSeries($txMetricQuery);
	function makeTPSSeries(values: TerraTransactionMetricsDTO[]) {
		if (values.length == 0) {
			return;
		}

		const sorted = values.sort(
			(a, b) => new Date(b.timestamp).getTime() - new Date(a.timestamp).getTime()
		);

		const dayTPS = sorted[1].transactionCount / (24 * 60 * 60);
		const monthTPS =
			sorted.slice(1, 31).reduce((curr, val) => curr + val.transactionCount, 0) /
			(30 * 24 * 60 * 60);

		const decimals = 3;

		AverageTPSChart.set({
			type: 'gauge',
			max: 1.5 * Math.max(dayTPS, monthTPS),
			detail: {
				width: 60,
				height: 14,
				fontSize: 14,
				color: '#fff',
				backgroundColor: 'inherit',
				borderRadius: 3,
				valueAnimation: true,
				formatter: (val) => val.toFixed(decimals) + ' TPS'
			},
			title: {
				offsetCenter: [0, 35],
				fontSize: 13
			},
			progress: {
				show: true,
				overlap: false,
				clip: false,
				itemStyle: {
					borderWidth: 1,
					borderColor: '#464646'
				}
			},
			axisLabel: {
				show: false
			},
			data: [
				{
					value: dayTPS,
					name: 'Today',
					detail: {
						offsetCenter: ['60%', '110%']
					},
					title: {
						offsetCenter: ['60%', '85%']
					},
					itemStyle: {
						color: 'purple'
					}
				},
				{
					value: monthTPS,
					name: '30D Average',
					detail: {
						offsetCenter: ['-60%', '110%']
					},
					title: {
						offsetCenter: ['-60%', '85%']
					},
					itemStyle: {}
				}
			]
		});
	}

	$: makeBPSSeries($txMetricQuery);
	function makeBPSSeries(values: TerraTransactionMetricsDTO[]) {
		if (values.length == 0) {
			return;
		}

		const sorted = values.sort(
			(a, b) => new Date(b.timestamp).getTime() - new Date(a.timestamp).getTime()
		);

		const dayBlockTime = 1 / (sorted[1].blockCount / (24 * 60 * 60));
		const monthBlockTime =
			1 /
			(sorted.slice(1, 31).reduce((curr, val) => curr + val.blockCount, 0) / (30 * 24 * 60 * 60));

		const decimals = 3;

		averageBlockTimeChart.set({
			type: 'gauge',
			max: 1.5 * Math.max(dayBlockTime, monthBlockTime),
			detail: {
				width: 40,
				height: 14,
				fontSize: 14,
				color: '#fff',
				backgroundColor: 'inherit',
				borderRadius: 3,
				valueAnimation: true,
				formatter: (val) => val.toFixed(decimals) + 's'
			},
			title: {
				offsetCenter: [0, 35],
				fontSize: 13
			},
			progress: {
				show: true,
				overlap: false,
				clip: false,
				itemStyle: {
					borderWidth: 1,
					borderColor: '#464646'
				}
			},
			axisLabel: {
				show: false
			},
			data: [
				{
					value: dayBlockTime,
					name: 'Today',
					detail: {
						offsetCenter: ['50%', '110%']
					},
					title: {
						offsetCenter: ['50%', '85%']
					},
					itemStyle: {
						color: 'purple'
					}
				},
				{
					value: monthBlockTime,
					name: '30D Average',
					detail: {
						offsetCenter: ['-50%', '110%']
					},
					title: {
						offsetCenter: ['-50%', '85%']
					}
				}
			]
		});
	}
</script>

<div class="grid grid-cols-1 mt-2 p-2 w-full transparent-background rounded-lg">
	<div class="grid grid-cols-1 sm:grid-cols-2">
		<SingleValueChart
			title={{ left: 'center', text: 'Transactions Per Second' }}
			class="h-64"
			series={$AverageTPSChart}
			queryName={QueryName.TerraTransactionMetricsHistory}
		/>
		<SingleValueChart
			title={{ left: 'center', text: 'Block Time' }}
			class="h-64"
			series={$averageBlockTimeChart}
			queryName={QueryName.TerraTransactionMetricsHistory}
		/>
	</div>

	<TimeSeriesChart
		title={{ text: 'New Transactions Executed' }}
		class="h-128"
		series={$newTxCountChart}
		queryName={QueryName.TerraTransactionMetricsHistory}
	/>
</div>
