<script lang="ts">
	import SingleValueChart from '$lib/charts/SingleValueChart.svelte';
	import TimeSeriesChart from '$lib/charts/TimeSeriesChart.svelte';
	import RefreshAnimation from '$lib/components/RefreshAnimation.svelte';
	import type { OsmosisEpochInfoDTO, OsmosisTransferDTO } from '$lib/models/DTOs/OsmosisDTOs';
	import {
		DaySeriesToWeekSeriesByLast,
		DaySeriesToWeekSeriesBySum,
		type TimeSeriesEntry
	} from '$lib/service/transform';
	import {
		airdropAmount,
		developerMintShare,
		genesisEpochProvisions,
		reductionFactor,
		reductionPeriodInEpochs
	} from '$lib/utils/OsmosisChainParams';
	import { isWeeklyModeStoreName } from '$lib/utils/Utils';
	import { HubConnectionBuilder, type HubConnection } from '@microsoft/signalr';
	import type { SeriesOption } from 'echarts';
	import { onMount, onDestroy, getContext } from 'svelte';
	import { writable, type Readable } from 'svelte/store';

	import pileIcon from '$lib/static/logo/pile.svg';
	import smallPileIcon from '$lib/static/logo/small_pile.webp';
	import {
		createQueryListener,
		QueryName,
		QuerySubscriptionBuilder
	} from '$lib/service/querysubscription';
	import type { StringPrimitiveObject } from '$lib/models/SharedDTOs';

	const subscriptionBuilder = new QuerySubscriptionBuilder();
	var connection: HubConnection;

	const developerTotalShareChart = writable<SeriesOption | null>(null);
	const developerMovedShareChart = writable<SeriesOption | null>(null);
	const developerMintChart = writable<SeriesOption[]>([]);

	const totalDeveloperMint = writable<number | null>(null);

	const isWeeklyModeStore = getContext<Readable<boolean>>(isWeeklyModeStoreName);

	const totalSupply = writable<number | null>(null);
	var totalSupplyAnimation: RefreshAnimation;
	const communityPoolBalance = writable<number | null>(null);
	var communityPoolBalanceAnimation: RefreshAnimation;
	const epochInfo = writable<OsmosisEpochInfoDTO | null>(null);
	var epochInfoAnimation: RefreshAnimation;

	const osmosisDevL0Wallets = createQueryListener(
		subscriptionBuilder,
		QueryName.OsmosisL0DevWallets
	);
	const osmosisDevL0Transfers = createQueryListener(
		subscriptionBuilder,
		QueryName.OsmosisL0DevTransfers
	);

	onMount(async () => {
		await subscriptionBuilder.start();

		connection = new HubConnectionBuilder()
			.withUrl('/api/hub/osmosis')
			.withAutomaticReconnect()
			.build();

		connection.on('TotalSupply', (newTotalSupply) => {
			totalSupply.set(newTotalSupply);
			totalSupplyAnimation.play();
		});
		connection.on('CommunityPoolBalance', (newCommunityPoolBalance) => {
			communityPoolBalance.set(newCommunityPoolBalance);
			communityPoolBalanceAnimation.play();
		});
		connection.on('CurrentEpochInfo', (newEpochInfo) => {
			epochInfo.set(newEpochInfo);
			epochInfoAnimation.play();
		});

		await connection.start();
	});
	onDestroy(() => {
		if (connection == null) {
			return;
		}

		connection.stop();
		subscriptionBuilder.dispose();
	});

	$: makeDeveloperTotalShareChart($totalSupply, $epochInfo);
	function makeDeveloperTotalShareChart(
		totalSupply: number | null,
		epochInfo: OsmosisEpochInfoDTO | null
	) {
		if (totalSupply == null || epochInfo == null) {
			return;
		}

		const totalDeveloperMint = [...Array(epochInfo.currentEpoch).keys()].reduce((total, epoch) => {
			return (
				total +
				genesisEpochProvisions *
					developerMintShare *
					Math.pow(reductionFactor, Math.floor(epoch / reductionPeriodInEpochs))
			);
		}, 0);

		developerTotalShareChart.set({
			type: 'pie',
			radius: ['40%', '70%'],
			top: '10%',
			label: {
				show: false,
				position: 'center'
			},
			labelLine: {
				show: false
			},
			emphasis: {
				label: {
					show: true,
					fontSize: 20,
					fontWeight: 'bold'
				}
			},
			data: [
				{
					name: 'Developers',
					value: Math.round(totalDeveloperMint)
				},
				{
					name: 'Staking Rewards',
					value: Math.round(totalDeveloperMint) //Also 25%
				},
				{
					name: 'Airdrop',
					value: airdropAmount
				},
				{
					name: 'Everything Else',
					value: Math.round(totalSupply - 2 * totalDeveloperMint - airdropAmount)
				}
			]
		});
	}

	$: makeDeveloperMovedShareChart($epochInfo, $osmosisDevL0Wallets, $osmosisDevL0Transfers);
	function makeDeveloperMovedShareChart(
		epochInfo: OsmosisEpochInfoDTO | null,
		walletsRaw: StringPrimitiveObject[],
		transfers: OsmosisTransferDTO[]
	) {
		if (epochInfo == null || walletsRaw.length == 0 || transfers.length == 0) {
			return;
		}
		const wallets = walletsRaw.map((x) => x.value);
		const totalDeveloperMint = [...Array(epochInfo.currentEpoch).keys()].reduce((total, epoch) => {
			return (
				total +
				genesisEpochProvisions *
					developerMintShare *
					Math.pow(reductionFactor, Math.floor(epoch / reductionPeriodInEpochs))
			);
		}, 0);

		const totalTransferredOut = transfers.reduce((total, transfer) => {
			if (wallets.includes(transfer.receiver) && wallets.includes(transfer.sender)) {
				return total;
			}
			if (wallets.includes(transfer.receiver)) {
				return total - transfer.amount;
			}
			if (wallets.includes(transfer.sender)) {
				return total + transfer.amount;
			}

			throw new Error('Unreachable');
		}, 0);

		developerMovedShareChart.set({
			type: 'pie',
			radius: ['40%', '70%'],
			top: '10%',
			label: {
				show: false,
				position: 'center'
			},
			labelLine: {
				show: false
			},
			emphasis: {
				label: {
					show: true,
					fontSize: 20,
					fontWeight: 'bold'
				}
			},
			data: [
				{
					name: 'Transferred Out',
					value: Math.round(totalTransferredOut)
				},
				{
					name: 'Untouched',
					value: Math.round(totalDeveloperMint - totalTransferredOut)
				}
			]
		});
	}

	$: makeDeveloperMintChart($totalSupply, $epochInfo, $isWeeklyModeStore);
	function makeDeveloperMintChart(
		totalSupply: number | null,
		epochInfo: OsmosisEpochInfoDTO | null,
		isWeekly: boolean
	) {
		if (totalSupply == null || epochInfo == null) {
			return;
		}

		const estimatedStepSize =
			(new Date().getTime() - new Date(epochInfo.startTime).getTime()) / epochInfo.currentEpoch;

		var developerMintedSeries = [...Array(epochInfo.currentEpoch).keys()]
			.map((epoch) => {
				const mint =
					genesisEpochProvisions *
					developerMintShare *
					Math.pow(reductionFactor, Math.floor(epoch / reductionPeriodInEpochs));
				return [
					new Date(new Date(epochInfo.startTime).getTime() + estimatedStepSize * epoch),
					mint
				];
			})
			.map<TimeSeriesEntry>((x) => {
				return {
					timestamp: x[0],
					value: x[1]
				} as TimeSeriesEntry;
			});

		var previous = 0;
		var totalMintedSeries = [...Array(epochInfo.currentEpoch).keys()]
			.map((epoch) => {
				const total =
					previous +
					genesisEpochProvisions *
						developerMintShare *
						Math.pow(reductionFactor, Math.floor(epoch / reductionPeriodInEpochs));
				previous = total;
				return [
					new Date(new Date(epochInfo.startTime).getTime() + estimatedStepSize * epoch),
					total
				];
			})
			.map<TimeSeriesEntry>((x) => {
				return {
					timestamp: x[0],
					value: x[1]
				} as TimeSeriesEntry;
			});

		if (isWeekly) {
			developerMintedSeries = DaySeriesToWeekSeriesBySum(developerMintedSeries);
			totalMintedSeries = DaySeriesToWeekSeriesByLast(totalMintedSeries);
		}

		totalDeveloperMint.set(previous);
		developerMintChart.set([
			{
				name: 'New Minted $OSMO',
				type: 'bar',
				data: developerMintedSeries.map((x) => [x.timestamp, x.value]),
				yAxisIndex: 1
			},
			{
				name: 'Total Minted $OSMO',
				type: 'line',
				areaStyle: {},
				data: totalMintedSeries.map((x) => [x.timestamp, x.value]),
				yAxisIndex: 0
			}
		]);
	}
</script>

<div
	class="grid place-content-start gap-6 grid-cols-1 w-full p-5 rounded-xl transparent-background"
>
	<ul
		class="text-center grid grid-cols-1 lg:grid-cols-3 justify-items-center gap-4
			 [&>li]:p-3 [&>li]:rounded-md [&>li]:bg-purple-400 [&>li]:w-full [&>li]:relative [&>li]
			 [&>li>h2]:font-bold
			 [&>li>svg]:absolute [&>li>svg]:w-12 [&>li>svg]:right-1 [&>li>svg]:top-4
			 [&>li>img]:absolute [&>li>img]:h-12 [&>li>img]:top-4
			 [&>li>h1]:font-bold [&>li>h1]:text-xl"
	>
		<li>
			<h1>Total Supply</h1>
			<img src={pileIcon} alt="" />
			<RefreshAnimation bind:this={totalSupplyAnimation} />
			{#if $totalSupply == null}
				<p class="text-lg">Loading...</p>
			{:else}
				<p class="text-lg">
					{$totalSupply.toLocaleString(undefined, {
						maximumFractionDigits: 0
					})} <b>$OSMO</b>
				</p>
			{/if}
		</li>
		<li>
			<h1>Developer Supply</h1>
			<img src={smallPileIcon} alt="" />
			{#if $totalDeveloperMint == null}
				<p class="text-lg">Loading...</p>
			{:else}
				<p class="text-lg">
					{$totalDeveloperMint.toLocaleString(undefined, {
						maximumFractionDigits: 0
					})} <b>$OSMO</b>
				</p>
			{/if}
		</li>
		<li>
			<h1>Community Pool Balance</h1>
			<img src={smallPileIcon} alt="" />
			<RefreshAnimation bind:this={communityPoolBalanceAnimation} />
			{#if $communityPoolBalance == null}
				<p class="text-lg">Loading...</p>
			{:else}
				<p class="text-lg">
					{$communityPoolBalance.toLocaleString(undefined, {
						maximumFractionDigits: 0
					})} <b>$OSMO</b>
				</p>
			{/if}
		</li>
	</ul>

	<div class="grid grid-cols-1 md:grid-cols-2">
		<SingleValueChart
			class="h-128"
			showLegend={true}
			title={{ text: 'Share of Total $OSMO Supply' }}
			showToolTip={true}
			series={$developerTotalShareChart}
			queryName={null}
		/>

		<SingleValueChart
			class="h-128"
			showLegend={true}
			title={{ text: 'Moved vs Remaining in the Receivers Wallets' }}
			showToolTip={true}
			series={$developerMovedShareChart}
			queryName={QueryName.OsmosisL0DevTransfers}
		/>
	</div>

	<TimeSeriesChart
		class="h-128"
		queryName={null}
		title={{ text: 'Developer $OSMO Mint' }}
		series={$developerMintChart}
		yAxis={[{}, {}]}
	/>
</div>
