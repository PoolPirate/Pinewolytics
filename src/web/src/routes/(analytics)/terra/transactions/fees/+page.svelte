<script lang="ts">
	import { QueryName, QuerySubscriptionBuilder } from '$lib/service/querysubscription';
	import type { LegendComponentOption, SeriesOption, YAXisComponentOption } from 'echarts';
	import { onMount } from 'svelte';
	import { writable } from 'svelte/store';

	import TimeSeriesChartRoundLog from '$lib/charts/TimeSeriesChart.svelte';
	import type {TerraTotalFeeDTO, TerraTransactionMetricsDTO} from '$lib/models/DTOs/TerraDTOs';

	const valuesStoreTransactionMetric = writable<TerraTransactionMetricsDTO[]>([]);
	const valuesStoreTotalFeesHistory = writable<TerraTotalFeeDTO[]>([]);
	const seriesFeeTotal = writable<SeriesOption[]>([]);
	const seriesFee = writable<SeriesOption[]>([]);

	onMount(async () => {
		await new QuerySubscriptionBuilder()
			.addQuery(QueryName.TerraTransactionMetricsHistory, (value) => {
				valuesStoreTransactionMetric.set(value);
			})
			.addQuery(QueryName.TerraTotalFeesHistory, (value) => {
				valuesStoreTotalFeesHistory.set(value);
			})
			.start();
	});

	$: makeSeriesFeeTotal($valuesStoreTotalFeesHistory);

	function makeSeriesFeeTotal(values: TerraTotalFeeDTO[]) {

		seriesFeeTotal.set([
			{
				type: 'bar',
				data: values
					.sort((a, b) => new Date(a.timestamp).getTime() - new Date(b.timestamp).getTime())
					.map((x) => [new Date(x.timestamp).getTime(), x.feesSinceInception]),

				showBackground: true,
				backgroundStyle: {
					color: 'rgba(180, 180, 180, 0.2)'
				}
			},
		]);}


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
<TimeSeriesChartRoundLog {yAxis} {legend} series={$seriesFeeTotal} />