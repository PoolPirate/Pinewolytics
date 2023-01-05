<script lang="ts">
	import TimeSeriesChart from '$lib/charts/TimeSeriesChart.svelte';
	import type { TerraContractMetricsDTO } from '$lib/models/DTOs/TerraDTOs';
	import {
		createQueryListener,
		QueryName,
		QuerySubscriptionBuilder
	} from '$lib/service/querysubscription';
	import {
		DaySeriesToWeekSeriesByLast,
		DaySeriesToWeekSeriesBySum,
		type TimeSeriesEntry
	} from '$lib/service/transform';
	import type { SeriesOption } from 'echarts';
	import { getContext, onDestroy, onMount } from 'svelte';
	import { writable, type Readable } from 'svelte/store';
	import { isWeeklyModeStoreName } from '../+layout.svelte';

	const subscriptionBuilder = new QuerySubscriptionBuilder();
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

	const isWeeklyModeStore = getContext<Readable<boolean>>(isWeeklyModeStoreName);

	$: makeTotalContractsSeries($contractMetricsHistoryQuery, $isWeeklyModeStore);
	function makeTotalContractsSeries(values: TerraContractMetricsDTO[], isWeeklyMode: boolean) {
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

	//$: makeNewContractsSeries($contractMetricsHistoryQuery, $isWeeklyModeStore);
	function makeNewContractsSeries(values: TerraContractMetricsDTO[], isWeeklyMode: boolean) {
		if (values.length == 0) {
			return;
		}

		if (isWeeklyMode) {
		}
	}
</script>

<div class="grid grid-cols-1">
	<TimeSeriesChart
		yAxis={[{ type: 'log', min: 0.99 }, { type: 'value' }]}
		title={{ text: 'Contracts Deployed' }}
		class="h-128"
		series={$contractsChart}
	/>
</div>
