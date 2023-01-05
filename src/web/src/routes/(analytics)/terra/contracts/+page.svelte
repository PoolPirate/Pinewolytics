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

	const totalContractsChart = writable<SeriesOption[]>([]);
	const newContractsChart = writable<SeriesOption[]>([]);

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

		if (isWeeklyMode) {
			totalContractsSeries = DaySeriesToWeekSeriesByLast(totalContractsSeries);
		}

		totalContractsChart.set([
			{
				type: 'line',
				areaStyle: {},
				name: 'Total Contracts Deployed',
				data: totalContractsSeries.map((x) => [x.timestamp, x.value])
			}
		]);
	}

	$: makeNewContractsSeries($contractMetricsHistoryQuery, $isWeeklyModeStore);
	function makeNewContractsSeries(values: TerraContractMetricsDTO[], isWeeklyMode: boolean) {
		if (values.length == 0) {
			return;
		}

		var newContractsSeries = values
			.map<TimeSeriesEntry>((x) => {
				return {
					timestamp: new Date(x.timestamp),
					value: x.newContracts
				};
			})
			.sort((a, b) => a.timestamp.getTime() - b.timestamp.getTime());

		if (isWeeklyMode) {
			newContractsSeries = DaySeriesToWeekSeriesBySum(newContractsSeries);
		}

		newContractsChart.set([
			{
				type: 'line',
				areaStyle: {},
				name: 'New Contracts Deployed',
				data: newContractsSeries.map((x) => [x.timestamp, x.value])
			}
		]);
	}
</script>

<div class="grid grid-cols-1">
	<TimeSeriesChart class="h-128" series={$totalContractsChart} />
	<TimeSeriesChart class="h-128" series={$newContractsChart} />
</div>
