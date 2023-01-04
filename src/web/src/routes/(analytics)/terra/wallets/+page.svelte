<script lang="ts">
	import TimeSeriesChart from '$lib/charts/TimeSeriesChart.svelte';
	import type { TerraWalletMetricsDTO } from '$lib/models/DTOs/TerraDTOs';
	import {
		createQueryListener,
		QueryName,
		QuerySubscriptionBuilder
	} from '$lib/service/querysubscription';
	import type { SeriesOption } from 'echarts';
	import { onMount } from 'svelte';
	import { writable } from 'svelte/store';

	const subscriptionBuilder = new QuerySubscriptionBuilder();

	const walletMetrics = createQueryListener(
		subscriptionBuilder,
		QueryName.TerraWalletMetricsHistory
	);
	const newSeries = writable<SeriesOption[]>([]);
	const totalSeries = writable<SeriesOption[]>([]);

	onMount(async () => {
		await subscriptionBuilder.start();
	});

	$: makeNewSeries($walletMetrics);
	$: makeTotalSeries($walletMetrics);

	function makeNewSeries(values: TerraWalletMetricsDTO[]) {
		newSeries.set([
			{
				type: 'line',
				name: 'Senders',
				data: values
					.sort((a, b) => new Date(a.timestamp).getTime() - new Date(b.timestamp).getTime())
					.map((x) => [new Date(x.timestamp).getTime(), x.senders])
			},
			{
				type: 'line',
				name: 'Receivers',
				data: values
					.sort((a, b) => new Date(a.timestamp).getTime() - new Date(b.timestamp).getTime())
					.map((x) => [new Date(x.timestamp).getTime(), x.receivers])
			}
		]);
	}

	function makeTotalSeries(values: TerraWalletMetricsDTO[]) {
		totalSeries.set([
			{
				type: 'line',
				name: 'Total Receivers',
				data: values
					.sort((a, b) => new Date(a.timestamp).getTime() - new Date(b.timestamp).getTime())
					.map((x) => [new Date(x.timestamp).getTime(), x.totalReceivers])
			},
			{
				type: 'line',
				name: 'Total Senders',
				data: values
					.sort((a, b) => new Date(a.timestamp).getTime() - new Date(b.timestamp).getTime())
					.map((x) => [new Date(x.timestamp).getTime(), x.totalSenders])
			},
			{
				type: 'line',
				name: 'Total Wallets',
				data: values
					.sort((a, b) => new Date(a.timestamp).getTime() - new Date(b.timestamp).getTime())
					.map((x) => [
						new Date(x.timestamp).getTime(),
						x.totalSendersAndReceivers +
							(x.totalReceivers - x.totalSendersAndReceivers) +
							(x.totalSenders - x.totalSendersAndReceivers)
					])
			}
		]);
	}
</script>

<TimeSeriesChart series={$newSeries} />
<TimeSeriesChart series={$totalSeries} />
