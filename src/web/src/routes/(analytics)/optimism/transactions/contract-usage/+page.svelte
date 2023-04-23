<script lang="ts">
	import SingleValueChart from '$lib/charts/SingleValueChart.svelte';
	import TimeSeriesChart from '$lib/charts/TimeSeriesChart.svelte';
	import type {
		OptimismContractActivityDTO,
		OptimismDAppUsageDTO
	} from '$lib/models/DTOs/OptimismDTO';
	import { QueryName } from '$lib/service/query-definitions';
	import { createQueryListener, SocketSubscriptionBuilder } from '$lib/service/subscriptions';
	import {
		DaySeriesToWeekSeriesByLast,
		DaySeriesToWeekSeriesByMax,
		DaySeriesToWeekSeriesBySum,
		type TimeSeriesEntry
	} from '$lib/service/transform';
	import { isWeeklyModeStoreName } from '$lib/utils/Utils';
	import type { SeriesOption } from 'echarts';
	import { getContext, onDestroy, onMount } from 'svelte';
	import { writable, type Readable } from 'svelte/store';

	const subscriptionBuilder = new SocketSubscriptionBuilder();

	onMount(async () => {
		await subscriptionBuilder.start();
	});
	onDestroy(() => {
		subscriptionBuilder.dispose();
	});

	const contractActivityQuery = createQueryListener(
		subscriptionBuilder,
		QueryName.OptimismContractActvityHistory
	);
	const dappLeaderboardQuery = createQueryListener(
		subscriptionBuilder,
		QueryName.OptimismDAppLeaderboard
	);

	const calledContractsChart = writable<SeriesOption[]>([]);
	const dappTxCountLeaderboardChart = writable<SeriesOption>();
	const dappAddressCountLeaderboardChart = writable<SeriesOption>();

	const isWeeklyModeStore = getContext<Readable<boolean>>(isWeeklyModeStoreName);

	$: makeCalledContractsChart($contractActivityQuery, $isWeeklyModeStore);
	function makeCalledContractsChart(
		values: OptimismContractActivityDTO[] | null,
		isWeeklyMode: boolean
	) {
		if (values == null) {
			return;
		}

		var directlyCalledSeries = values
			.map<TimeSeriesEntry>((x) => {
				return {
					timestamp: new Date(x.timestamp),
					value: isWeeklyMode ? x.weeklyCalledContracts : x.dailyCalledContracts
				};
			})
			.sort((a, b) => a.timestamp.getTime() - b.timestamp.getTime());
		var indirectlyCalledSeries = values
			.map<TimeSeriesEntry>((x) => {
				return {
					timestamp: new Date(x.timestamp),
					value: isWeeklyMode
						? x.weeklyUsedContracts - x.weeklyCalledContracts
						: x.dailyUsedContracts - x.dailyCalledContracts
				};
			})
			.sort((a, b) => a.timestamp.getTime() - b.timestamp.getTime());
		var totalCallsSeries = values
			.map<TimeSeriesEntry>((x) => {
				return {
					timestamp: new Date(x.timestamp),
					value: isWeeklyMode ? x.weeklyUsedContracts : x.dailyUsedContracts
				};
			})
			.sort((a, b) => a.timestamp.getTime() - b.timestamp.getTime());

		if (isWeeklyMode) {
			directlyCalledSeries = DaySeriesToWeekSeriesByMax(directlyCalledSeries);
			indirectlyCalledSeries = DaySeriesToWeekSeriesByMax(indirectlyCalledSeries);
			totalCallsSeries = DaySeriesToWeekSeriesByMax(totalCallsSeries);
		}

		calledContractsChart.set([
			{
				type: 'bar',
				name: 'Contracts Called Directly',
				stack: 'calls',
				data: directlyCalledSeries.map((x) => [x.timestamp, x.value])
			},
			{
				type: 'bar',
				stack: 'calls',
				name: 'Contracts Called Indirectly',
				data: indirectlyCalledSeries.map((x) => [x.timestamp, x.value])
			},
			{
				type: 'line',
				name: 'Total Active Contracts',
				data: totalCallsSeries.map((x) => [x.timestamp, x.value]),
				itemStyle: {
					color: 'orange'
				}
			}
		]);
	}

	$: makeDAppTxCountLeaderboardChart($dappLeaderboardQuery);
	function makeDAppTxCountLeaderboardChart(values: OptimismDAppUsageDTO[] | null) {
		if (values == null) {
			return;
		}

		values = values.sort((a, b) => b.txCount - a.txCount);

		dappTxCountLeaderboardChart.set({
			name: 'Transaction Count',
			type: 'pie',
			data: values.map((x, i) => {
				return {
					name: x.projectName,
					value: x.txCount,
					itemStyle: {
						color: i == 9 ? 'gray' : undefined
					}
				};
			}),
			selectedMode: 'series',
			radius: [60, 170],
			center: ['65%', '50%'],
			roseType: 'area',
			itemStyle: {
				borderRadius: 8
			},
			label: { show: false }
		});
	}

	$: makeDAppAddressCountLeaderboardChart($dappLeaderboardQuery);
	function makeDAppAddressCountLeaderboardChart(values: OptimismDAppUsageDTO[] | null) {
		if (values == null) {
			return;
		}

		values = values.sort((a, b) => b.addressCount - a.addressCount);

		dappAddressCountLeaderboardChart.set({
			name: 'Unique Address Count',
			type: 'pie',
			data: values.map((x, i) => {
				return {
					name: x.projectName,
					value: x.addressCount,
					itemStyle: {
						color: i == 9 ? 'gray' : undefined
					}
				};
			}),
			selectedMode: 'series',
			radius: [60, 170],
			center: ['65%', '50%'],
			roseType: 'area',
			itemStyle: {
				borderRadius: 8
			},
			label: { show: false }
		});
	}
</script>

<div class="grid grid-cols-1 mt-2 p-2 w-full transparent-background rounded-lg">
	<h1 class="text-xl font-bold">Optimism Smart Contract Usage</h1>
	<p>
		Optimism uses the <a
			class="font-bold text-blue-600"
			href="https://ethereum.org/en/developers/docs/evm/">Ethereum Virtual Machine</a
		> allowing it to run the same contracts you can find on Ethereum.
	</p>
	<p>
		Users can call those contracts either directly, or indirectly. Direct means the actual
		transaction target is the contract, indirect means that the contract is called by another
		contract.
	</p>
</div>
<div class="grid grid-cols-1 mt-2 p-2 w-full transparent-background rounded-lg">
	<div class="grid grid-cols-1 md:grid-cols-2">
		<SingleValueChart
			title={{ text: 'Most used DApps by tx count' }}
			class="h-100"
			series={$dappTxCountLeaderboardChart}
			queryName={QueryName.OptimismDAppLeaderboard}
			showLegend={true}
			sidebarLegend={true}
			showToolTip={true}
		/>
		<SingleValueChart
			title={{ text: 'Most used DApps by address count' }}
			class="h-100"
			series={$dappAddressCountLeaderboardChart}
			queryName={QueryName.OptimismDAppLeaderboard}
			showLegend={true}
			sidebarLegend={true}
			showToolTip={true}
		/>
	</div>
	<TimeSeriesChart
		title={{ text: 'Contracts Called' }}
		class="h-128"
		series={$calledContractsChart}
		queryName={QueryName.OptimismContractActvityHistory}
	/>
</div>
