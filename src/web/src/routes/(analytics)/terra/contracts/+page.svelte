<script lang="ts">
	import TimeSeriesChart from '$lib/charts/TimeSeriesChart.svelte';
	import type { TerraContractMetricsDTO } from '$lib/models/DTOs/TerraDTOs';
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
	const contractMetricsHistoryQuery = createQueryListener(
		subscriptionBuilder,
		QueryName.TerraContractMetricsHistory
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
	function makeContractsChart(values: TerraContractMetricsDTO[], isWeeklyMode: boolean) {
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
	function makeDevelopersChart(values: TerraContractMetricsDTO[], isWeeklyMode: boolean) {
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
		queryName={QueryName.TerraContractMetricsHistory}
	/>

	<TimeSeriesChart
		yAxis={[{ type: 'log', min: 0.99 }, { type: 'value' }]}
		title={{ text: 'Addresses Deploying A Contract' }}
		class="h-128"
		series={$developersChart}
		queryName={QueryName.TerraContractMetricsHistory}
	/>
</div>
