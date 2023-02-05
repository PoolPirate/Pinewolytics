<script lang="ts">
	import SingleValueChart from '$lib/charts/SingleValueChart.svelte';
	import TimeSeriesChart from '$lib/charts/TimeSeriesChart.svelte';
	import type {
		OsmosisDelegateDTO,
		OsmosisEpochInfoDTO,
		OsmosisStakingRewardDTO,
		OsmosisTotalDelegationsDTO,
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
		DaySeriesToWeekSeriesByLast,
		DaySeriesToWeekSeriesBySum,
		type TimeSeriesEntry
	} from '$lib/service/transform';
	import {
		genesisEpochProvisions,
		reductionFactor,
		reductionPeriodInEpochs,
		stakingMintShare
	} from '$lib/utils/OsmosisChainParams';
	import { isWeeklyModeStoreName } from '$lib/utils/Utils';
	import type { SeriesOption } from 'echarts';
	import { getContext, onDestroy, onMount } from 'svelte';
	import { writable, type Readable } from 'svelte/store';

	const subscriptionBuilder = new SocketSubscriptionBuilder();
	const isWeeklyModeStore = getContext<Readable<boolean>>(isWeeklyModeStoreName);

	const shareOfTotalDelegationsChart = writable<SeriesOption | null>(null);
	const stakingAPRWithAndWithoutDevsChart = writable<SeriesOption | null>(null);
	const devDelegationsChart = writable<SeriesOption[]>([]);
	const devStakingRewardsChart = writable<SeriesOption[]>([]);

	const osmosisDevStakingRewardsQuery = createQueryListener(
		subscriptionBuilder,
		QueryName.OsmosisL5DevStakingRewards
	);
	const osmosisDevTotalDelegationsHistoryQuery = createQueryListener(
		subscriptionBuilder,
		QueryName.OsmosisL5DevTotalDelegationsHistory
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
		$osmosisDevTotalDelegationsHistoryQuery,
		$totalDelegationsHistoryQuery,
		$totalSuperfluidStaked
	);
	function makeShareOfTotalDelegationsChart(
		totalDevDelegations: OsmosisTotalDelegationsDTO[],
		totalDelegations: TimeSeriesEntryDTO[],
		currentTotalSuperfluidStaked: number | null
	) {
		if (
			totalDevDelegations.length == 0 ||
			totalDelegations.length == 0 ||
			currentTotalSuperfluidStaked == null
		) {
			return;
		}

		const latestTotalDelegations = totalDelegations.sort(
			(a, b) => new Date(b.timestamp).getTime() - new Date(a.timestamp).getTime()
		)[0];
		const latestTotalDevDelegationsEntry = totalDevDelegations.sort(
			(a, b) => new Date(b.date).getTime() - new Date(a.date).getTime()
		)[0];
		const latestTotalDevDelegations =
			latestTotalDevDelegationsEntry.totalDelegated -
			latestTotalDevDelegationsEntry.totalUndelegated;
		const otherNetDelegated = latestTotalDelegations.value - latestTotalDevDelegations;

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
					value: Math.round(latestTotalDevDelegations),
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
		$osmosisDevTotalDelegationsHistoryQuery,
		$totalDelegationsHistoryQuery,
		$totalSuperfluidStaked,
		$epochInfo
	);
	function makeStakingAPRWithAndWithoutDevsChart(
		totalDevDelegations: OsmosisTotalDelegationsDTO[],
		totalDelegations: TimeSeriesEntryDTO[],
		currentTotalSuperfluidStaked: number | null,
		epochInfo: OsmosisEpochInfoDTO | null
	) {
		if (
			totalDevDelegations.length == 0 ||
			totalDelegations.length == 0 ||
			currentTotalSuperfluidStaked == null ||
			epochInfo == null
		) {
			return;
		}

		const latestTotalDelegations = totalDelegations.sort(
			(a, b) => new Date(b.timestamp).getTime() - new Date(a.timestamp).getTime()
		)[0].value;
		const latestTotalDevDelegationsEntry = totalDevDelegations.sort(
			(a, b) => new Date(b.date).getTime() - new Date(a.date).getTime()
		)[0];
		const latestTotalDevDelegations =
			latestTotalDevDelegationsEntry.totalDelegated -
			latestTotalDevDelegationsEntry.totalUndelegated;
		const otherNetDelegated = latestTotalDelegations - latestTotalDevDelegations;

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

	$: makeDevStakingRewardsChart($osmosisDevStakingRewardsQuery, $isWeeklyModeStore);
	function makeDevStakingRewardsChart(
		stakingRewards: OsmosisStakingRewardDTO[],
		isWeeklyMode: boolean
	) {
		if (stakingRewards.length == 0) {
			return;
		}

		const dailyStakingRewards = stakingRewards.reduce((dailyTotals, reward) => {
			return dailyTotals.set(reward.date, reward.amount + (dailyTotals.get(reward.date) ?? 0));
		}, new Map<string, number>());

		var newStakingRewardsSeries = Array.from(dailyStakingRewards.entries())
			.map<TimeSeriesEntry>(([date, value]) => {
				return {
					timestamp: new Date(date),
					value: value
				};
			})
			.sort((a, b) => a.timestamp.getTime() - b.timestamp.getTime());

		if (isWeeklyMode) {
			newStakingRewardsSeries = DaySeriesToWeekSeriesBySum(newStakingRewardsSeries);
		}

		const cumulativeStakingRewardsSeries = newStakingRewardsSeries.map<TimeSeriesEntry>(
			(value, i, arr) => {
				return {
					timestamp: value.timestamp,
					value: arr.slice(0, i + 1).reduce((total, value) => total + value.value, 0)
				};
			}
		);

		devStakingRewardsChart.set([
			{
				type: 'bar',
				name: 'New Claimed Rewards',
				data: newStakingRewardsSeries.map((x) => [x.timestamp, Math.round(x.value)]),
				yAxisIndex: 1
			},
			{
				type: 'line',
				areaStyle: {},
				name: 'Total Cumulative Rewards',
				data: cumulativeStakingRewardsSeries.map((x) => [x.timestamp, Math.round(x.value)])
			}
		]);
	}

	$: makeDevDelegationsChart($osmosisDevTotalDelegationsHistoryQuery, $isWeeklyModeStore);
	function makeDevDelegationsChart(
		totalDevDelegations: OsmosisTotalDelegationsDTO[],
		isWeeklyMode: boolean
	) {
		if (totalDevDelegations.length == 0) {
			return;
		}

		var totalDelegationsSeries = totalDevDelegations
			.map<TimeSeriesEntry>((value) => {
				return {
					timestamp: new Date(value.date),
					value: value.totalDelegated - value.totalUndelegated
				};
			})
			.sort((a, b) => a.timestamp.getTime() - b.timestamp.getTime());

		if (isWeeklyMode) {
			totalDelegationsSeries = DaySeriesToWeekSeriesByLast(totalDelegationsSeries);
		}

		devDelegationsChart.set([
			{
				type: 'line',
				areaStyle: {},
				data: totalDelegationsSeries.map((x) => [x.timestamp, x.value])
			}
		]);
	}
</script>

<div class="grid grid-cols-1 gap-2">
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

	<TimeSeriesChart
		class="h-128 transparent-background rounded-xl p-5"
		title={{ text: 'Total Dev Delegations' }}
		queryName={QueryName.OsmosisL5DevTotalDelegationsHistory}
		series={$devDelegationsChart}
		yAxis={[{}, {}]}
	/>

	<TimeSeriesChart
		class="h-128 transparent-background rounded-xl p-5"
		title={{ text: 'Staking Rewards Claimed' }}
		queryName={QueryName.OsmosisL5DevStakingRewards}
		series={$devStakingRewardsChart}
		yAxis={[{}, {}]}
	/>
</div>
