<script lang="ts">
	import TimeSeriesChart from '$lib/charts/TimeSeriesChart.svelte';
	import type { OptimismContractMetricsDTO } from '$lib/models/DTOs/OptimismDTO';
	import {
		createQueryListener,
		QueryName,
		QuerySubscriptionBuilder
	} from '$lib/service/querysubscription';
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

	const subscriptionBuilder = new QuerySubscriptionBuilder();
	const contractMetricsHistoryQuery = createQueryListener(
		subscriptionBuilder,
		QueryName.OptimismContractMetricsHistory
	);

	onMount(async () => {
		await subscriptionBuilder.start();
	});
	onDestroy(() => {
		subscriptionBuilder.dispose();
	});

	const contractsChart = writable<SeriesOption[]>([]);
	const developersChart = writable<SeriesOption[]>([]);

	const isWeeklyModeStore = getContext<Readable<boolean>>(isWeeklyModeStoreName);

	$: makeContractsChart($contractMetricsHistoryQuery, $isWeeklyModeStore);
	function makeContractsChart(values: OptimismContractMetricsDTO[], isWeeklyMode: boolean) {
		if (values.length == 0) {
			return;
		}

		var totalContractsSeries = values
			.map<TimeSeriesEntry>((x) => {
				return {
					timestamp: new Date(x.timestamp),
					value: x.totalContracts
				};
			})
			.sort((a, b) => a.timestamp.getTime() - b.timestamp.getTime());
		var newContractsSeries = values
			.map<TimeSeriesEntry>((x) => {
				return {
					timestamp: new Date(x.timestamp),
					value: x.newContracts
				};
			})
			.sort((a, b) => a.timestamp.getTime() - b.timestamp.getTime());

		if (isWeeklyMode) {
			totalContractsSeries = DaySeriesToWeekSeriesByLast(totalContractsSeries);
			newContractsSeries = DaySeriesToWeekSeriesBySum(newContractsSeries);
		}

		contractsChart.set([
			{
				type: 'line',
				yAxisIndex: 0,
				areaStyle: {},
				name: 'Total Contracts Deployed',
				data: totalContractsSeries.map((x) => [x.timestamp, x.value])
			},
			{
				type: 'bar',
				z: 10,
				yAxisIndex: 1,
				name: 'New Contracts Deployed',
				data: newContractsSeries.map((x) => [x.timestamp, x.value])
			}
		]);
	}

	$: makeDevelopersChart($contractMetricsHistoryQuery, $isWeeklyModeStore);
	function makeDevelopersChart(values: OptimismContractMetricsDTO[], isWeeklyMode: boolean) {
		if (values.length == 0) {
			return;
		}

		var totalDevlopersSeries = values
			.map<TimeSeriesEntry>((x) => {
				return {
					timestamp: new Date(x.timestamp),
					value: x.totalDevelopers
				};
			})
			.sort((a, b) => a.timestamp.getTime() - b.timestamp.getTime());
		var activeDevelopersChart = values
			.map<TimeSeriesEntry>((x) => {
				return {
					timestamp: new Date(x.timestamp),
					value: x.activeDevelopers
				};
			})
			.sort((a, b) => a.timestamp.getTime() - b.timestamp.getTime());

		if (isWeeklyMode) {
			totalDevlopersSeries = DaySeriesToWeekSeriesByLast(totalDevlopersSeries);
			activeDevelopersChart = DaySeriesToWeekSeriesByMax(activeDevelopersChart);
		}

		developersChart.set([
			{
				type: 'line',
				yAxisIndex: 0,
				areaStyle: {},
				name: 'Total Developers',
				data: totalDevlopersSeries.map((x) => [x.timestamp, x.value])
			},
			{
				type: 'bar',
				z: 10,
				yAxisIndex: 1,
				name: 'Active Developers',
				data: activeDevelopersChart.map((x) => [x.timestamp, x.value])
			}
		]);
	}
</script>

<div class="grid grid-cols-1 mt-2 p-2 w-full transparent-background rounded-lg">
	<TimeSeriesChart
		yAxis={[{ type: 'log', min: 0.99 }, { type: 'value' }]}
		title={{ text: 'Contracts Deployed' }}
		class="h-128"
		series={$contractsChart}
		queryName={QueryName.OptimismContractMetricsHistory}
	/>

	<TimeSeriesChart
		yAxis={[{ type: 'log', min: 0.99 }, { type: 'value' }]}
		title={{ text: 'Addresses Deploying A Contract (Excluding Contracts)' }}
		class="h-128"
		series={$developersChart}
		queryName={QueryName.OptimismContractMetricsHistory}
	/>
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
