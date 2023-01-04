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
				name: 'Transaction Count',
				type: 'line',
				data: values
					.sort((a, b) => new Date(a.timestamp).getTime() - new Date(b.timestamp).getTime())
					.map((x) => [new Date(x.timestamp).getTime(), x.value1]),
				symbol: 'none',
				sampling: 'sum',
				markLine: {
					data: [{ type: 'max', name: 'Max' }]
				},
				areaStyle: {
					origin: "auto"
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
				markLine: {
					data: [{ type: 'max', name: 'Max' }]
				},
				areaStyle: {
					origin: "auto"
				}
			}
		]);
	}
</script>

<TimeSeriesChart series={$series} />
