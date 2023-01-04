<script lang="ts">
	import {
		createQueryListener,
		QueryName,
		QuerySubscriptionBuilder
	} from '$lib/service/querysubscription';
	import type { LegendComponentOption, SeriesOption, YAXisComponentOption } from 'echarts';
	import { onMount } from 'svelte';
	import { writable } from 'svelte/store';

	import TimeSeriesChartRoundLog from '$lib/charts/TimeSeriesChart.svelte';
	import type { TerraTotalFeeDTO, TerraTransactionMetricsDTO } from '$lib/models/DTOs/TerraDTOs';
	import SingleValueChart from '$lib/charts/SingleValueChart.svelte';

	const subscriptionBuilder = new QuerySubscriptionBuilder();
	const valuesStoreTransactionMetric = createQueryListener(
		subscriptionBuilder,
		QueryName.TerraTransactionMetricsHistory
	);
	const valuesStoreTotalFeesHistory = createQueryListener(
		subscriptionBuilder,
		QueryName.TerraTotalFeesHistory
	);
	const seriesFeeTotal = writable<SeriesOption[]>([]);
	const seriesFee = writable<SeriesOption[]>([]);

	onMount(async () => {
		await subscriptionBuilder.start();
	});

	$: makeSeriesFeeTotal($valuesStoreTotalFeesHistory);

	function makeSeriesFeeTotal(values: TerraTotalFeeDTO[]) {
		if (values.length == 0) return;

		const sortValues = values.sort(
			(a, b) => new Date(b.timestamp).getTime() - new Date(a.timestamp).getTime()
		);

		var last30 = sortValues[0.0].feesSinceInception - sortValues[30].feesSinceInception;
		var last60 = sortValues[30].feesSinceInception - sortValues[60].feesSinceInception;
		var last90 = sortValues[90].feesSinceInception - sortValues[120].feesSinceInception;

		const lastmax = Math.max(last90, last60, last30);

		last30 /= lastmax;
		last60 /= lastmax;
		last90 /= lastmax;

		const gaugeData = [
			{
				value: Math.ceil(last30 * 100),
				name: 'last30',
				title: {
					offsetCenter: ['0%', '-30%']
				},
				detail: {
					valueAnimation: true,
					offsetCenter: ['0%', '-20%']
				}
			},
			{
				value: Math.ceil(last60 * 100),
				name: 'last60',
				title: {
					offsetCenter: ['0%', '0%']
				},
				detail: {
					valueAnimation: true,
					offsetCenter: ['0%', '10%']
				}
			},
			{
				value: Math.ceil(last90 * 100),
				name: 'last90',
				title: {
					offsetCenter: ['0%', '30%']
				},
				detail: {
					valueAnimation: true,
					offsetCenter: ['0%', '40%']
				}
			}
		];

		seriesFeeTotal.set([
			{
				type: 'gauge',
				startAngle: 90,
				endAngle: -270,
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
					formatter: '{value}%'
				}
			}
		]);
	}

	$: makeSeriesFee($valuesStoreTransactionMetric);

	function makeSeriesFee(values: TerraTransactionMetricsDTO[]) {
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

<TimeSeriesChartRoundLog {yAxis} {legend} series={$seriesFee} />
<SingleValueChart {yAxis} {legend} series={$seriesFeeTotal} />
