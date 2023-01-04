<script lang="ts">
	import {
		createQueryListener,
		QueryName,
		QuerySubscriptionBuilder
	} from '$lib/service/querysubscription';
	import type { LegendComponentOption, SeriesOption, YAXisComponentOption } from 'echarts';
	import { onDestroy, onMount } from 'svelte';
	import { writable } from 'svelte/store';

	import TimeSeriesChart from '$lib/charts/TimeSeriesChart.svelte';
	import type { TerraTotalFeeDTO, TerraTransactionMetricsDTO } from '$lib/models/DTOs/TerraDTOs';
	import SingleValueChart from '$lib/charts/SingleValueChart.svelte';
	import { ConsoleLogger } from '@microsoft/signalr/dist/esm/Utils';

	const subscriptionBuilder = new QuerySubscriptionBuilder();
	const transactionMetricsHistoryQuery = createQueryListener(
		subscriptionBuilder,
		QueryName.TerraTransactionMetricsHistory
	);
	const totalFeeHistoryQuery = createQueryListener(
		subscriptionBuilder,
		QueryName.TerraTotalFeesHistory
	);

	const seriesFeeTotal = writable<SeriesOption>();
	const seriesFee = writable<SeriesOption[]>([]);

	onMount(async () => {
		await subscriptionBuilder.start();
	});
	onDestroy(() => {
		subscriptionBuilder.dispose();
	});

	$: makeSeriesFeeTotal($totalFeeHistoryQuery);

	function makeSeriesFeeTotal(values: TerraTotalFeeDTO[]) {
		if (values.length == 0) {
			return;
		}

		const sortValues = values.sort(
			(a, b) => new Date(b.timestamp).getTime() - new Date(a.timestamp).getTime()
		);

		const latestMonth = new Date(sortValues[0].timestamp).getMonth();
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
					offsetCenter: ['0%', '-30%']
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
					offsetCenter: ['0%', '0%']
				},
				detail: {
					valueAnimation: true,
					offsetCenter: ['0%', '10%']
				}
			},
			{
				value: lastMonths[2],
				name: new Date(sortValues[boundaries[2]].timestamp).toLocaleDateString('en', {
					month: 'long'
				}),
				title: {
					offsetCenter: ['0%', '30%']
				},
				detail: {
					valueAnimation: true,
					offsetCenter: ['0%', '40%']
				}
			}
		];

		seriesFeeTotal.set({
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
				width: 50,
				height: 14,
				fontSize: 14,
				color: 'inherit',
				borderColor: 'inherit',
				borderRadius: 20,
				borderWidth: 1,
				formatter: (value) => Math.round((100 * value) / limit) + '%'
			}
		});
	}

	$: makeSeriesFee($transactionMetricsHistoryQuery);

	function makeSeriesFee(values: TerraTransactionMetricsDTO[]) {
		if (values.length == 0) {
			return;
		}

		seriesFee.set([
			{
				name: 'minimumFee',
				type: 'line',
				smooth: true,

				lineStyle: {
					width: 5
				},
				showSymbol: false,
				emphasis: {
					focus: 'series'
				},
				data: values
					.sort((a, b) => new Date(a.timestamp).getTime() - new Date(b.timestamp).getTime())
					.map((x) => [new Date(x.timestamp).getTime(), x.minimumFee])
			},
			{
				name: 'maximumFee',
				type: 'line',
				smooth: true,
				lineStyle: {
					width: 5
				},
				showSymbol: false,
				emphasis: {
					focus: 'series'
				},
				data: values
					.sort((a, b) => new Date(a.timestamp).getTime() - new Date(b.timestamp).getTime())
					.map((x) => [new Date(x.timestamp).getTime(), x.maximumFee])
			},
			{
				name: 'medianFee',
				type: 'line',
				smooth: true,
				lineStyle: {
					width: 5
				},
				showSymbol: false,
				emphasis: {
					focus: 'series'
				},
				data: values
					.sort((a, b) => new Date(a.timestamp).getTime() - new Date(b.timestamp).getTime())
					.map((x) => [new Date(x.timestamp).getTime(), x.medianFee])
			},
			{
				name: 'averageFee',
				type: 'line',
				smooth: true,
				lineStyle: {
					width: 5
				},
				showSymbol: false,
				emphasis: {
					focus: 'series'
				},
				data: values
					.sort((a, b) => new Date(a.timestamp).getTime() - new Date(b.timestamp).getTime())
					.map((x) => [new Date(x.timestamp).getTime(), x.averageFee])
			}
		]);
	}

	const legend: LegendComponentOption = {
		inactiveColor: '#fff',
		textStyle: {
			fontSize: 16
		},
		data: ['minimumFee', 'maximumFee', 'medianFee', 'averageFee']
	};

	const yAxis: YAXisComponentOption = {
		type: 'log',
		min: 0.0001
	};
</script>

<div class="p-3 transparent-background rounded-lg text-black">
	<h1 class="text-center text-2xl">Transaction Fees Paid on Terra</h1>
</div>

<div class="xl:grid grid-cols-1 mt-2 p-2 w-full transparent-background rounded-lg">
	<SingleValueChart class="h-128" series={$seriesFeeTotal} />
	<TimeSeriesChart class="h-128" {yAxis} {legend} series={$seriesFee} />
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
