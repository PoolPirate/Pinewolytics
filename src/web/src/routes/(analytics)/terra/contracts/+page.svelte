<script lang="ts">
	import TimeSeriesChart from '$lib/charts/TimeSeriesChart.svelte';
	import type { TerraContractMetricsDTO } from '$lib/models/DTOs/TerraDTOs';
	import {
		createQueryListener,
		QueryName,
		QuerySubscriptionBuilder
	} from '$lib/service/querysubscription';
	import type { SeriesOption } from 'echarts';
	import { onDestroy, onMount } from 'svelte';
	import { writable } from 'svelte/store';

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

	const totalContractsSeries = writable<SeriesOption[]>([]);
	const newContractsSeries = writable<SeriesOption[]>([]);

	$: makeTotalContractsSeries($contractMetricsHistoryQuery);
	function makeTotalContractsSeries(values: TerraContractMetricsDTO[]) {
		if (values.length == 0) {
			return;
		}

		totalContractsSeries.set([
			{
				type: 'line',
				areaStyle: {},
				name: 'Total Contracts Deployed',
				data: values
					.sort((a, b) => new Date(a.timestamp).getTime() - new Date(b.timestamp).getTime())
					.map((value) => [new Date(value.timestamp), value.totalContracts])
			}
		]);
	}

	$: makeNewContractsSeries($contractMetricsHistoryQuery);
	function makeNewContractsSeries(values: TerraContractMetricsDTO[]) {
		if (values.length == 0) {
			return;
		}

		newContractsSeries.set([
			{
				type: 'line',
				areaStyle: {},
				name: 'New Contracts Deployed',
				data: values
					.sort((a, b) => new Date(a.timestamp).getTime() - new Date(b.timestamp).getTime())
					.map((value) => [new Date(value.timestamp), value.newContracts])
			}
		]);
	}
</script>

<div class="grid grid-cols-1">
	<TimeSeriesChart class="h-128" series={$totalContractsSeries} />
	<TimeSeriesChart class="h-128" series={$newContractsSeries} />
</div>
