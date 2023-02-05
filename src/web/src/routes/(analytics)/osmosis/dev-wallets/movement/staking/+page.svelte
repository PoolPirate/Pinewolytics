<script lang="ts">
	import SingleValueChart from '$lib/charts/SingleValueChart.svelte';
	import type {
		OsmosisDelegateDTO,
		OsmosisEpochInfoDTO,
		OsmosisStakingRewardDTO,
		OsmosisUndelegateDTO
	} from '$lib/models/DTOs/OsmosisDTOs';
	import type { TimeSeriesEntryDTO } from '$lib/models/SharedDTOs';
	import { sumDelegateVolume, sumUndelegateVolume } from '$lib/service/decode';
	import { QueryName } from '$lib/service/query-definitions';
	import { RealtimeValueName } from '$lib/service/realtime-value-definitions';
	import {
		createQueryListener,
		createRealtimeValueListener,
		SocketSubscriptionBuilder
	} from '$lib/service/subscriptions';
	import {
		genesisEpochProvisions,
		reductionFactor,
		reductionPeriodInEpochs,
		stakingMintShare
	} from '$lib/utils/OsmosisChainParams';
	import type { SeriesOption } from 'echarts';
	import { onDestroy, onMount } from 'svelte';
	import { writable } from 'svelte/store';

	const subscriptionBuilder = new SocketSubscriptionBuilder();

	const shareOfTotalDelegationsChart = writable<SeriesOption | null>(null);
	const stakingAPRWithAndWithoutDevsChart = writable<SeriesOption | null>(null);

	const osmosisDevDelegationsQuery = createQueryListener(
		subscriptionBuilder,
		QueryName.OsmosisL5Delegations
	);
	const osmosisDevUndelegationsQuery = createQueryListener(
		subscriptionBuilder,
		QueryName.OsmosisL5DevUndelegations
	);
	const totalDelegationsHistoryQuery = createQueryListener(
		subscriptionBuilder,
		QueryName.OsmosisTotalDelegationsHistory
	);
	const totalSuperfluidStaked = createRealtimeValueListener(
		subscriptionBuilder,
		RealtimeValueName.OsmosisTotalSuperfluidDelegations,
		() => []
	);
	const epochInfo = createRealtimeValueListener(
		subscriptionBuilder,
		RealtimeValueName.OsmosisEpochInfo,
		() => []
	);

	onMount(async () => {
		await subscriptionBuilder.start();
	});
	onDestroy(() => {
		subscriptionBuilder.dispose();
	});

	$: makeShareOfTotalDelegationsChart(
		$osmosisDevDelegationsQuery,
		$osmosisDevUndelegationsQuery,
		$totalDelegationsHistoryQuery,
		$totalSuperfluidStaked
	);
	function makeShareOfTotalDelegationsChart(
		delegations: OsmosisDelegateDTO[],
		undelegations: OsmosisUndelegateDTO[],
		totalDelegations: TimeSeriesEntryDTO[],
		currentTotalSuperfluidStaked: number | null
	) {
		if (
			delegations.length == 0 ||
			undelegations.length == 0 ||
			totalDelegations.length == 0 ||
			currentTotalSuperfluidStaked == null
		) {
			return;
		}

		const latestTotalDelegations = totalDelegations.sort(
			(a, b) => new Date(b.timestamp).getTime() - new Date(a.timestamp).getTime()
		)[0];

		const devNetDelegated = sumDelegateVolume(delegations) - sumUndelegateVolume(undelegations);
		const otherNetDelegated = latestTotalDelegations.value - devNetDelegated;

		shareOfTotalDelegationsChart.set({
			type: 'pie',
			label: {
				show: false
			},
			top: '7.5%',
			radius: ['40%', '70%'],
			data: [
				{
					name: 'Developer Delegations',
					value: Math.round(devNetDelegated),
					itemStyle: {
						color: 'red'
					}
				},
				{
					name: 'Remaining Delegations',
					value: Math.round(otherNetDelegated)
				},
				{
					name: 'Superfluid Delegations',
					value: Math.round(currentTotalSuperfluidStaked)
				}
			]
		});
	}

	$: makeStakingAPRWithAndWithoutDevsChart(
		$osmosisDevDelegationsQuery,
		$osmosisDevUndelegationsQuery,
		$totalDelegationsHistoryQuery,
		$totalSuperfluidStaked,
		$epochInfo
	);
	function makeStakingAPRWithAndWithoutDevsChart(
		delegations: OsmosisDelegateDTO[],
		undelegations: OsmosisUndelegateDTO[],
		totalDelegations: TimeSeriesEntryDTO[],
		currentTotalSuperfluidStaked: number | null,
		epochInfo: OsmosisEpochInfoDTO | null
	) {
		if (
			delegations.length == 0 ||
			undelegations.length == 0 ||
			totalDelegations.length == 0 ||
			currentTotalSuperfluidStaked == null ||
			epochInfo == null
		) {
			return;
		}

		const latestTotalDelegations = totalDelegations.sort(
			(a, b) => new Date(b.timestamp).getTime() - new Date(a.timestamp).getTime()
		)[0].value;

		const devNetDelegated = sumDelegateVolume(delegations) - sumUndelegateVolume(undelegations);
		const otherNetDelegated = latestTotalDelegations - devNetDelegated;

		const currentInflation =
			Math.pow(reductionFactor, Math.floor(epochInfo.currentEpoch / reductionPeriodInEpochs)) *
			genesisEpochProvisions;

		stakingAPRWithAndWithoutDevsChart.set({
			type: 'gauge',
			startAngle: 90,
			endAngle: -270,
			pointer: {
				show: false
			},
			progress: {
				show: true,
				overlap: false,
				roundCap: true,
				clip: false
			},
			axisLine: {
				lineStyle: {
					width: 40
				}
			},
			splitLine: {
				show: false,
				distance: 0,
				length: 10
			},
			axisTick: {
				show: false
			},
			axisLabel: {
				show: false
			},
			title: {
				fontSize: 14
			},
			detail: {
				width: 100,
				height: 14,
				fontSize: 14,
				color: 'inherit',
				borderColor: 'inherit',
				borderRadius: 20,
				borderWidth: 1,
				formatter: (value) =>
					value.toLocaleString(undefined, { minimumFractionDigits: 2, maximumFractionDigits: 2 }) +
					' %'
			},
			data: [
				{
					name: 'Current APR',
					value:
						(100 * (365 * stakingMintShare * currentInflation)) /
						(latestTotalDelegations + currentTotalSuperfluidStaked),
					title: {
						offsetCenter: ['0%', '-44%']
					},
					detail: {
						valueAnimation: true,
						offsetCenter: ['0%', '-25%']
					}
				},
				{
					name: 'APR Without Devs',
					value:
						(100 * (365 * stakingMintShare * currentInflation)) /
						(otherNetDelegated + currentTotalSuperfluidStaked),
					itemStyle: {
						color: 'purple'
					},
					title: {
						offsetCenter: ['0%', '18%']
					},
					detail: {
						valueAnimation: true,
						offsetCenter: ['0%', '37%']
					}
				}
			]
		});
	}
</script>

<div class="grid grid-cols-1 md:grid-cols-2 transparent-background rounded-xl p-5">
	<SingleValueChart
		showLegend={true}
		showToolTip={true}
		title={{ text: 'Total Delegations Breakdown' }}
		class="h-128"
		queryName={null}
		series={$shareOfTotalDelegationsChart}
	/>
	<SingleValueChart
		title={{ text: 'Staking APR %' }}
		class="h-128"
		queryName={null}
		series={$stakingAPRWithAndWithoutDevsChart}
	/>
</div>
