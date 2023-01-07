<script lang="ts">
	import {
		createQueryListener,
		QueryName,
		QuerySubscriptionBuilder
	} from '$lib/service/querysubscription';
	import type { LegendComponentOption, SeriesOption, YAXisComponentOption } from 'echarts';
	import { getContext, onDestroy, onMount } from 'svelte';
	import { writable, type Readable } from 'svelte/store';

	import TimeSeriesChart from '$lib/charts/TimeSeriesChart.svelte';
	import type { TerraTotalFeeDTO, TerraTransactionMetricsDTO } from '$lib/models/DTOs/TerraDTOs';
	import SingleValueChart from '$lib/charts/SingleValueChart.svelte';
	import {
		DaySeriesToWeekSeriesByAvg,
		DaySeriesToWeekSeriesByMax,
		DaySeriesToWeekSeriesByMin,
		type TimeSeriesEntry
	} from '$lib/service/transform';
	import { isWeeklyModeStoreName } from '$lib/utils/Utils';

	const subscriptionBuilder = new QuerySubscriptionBuilder();
	const transactionMetricsHistoryQuery = createQueryListener(
		subscriptionBuilder,
		QueryName.TerraTransactionMetricsHistory
	);
	const totalFeeHistoryQuery = createQueryListener(
		subscriptionBuilder,
		QueryName.TerraTotalFeesHistory
	);

	onMount(async () => {
		await subscriptionBuilder.start();
	});
	onDestroy(() => {
		subscriptionBuilder.dispose();
	});

	const monthlyTotalFeeChart = writable<SeriesOption>();
	const gasFeesChart = writable<SeriesOption[]>([]);

	const isWeeklyModeStore = getContext<Readable<boolean>>(isWeeklyModeStoreName);

	$: makeMonthlyTotalFeeChart($totalFeeHistoryQuery);
	function makeMonthlyTotalFeeChart(values: TerraTotalFeeDTO[]) {
		if (values.length == 0) {
			return;
		}

		const sortValues = values.sort(
			(a, b) => new Date(b.timestamp).getTime() - new Date(a.timestamp).getTime()
		);

		const boundaries = [];

		var offset = 0;

		for (let i = 0; i < 3; i++) {
			const lastDayIndex =
				sortValues.slice(offset).findIndex((value, index, arr) => {
					const nextMonth = new Date(arr[index + 1].timestamp).getMonth();
					const currMonth = new Date(value.timestamp).getMonth();
					return nextMonth != currMonth;
				}) + offset;

			offset = lastDayIndex + 1;

			boundaries.push(lastDayIndex);
		}

		var lastMonths = [
			sortValues[0].feesSinceInception - sortValues[boundaries[0]].feesSinceInception,
			sortValues[boundaries[0]].feesSinceInception - sortValues[boundaries[1]].feesSinceInception,
			sortValues[boundaries[1]].feesSinceInception - sortValues[boundaries[2]].feesSinceInception
		];

		const limit = Math.max(...lastMonths);

		const gaugeData = [
			{
				value: lastMonths[0],
				name: new Date(sortValues[boundaries[0]].timestamp).toLocaleDateString('en', {
					month: 'long'
				}),
				title: {
					offsetCenter: ['0%', '-33%']
				},
				detail: {
					valueAnimation: true,
					offsetCenter: ['0%', '-20%']
				}
			},
			{
				value: lastMonths[1],
				name: new Date(sortValues[boundaries[1]].timestamp).toLocaleDateString('en', {
					month: 'long'
				}),
				title: {
					offsetCenter: ['0%', '-3%']
				},
				detail: {
					valueAnimation: true,
					offsetCenter: ['0%', '10%']
				}
			},
			{
				value: lastMonths[2],
				itemStyle: {
					color: 'purple'
				},
				name: new Date(sortValues[boundaries[2]].timestamp).toLocaleDateString('en', {
					month: 'long'
				}),
				title: {
					offsetCenter: ['0%', '27%']
				},
				detail: {
					valueAnimation: true,
					offsetCenter: ['0%', '40%']
				}
			}
		];

		monthlyTotalFeeChart.set({
			type: 'gauge',
			startAngle: 90,
			endAngle: -270,
			max: limit,
			pointer: {
				show: false
			},
			progress: {
				show: true,
				overlap: false,
				roundCap: true,
				clip: false
			},
			axisLine: {
				lineStyle: {
					width: 40
				}
			},
			splitLine: {
				show: false,
				distance: 0,
				length: 10
			},
			axisTick: {
				show: false
			},
			axisLabel: {
				show: false
			},
			data: gaugeData,
			title: {
				fontSize: 14
			},
			detail: {
				width: 100,
				height: 14,
				fontSize: 14,
				color: 'inherit',
				borderColor: 'inherit',
				borderRadius: 20,
				borderWidth: 1,
				formatter: (value) => Math.round(value).toLocaleString() + ' $LUNA'
			}
		});
	}

	$: makeGasFeeChart($transactionMetricsHistoryQuery, $isWeeklyModeStore);
	function makeGasFeeChart(values: TerraTransactionMetricsDTO[], isWeeklyMode: boolean) {
		if (values.length == 0) {
			return;
		}

		var minimumFeeSeries = values
			.map<TimeSeriesEntry>((x) => {
				return {
					timestamp: new Date(x.timestamp),
					value: x.minimumFee
				};
			})
			.sort((a, b) => a.timestamp.getTime() - b.timestamp.getTime());
		var maximumFeeSeries = values
			.map<TimeSeriesEntry>((x) => {
				return {
					timestamp: new Date(x.timestamp),
					value: x.maximumFee
				};
			})
			.sort((a, b) => a.timestamp.getTime() - b.timestamp.getTime());
		var medianFeeSeries = values
			.map<TimeSeriesEntry>((x) => {
				return {
					timestamp: new Date(x.timestamp),
					value: x.medianFee
				};
			})
			.sort((a, b) => a.timestamp.getTime() - b.timestamp.getTime());
		var averageFeeSeries = values
			.map<TimeSeriesEntry>((x) => {
				return {
					timestamp: new Date(x.timestamp),
					value: x.averageFee
				};
			})
			.sort((a, b) => a.timestamp.getTime() - b.timestamp.getTime());

		if (isWeeklyMode) {
			minimumFeeSeries = DaySeriesToWeekSeriesByMin(minimumFeeSeries);
			maximumFeeSeries = DaySeriesToWeekSeriesByMax(maximumFeeSeries);
			medianFeeSeries = DaySeriesToWeekSeriesByAvg(medianFeeSeries);
			averageFeeSeries = DaySeriesToWeekSeriesByAvg(averageFeeSeries);
		}

		gasFeesChart.set([
			{
				name: 'Minimum Fee',
				type: 'line',
				smooth: true,

				lineStyle: {
					width: 5
				},
				showSymbol: false,
				emphasis: {
					focus: 'series'
				},
				data: minimumFeeSeries.map((x) => [x.timestamp, x.value])
			},
			{
				name: 'Maximum Fee',
				type: 'line',
				smooth: true,
				lineStyle: {
					width: 5
				},
				showSymbol: false,
				emphasis: {
					focus: 'series'
				},
				data: maximumFeeSeries.map((x) => [x.timestamp, x.value])
			},
			{
				name: 'Median Fee',
				type: 'line',
				smooth: true,
				lineStyle: {
					width: 5
				},
				showSymbol: false,
				emphasis: {
					focus: 'series'
				},
				data: medianFeeSeries.map((x) => [x.timestamp, x.value])
			},
			{
				name: 'Average Fee',
				type: 'line',
				smooth: true,
				lineStyle: {
					width: 5
				},
				showSymbol: false,
				emphasis: {
					focus: 'series'
				},
				data: averageFeeSeries.map((x) => [x.timestamp, x.value])
			}
		]);
	}

	const legend: LegendComponentOption = {
		inactiveColor: '#fff',
		textStyle: {
			fontSize: 14
		}
	};

	const yAxis: YAXisComponentOption = {
		type: 'log'
	};
</script>

<div class="p-3 transparent-background rounded-lg text-black">
	<h1 class="text-center text-2xl">Transaction Fees Paid on Terra</h1>
</div>

<div class="grid grid-cols-1 mt-2 p-2 w-full transparent-background rounded-lg">
	<div class="grid grid-cols-1 md:grid-cols-2">
		<div class="h-128 flex flex-col justify-center items-center">
			<div class="p-16 bg-white rounded-xl text-center">
				<h3 class="font-bold text-xl">Total Amount Of Fees Ever Paid</h3>
				<p>
					{$totalFeeHistoryQuery
						.sort((a, b) => new Date(b.timestamp).getTime() - new Date(a.timestamp).getTime())[0]
						?.feesSinceInception?.toLocaleString()} $LUNA
				</p>
			</div>
		</div>

		<SingleValueChart
			title={{ text: 'Total Fees Paid In Last Months' }}
			class="h-128"
			series={$monthlyTotalFeeChart}
			queryName={QueryName.TerraTotalFeesHistory}
		/>
	</div>

	<TimeSeriesChart
		title={{ text: 'Transaction Fee Per Transaction' }}
		class="h-128"
		{yAxis}
		{legend}
		series={$gasFeesChart}
		queryName={QueryName.TerraTransactionMetricsHistory}
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
