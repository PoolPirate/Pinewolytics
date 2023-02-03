<script lang="ts">
	import SingleValueChart from '$lib/charts/SingleValueChart.svelte';
	import type {
		OsmosisDelegateDTO,
		OsmosisIBCTransferDTO,
		OsmosisLPExitDTO,
		OsmosisLPJoinDTO,
		OsmosisSwapDTO,
		OsmosisTransferDTO,
		OsmosisUndelegateDTO
	} from '$lib/models/DTOs/OsmosisDTOs';
	import {
		sumDelegateVolume,
		sumIBCTransferVolume,
		sumLPExitVolume,
		sumLPJoinVolume,
		sumSwapFromVolume,
		sumSwapToVolume,
		sumTransferVolume,
		sumUndelegateVolume
	} from '$lib/service/decode';
	import {
		createQueryListener,
		QueryName,
		QuerySubscriptionBuilder
	} from '$lib/service/querysubscription';
	import type { SeriesOption } from 'echarts';
	import { onDestroy, onMount } from 'svelte';
	import { xlink_attr } from 'svelte/internal';
	import { writable } from 'svelte/store';

	const subscriptionBuilder = new QuerySubscriptionBuilder();

	const osmosisDevWalletsQuery = createQueryListener(
		subscriptionBuilder,
		QueryName.OsmosisL5DevWallets
	);

	const osmosisDevLPJoinsQuery = createQueryListener(
		subscriptionBuilder,
		QueryName.OsmosisL5DevLPJoins
	);

	const osmosisDevLPExitsQuery = createQueryListener(
		subscriptionBuilder,
		QueryName.OsmosisL5DevLPExits
	);

	const osmosisDevIBCTransfersQuery = createQueryListener(
		subscriptionBuilder,
		QueryName.OsmosisL5DevIBCTransfers
	);

	const osmosisDevTransfersQuery = createQueryListener(
		subscriptionBuilder,
		QueryName.OsmosisL5DevTransfers
	);

	const osmosisDevSwapsQuery = createQueryListener(
		subscriptionBuilder,
		QueryName.OsmosisL5DevSwaps
	);

	const osmosisDevDelegatesQuery = createQueryListener(
		subscriptionBuilder,
		QueryName.OsmosisL5Delegates
	);

	const osmosisDevUndelegatesQuery = createQueryListener(
		subscriptionBuilder,
		QueryName.OsmosisL5DevUndelegates
	);

	const usageDistributionChart = writable<SeriesOption | null>(null);

	$: makeUsageDistributionChart(
		$osmosisDevWalletsQuery.map((x) => x.value),
		$osmosisDevLPJoinsQuery,
		$osmosisDevLPExitsQuery,
		$osmosisDevIBCTransfersQuery,
		$osmosisDevTransfersQuery,
		$osmosisDevSwapsQuery,
		$osmosisDevDelegatesQuery,
		$osmosisDevUndelegatesQuery
	);
	function makeUsageDistributionChart(
		wallets: string[],
		lpJoins: OsmosisLPJoinDTO[],
		lpExits: OsmosisLPExitDTO[],
		ibcTransfers: OsmosisIBCTransferDTO[],
		transfers: OsmosisTransferDTO[],
		swaps: OsmosisSwapDTO[],
		delegates: OsmosisDelegateDTO[],
		undelegates: OsmosisUndelegateDTO[]
	) {
		if (
			wallets.length == 0 ||
			lpJoins.length == 0 ||
			lpExits.length == 0 ||
			ibcTransfers.length == 0 ||
			transfers.length == 0 ||
			swaps.length == 0 ||
			delegates.length == 0 ||
			undelegates.length == 0
		) {
			return;
		}

		const netLP = Math.round(
			sumLPJoinVolume(lpJoins.filter((x) => wallets.includes(x.liquidityProviderAddress))) -
				sumLPExitVolume(lpExits.filter((x) => wallets.includes(x.liquidityProviderAddress)))
		);

		const netIBCTransfers = Math.round(
			sumIBCTransferVolume(ibcTransfers.filter((x) => wallets.includes(x.sender))) -
				sumIBCTransferVolume(ibcTransfers.filter((x) => wallets.includes(x.receiver)))
		);

		const netTransfers = Math.round(
			sumTransferVolume(transfers.filter((x) => wallets.includes(x.sender))) -
				sumTransferVolume(transfers.filter((x) => wallets.includes(x.receiver)))
		);

		const netSwapVolume = Math.round(
			sumSwapFromVolume(swaps.filter((x) => x.fromCurrency == 'uosmo')) -
				sumSwapToVolume(swaps.filter((x) => x.toCurrency == 'uosmo'))
		);

		const netStaked = Math.round(
			sumDelegateVolume(delegates.filter((x) => wallets.includes(x.address))) -
				sumUndelegateVolume(undelegates.filter((x) => wallets.includes(x.address)))
		);

		usageDistributionChart.set({
			name: 'Radius Mode',
			type: 'pie',
			radius: [20, 140],
			center: ['25%', '50%'],
			roseType: 'radius',
			itemStyle: {
				borderRadius: 5
			},
			label: {
				show: true
			},
			emphasis: {
				label: {
					show: true
				}
			},
			data: [
				{
					name: 'Added to LP',
					value: netLP
				},
				{
					name: "IBC'd out",
					value: netIBCTransfers
				},
				{
					name: 'Transferred out further',
					value: netTransfers
				},
				{
					name: 'Swapped to other assets',
					value: netSwapVolume
				},
				{
					name: 'Staked',
					value: netStaked
				}
			]
		});
	}

	onMount(async () => {
		await subscriptionBuilder.start();
	});
	onDestroy(() => {
		subscriptionBuilder.dispose();
	});
</script>

<div class="grid grid-cols-1">
	<SingleValueChart
		class="h-128"
		series={$usageDistributionChart}
		showLegend={true}
		showToolTip={true}
		queryName={null}
	/>
</div>
