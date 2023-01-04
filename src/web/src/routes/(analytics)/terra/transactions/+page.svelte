<script lang="ts">
	import { QueryName, QuerySubscriptionBuilder } from '$lib/service/querysubscription';
	import type { SeriesOption } from 'echarts';
	import { onMount } from 'svelte';
	import { writable } from 'svelte/store';

	import TimeSeriesChartRoundLog from '$lib/charts/TimeSeriesChartRoundLog.svelte';
	import type {TerraTransactionMetricsDTO} from '$lib/models/DTOs/TerraDTOs'


	const valuesStore = writable<TerraTransactionMetricsDTO[]>([]);
	const series = writable<SeriesOption[]>([]);

	onMount(async () => {
		await new QuerySubscriptionBuilder()
			.addQuery(QueryName.TerraTransactionMetricHistory, (value) => {
				valuesStore.set(value);
			})
			.start();
	});

	$: makeSeries($valuesStore);

	function makeSeries(values: TerraTransactionMetricsDTO[]) {
		series.set([
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
					.map((x) => [new Date(x.timestamp).getTime(), x.minimumFee]),
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
					.map((x) => [new Date(x.timestamp).getTime(), x.maximumFee]),
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
					.map((x) => [new Date(x.timestamp).getTime(), x.medianFee]),
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
					.map((x) => [new Date(x.timestamp).getTime(), x.averageFee]),
			}
		]);
	}
</script>

<TimeSeriesChartRoundLog series={$series} />
