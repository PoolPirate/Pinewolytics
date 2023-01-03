<script lang="ts">
	import type { TimeSeriesEntryDTO2 } from '$lib/models/SharedDTOs';
	import { QueryName, QuerySubscriptionBuilder } from '$lib/service/querysubscription';
	import type { SeriesOption } from 'echarts';
	import * as echarts from 'echarts';
	import { onMount } from 'svelte';
	import { writable } from 'svelte/store';

	import TimeSeriesChart from '$lib/charts/TimeSeriesChart.svelte';

	const valuesStore = writable<TimeSeriesEntryDTO2[]>([]);
	const series = writable<SeriesOption[]>([]);

	onMount(async () => {
		await new QuerySubscriptionBuilder()
			.addQuery(QueryName.OptimismTransactionCountHistory, (value) => {
				valuesStore.set(value);
			})
			.start();
	});

	$: makeSeries($valuesStore);

	function makeSeries(values: TimeSeriesEntryDTO2[]) {
		series.set([
			{
				type: 'line',
				data: values
					.sort((a, b) => new Date(a.timestamp).getTime() - new Date(b.timestamp).getTime())
					.map((x) => [new Date(x.timestamp).getTime(), x.value1]),
				name: 'Transaction Count',
				symbol: 'none',
				sampling: 'sum',
				itemStyle: {
					color: 'rgb(255, 70, 131)'
				},
				areaStyle: {
					color: new echarts.graphic.LinearGradient(0, 0, 0, 1, [
						{
							offset: 0,
							color: 'green'
						},
						{
							offset: 1,
							color: 'lime'
						}
					])
				}
			},
			{
				type: 'line',
				data: values
					.sort((a, b) => new Date(a.timestamp).getTime() - new Date(b.timestamp).getTime())
					.map((x) => [new Date(x.timestamp).getTime(), x.value2]),
				name: 'Address Count',
				symbol: 'none',
				sampling: 'sum',
				itemStyle: {
					color: 'rgb(255, 70, 131)'
				},
				areaStyle: {
					color: new echarts.graphic.LinearGradient(0, 0, 0, 1, [
						{
							offset: 0,
							color: 'blue'
						},
						{
							offset: 1,
							color: 'black'
						}
					])
				}
			}
		]);
	}
</script>

<TimeSeriesChart series={$series} />
