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
		DaySeriesToWeekSeriesByMax,
		DaySeriesToWeekSeriesBySum
	} from '$lib/service/transform';
	import type { TimeSeriesEntry } from '$lib/service/transform';
	import type { SeriesOption } from 'echarts';
	import { getContext, onDestroy, onMount } from 'svelte';
	import { writable, type Readable } from 'svelte/store';
	import { isWeeklyModeStoreName } from '$lib/utils/Utils';

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

	const activeWalletsChart = writable<SeriesOption[]>([]);
	const newWalletsChart = writable<SeriesOption[]>([]);
	const totalWalletsChart = writable<SeriesOption[]>([]);

	const isWeeklyModeStore = getContext<Readable<boolean>>(isWeeklyModeStoreName);

	$: makeActiveWalletsChart($walletMetrics, $isWeeklyModeStore);
	function makeActiveWalletsChart(values: TerraWalletMetricsDTO[], isWeeklyMode: boolean) {
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
			sendersSeries = DaySeriesToWeekSeriesByMax(
				sendersSeries.map((x) => {
					x.value *= 1.2;
					return x;
				})
			);
			receiverSeries = DaySeriesToWeekSeriesByMax(
				receiverSeries.map((x) => {
					x.value *= 1.2;
					return x;
				})
			);
		}

		activeWalletsChart.set([
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

	$: makeNewWalletsChart($walletMetrics, $isWeeklyModeStore);
	function makeNewWalletsChart(values: TerraWalletMetricsDTO[], isWeeklyMode: boolean) {
		if (values.length == 0) {
			return;
		}

		var newSendersSeries = values
			.map<TimeSeriesEntry>((x) => {
				return {
					timestamp: new Date(x.timestamp),
					value: x.totalSenders
				};
			})
			.sort((a, b) => a.timestamp.getTime() - b.timestamp.getTime())
			.map<TimeSeriesEntry>((x, i, arr) => {
				return {
					timestamp: x.timestamp,
					value: x.value - (i == 0 ? 0 : arr[i - 1].value)
				};
			})
			.slice(1); //To solve ^

		var newReceiversSeries = values
			.map<TimeSeriesEntry>((x) => {
				return {
					timestamp: new Date(x.timestamp),
					value: x.totalReceivers
				};
			})
			.sort((a, b) => a.timestamp.getTime() - b.timestamp.getTime())
			.map<TimeSeriesEntry>((x, i, arr) => {
				return {
					timestamp: x.timestamp,
					value: x.value - (i == 0 ? 0 : arr[i - 1].value)
				};
			})
			.slice(1); //To solve ^

		if (isWeeklyMode) {
			newSendersSeries = DaySeriesToWeekSeriesByMax(
				newSendersSeries.map((x) => {
					x.value *= 1.2;
					return x;
				})
			);
			newReceiversSeries = DaySeriesToWeekSeriesByMax(
				newReceiversSeries.map((x) => {
					x.value *= 1.2;
					return x;
				})
			);
		}

		newWalletsChart.set([
			{
				type: 'line',
				name: 'Senders',
				data: newSendersSeries.map((x) => [x.timestamp, x.value])
			},
			{
				type: 'line',
				name: 'Receivers',
				data: newReceiversSeries.map((x) => [x.timestamp, x.value])
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

<div class="grid grid-cols-1 transparent-background rounded-lg mt-2 p-2">
	<TimeSeriesChart
		title={{ text: 'Active Addresses' }}
		class="h-128"
		series={$activeWalletsChart}
		queryName={QueryName.TerraWalletMetricsHistory}
	/>

	<div class="grid grid-cols-1 xl:grid-cols-2  ">
		<TimeSeriesChart
			title={{ text: 'New Addresses' }}
			class="h-128"
			series={$newWalletsChart}
			queryName={QueryName.TerraWalletMetricsHistory}
		/>
		<TimeSeriesChart
			title={{ text: 'Total Unique Addresses' }}
			class="h-128"
			series={$totalWalletsChart}
			queryName={QueryName.TerraWalletMetricsHistory}
		/>
	</div>
</div>
