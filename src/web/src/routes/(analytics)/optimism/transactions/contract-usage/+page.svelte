<script lang="ts">
	import SingleValueChart from '$lib/charts/SingleValueChart.svelte';
	import TimeSeriesChart from '$lib/charts/TimeSeriesChart.svelte';
	import type { OptimismContractActivityDTO } from '$lib/models/DTOs/OptimismDTO';
	import type { TerraTransactionMetricsDTO } from '$lib/models/DTOs/TerraDTOs';
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
	const calledContractsChart = writable<SeriesOption[]>([]);

	const isWeeklyModeStore = getContext<Readable<boolean>>(isWeeklyModeStoreName);

	$: makeCalledContractsChart($contractActivityQuery, $isWeeklyModeStore);
	function makeCalledContractsChart(values: OptimismContractActivityDTO[], isWeeklyMode: boolean) {
		if (values.length == 0) {
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

		if (isWeeklyMode) {
			directlyCalledSeries = DaySeriesToWeekSeriesByMax(directlyCalledSeries);
			indirectlyCalledSeries = DaySeriesToWeekSeriesByMax(indirectlyCalledSeries);
		}

		calledContractsChart.set([
			{
				type: 'bar',
				name: 'Contracts Called Directly',
				data: directlyCalledSeries.map((x) => [x.timestamp, x.value])
			},
			{
				type: 'bar',
				name: 'Contracts Called Indirectly',
				data: indirectlyCalledSeries.map((x) => [x.timestamp, x.value])
			}
		]);
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
	<TimeSeriesChart
		title={{ text: 'Contracts Called' }}
		class="h-128 mb-2"
		series={$calledContractsChart}
		queryName={QueryName.OptimismContractActvityHistory}
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
