<script lang="ts">
	import TimeSeriesChart from '$lib/charts/TimeSeriesChart.svelte';
	import type { TerraWalletMetricsDTO } from '$lib/models/DTOs/TerraDTOs';
	import {
		createQueryListener,
		QueryName,
		QuerySubscriptionBuilder
	} from '$lib/service/querysubscription';
	import type { SeriesOption } from 'echarts';
	import { onDestroy, onMount } from 'svelte';
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
	onDestroy(async () => {
		subscriptionBuilder.dispose();
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

<div class="p-3 transparent-background rounded-lg text-black">
	<h1 class="text-center text-2xl">Wallet Activity on Terra</h1>

	<h2 class="text-xl font-bold">Definitions</h2>
	<b>Sender:</b>
	<p>An address posting a transaction on the network</p>
	<b>Receiver:</b>
	<p>An address receiving $LUNA tokens</p>
</div>

<div class="xl:grid grid-cols-2 mt-2 p-2 w-full transparent-background rounded-lg">
	<TimeSeriesChart class="h-128" series={$newSeries} />
	<TimeSeriesChart class="h-128" series={$totalSeries} />
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
