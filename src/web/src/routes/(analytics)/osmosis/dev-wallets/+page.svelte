<script lang="ts">
	import TimeSeriesChart from '$lib/charts/TimeSeriesChart.svelte';
	import { getEpochInfosAsync } from '$lib/service/osmosislcd';
	import {
		genesisEpochProvisions,
		reductionFactor,
		reductionPeriodInEpochs
	} from '$lib/utils/OsmosisChainParams';
	import type { SeriesOption } from 'echarts';
	import { onMount } from 'svelte';
	import { text } from 'svelte/internal';
	import { writable } from 'svelte/store';

	const developerMintChart = writable<SeriesOption[]>([]);

	onMount(async () => {
		const epochInfo = (await getEpochInfosAsync()).find((x) => x.identifier == 'day')!;

		const estimatedStepSize =
			(new Date().getTime() - epochInfo.startTime.getTime()) / epochInfo.currentEpoch;

		const developerMintedSeries = [...Array(epochInfo.currentEpoch).keys()].map((epoch) => {
			const mint =
				genesisEpochProvisions *
				Math.pow(reductionFactor, Math.floor(epoch / reductionPeriodInEpochs));
			return [new Date(epochInfo.startTime.getTime() + estimatedStepSize * epoch), mint];
		});

		var previous = 0;
		const totalMintedSeries = [...Array(epochInfo.currentEpoch).keys()].map((epoch, i) => {
			const total =
				previous +
				genesisEpochProvisions *
					Math.pow(reductionFactor, Math.floor(epoch / reductionPeriodInEpochs));
			previous = total;
			return [new Date(epochInfo.startTime.getTime() + estimatedStepSize * epoch), total];
		});

		developerMintChart.set([
			{
				name: 'New Minted $OSMO',
				type: 'bar',
				data: developerMintedSeries,
				yAxisIndex: 1
			},
			{
				name: 'Total Minted $OSMO',
				type: 'line',
				areaStyle: {},
				data: totalMintedSeries,
				yAxisIndex: 0
			}
		]);
	});
</script>

<div class="grid grid-cols-1 w-full h-full p-5 rounded-xl transparent-background">
	<TimeSeriesChart
		class="h-128"
		queryName={null}
		title={{ text: 'Developer $OSMO Mint' }}
		series={$developerMintChart}
		yAxis={[{}, {}]}
	/>
</div>
