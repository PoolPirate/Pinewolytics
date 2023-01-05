<script lang="ts">
	import TimeSeriesChart from '$lib/charts/TimeSeriesChart.svelte';
	import type { TerraWalletMetricsDTO } from '$lib/models/DTOs/TerraDTOs';
	import {
		createQueryListener,
		QueryName,
		QuerySubscriptionBuilder
	} from '$lib/service/querysubscription';
	import {
		DaySeriesToWeekSeriesByAvg,
		DaySeriesToWeekSeriesByLast,
		DaySeriesToWeekSeriesBySum
	} from '$lib/service/transform';
	import type { TimeSeriesEntry } from '$lib/service/transform';
	import type { SeriesOption } from 'echarts';
	import { getContext, onDestroy, onMount } from 'svelte';
	import { writable, type Readable } from 'svelte/store';
	import { isWeeklyModeStoreName } from '../+layout.svelte';

	const subscriptionBuilder = new QuerySubscriptionBuilder();

	const walletMetrics = createQueryListener(
		subscriptionBuilder,
		QueryName.TerraWalletMetricsHistory
	);

	onMount(async () => {
		await subscriptionBuilder.start();
	});
	onDestroy(async () => {
		subscriptionBuilder.dispose();
	});

	const newWalletsChart = writable<SeriesOption[]>([]);
	const totalWalletsChart = writable<SeriesOption[]>([]);

	const isWeeklyModeStore = getContext<Readable<boolean>>(isWeeklyModeStoreName);

	$: makeNewSeries($walletMetrics, $isWeeklyModeStore);
	function makeNewSeries(values: TerraWalletMetricsDTO[], isWeeklyMode: boolean) {
		if (values.length == 0) {
			return;
		}

		var sendersSeries = values
			.map<TimeSeriesEntry>((x) => {
				return {
					timestamp: new Date(x.timestamp),
					value: x.senders
				};
			})
			.sort((a, b) => a.timestamp.getTime() - b.timestamp.getTime());
		var receiverSeries = values
			.map<TimeSeriesEntry>((x) => {
				return {
					timestamp: new Date(x.timestamp),
					value: x.receivers
				};
			})
			.sort((a, b) => a.timestamp.getTime() - b.timestamp.getTime());

		if (isWeeklyMode) {
			sendersSeries = DaySeriesToWeekSeriesByAvg(sendersSeries);
			receiverSeries = DaySeriesToWeekSeriesByAvg(receiverSeries);
		}

		newWalletsChart.set([
			{
				type: 'line',
				name: 'Senders',
				data: sendersSeries.map((x) => [x.timestamp, x.value])
			},
			{
				type: 'line',
				name: 'Receivers',
				data: receiverSeries.map((x) => [x.timestamp, x.value])
			}
		]);
	}

	$: makeTotalSeries($walletMetrics, $isWeeklyModeStore);
	function makeTotalSeries(values: TerraWalletMetricsDTO[], isWeeklyMode: boolean) {
		if (values.length == 0) {
			return;
		}

		var totalSendersSeries = values
			.map<TimeSeriesEntry>((x) => {
				return {
					timestamp: new Date(x.timestamp),
					value: x.totalSenders
				};
			})
			.sort((a, b) => a.timestamp.getTime() - b.timestamp.getTime());
		var totalReceiverSeries = values
			.map<TimeSeriesEntry>((x) => {
				return {
					timestamp: new Date(x.timestamp),
					value: x.totalReceivers
				};
			})
			.sort((a, b) => a.timestamp.getTime() - b.timestamp.getTime());
		var totalWalletsSeries = values
			.map<TimeSeriesEntry>((x) => {
				return {
					timestamp: new Date(x.timestamp),
					value:
						x.totalSendersAndReceivers +
						(x.totalReceivers - x.totalSendersAndReceivers) +
						(x.totalSenders - x.totalSendersAndReceivers)
				};
			})
			.sort((a, b) => a.timestamp.getTime() - b.timestamp.getTime());

		if (isWeeklyMode) {
			totalSendersSeries = DaySeriesToWeekSeriesByLast(totalSendersSeries);
			totalReceiverSeries = DaySeriesToWeekSeriesByLast(totalReceiverSeries);
			totalWalletsSeries = DaySeriesToWeekSeriesByLast(totalWalletsSeries);
		}

		totalWalletsChart.set([
			{
				type: 'line',
				name: 'Total Receivers',
				data: totalReceiverSeries.map((x) => [x.timestamp, x.value])
			},
			{
				type: 'line',
				name: 'Total Senders',
				data: totalSendersSeries.map((x) => [x.timestamp, x.value])
			},
			{
				type: 'line',
				name: 'Total Wallets',
				data: totalWalletsSeries.map((x) => [x.timestamp, x.value])
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
	<TimeSeriesChart class="h-128" series={$newWalletsChart} />
	<TimeSeriesChart class="h-128" series={$totalWalletsChart} />
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
