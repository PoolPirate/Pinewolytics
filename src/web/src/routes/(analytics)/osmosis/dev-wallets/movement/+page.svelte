<script lang="ts">
	import SingleValueChart from '$lib/charts/SingleValueChart.svelte';
	import type {
		OsmosisDelegateDTO,
		OsmosisEpochInfoDTO,
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
	import { writable } from 'svelte/store';

	import WalletNetwork from '$lib/static/wallet-network.png';
	import CategoryChart from '$lib/charts/CategoryChart.svelte';
	import { HubConnectionBuilder, type HubConnection } from '@microsoft/signalr';
	import {
		developerMintShare,
		genesisEpochProvisions,
		reductionFactor,
		reductionPeriodInEpochs
	} from '$lib/utils/OsmosisChainParams';

	const subscriptionBuilder = new QuerySubscriptionBuilder();
	var connection: HubConnection;

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

	const osmosisDevL0WalletsQuery = createQueryListener(
		subscriptionBuilder,
		QueryName.OsmosisL0DevWallets
	);

	const osmosisDevL0TransfersQuery = createQueryListener(
		subscriptionBuilder,
		QueryName.OsmosisL0DevTransfers
	);

	const epochInfo = writable<OsmosisEpochInfoDTO | null>(null);

	const usageDistributionChart = writable<SeriesOption | null>(null);

	onMount(async () => {
		await subscriptionBuilder.start();

		connection = new HubConnectionBuilder()
			.withUrl('/api/hub/osmosis')
			.withAutomaticReconnect()
			.build();

		connection.on('CurrentEpochInfo', (newEpochInfo) => {
			epochInfo.set(newEpochInfo);
		});

		await connection.start();
	});
	onDestroy(() => {
		subscriptionBuilder.dispose();
		connection?.stop();
	});

	const categories = {
		LP: 'Added to LP',
		IBC: "IBC'd out",
		TRANSFER: 'Transferred out further',
		SWAP: 'Swapped to other assets',
		STAKE: 'Staked',
		LIQUID: 'Moved and Held',
		UNTOUCHED: 'Untouched'
	};

	$: makeUsageDistributionChart(
		$osmosisDevWalletsQuery.map((x) => x.value),
		$osmosisDevLPJoinsQuery,
		$osmosisDevLPExitsQuery,
		$osmosisDevIBCTransfersQuery,
		$osmosisDevTransfersQuery,
		$osmosisDevSwapsQuery,
		$osmosisDevDelegatesQuery,
		$osmosisDevUndelegatesQuery,
		$epochInfo,
		$osmosisDevL0WalletsQuery.map((x) => x.value),
		$osmosisDevL0TransfersQuery
	);
	function makeUsageDistributionChart(
		wallets: string[],
		lpJoins: OsmosisLPJoinDTO[],
		lpExits: OsmosisLPExitDTO[],
		ibcTransfers: OsmosisIBCTransferDTO[],
		transfers: OsmosisTransferDTO[],
		swaps: OsmosisSwapDTO[],
		delegates: OsmosisDelegateDTO[],
		undelegates: OsmosisUndelegateDTO[],
		epochInfo: OsmosisEpochInfoDTO | null,
		l0wallets: string[],
		l0Transfers: OsmosisTransferDTO[]
	) {
		if (
			wallets.length == 0 ||
			lpJoins.length == 0 ||
			lpExits.length == 0 ||
			ibcTransfers.length == 0 ||
			transfers.length == 0 ||
			swaps.length == 0 ||
			delegates.length == 0 ||
			undelegates.length == 0 ||
			epochInfo == null ||
			l0Transfers.length == 0
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

		const totalDeveloperMint = [...Array(epochInfo.currentEpoch).keys()].reduce((total, epoch) => {
			return (
				total +
				genesisEpochProvisions *
					developerMintShare *
					Math.pow(reductionFactor, Math.floor(epoch / reductionPeriodInEpochs))
			);
		}, 0);

		const totalTransferredOut = l0Transfers.reduce((total, transfer) => {
			if (l0wallets.includes(transfer.receiver) && l0wallets.includes(transfer.sender)) {
				return total;
			}
			if (l0wallets.includes(transfer.receiver)) {
				return total - transfer.amount;
			}
			if (l0wallets.includes(transfer.sender)) {
				return total + transfer.amount;
			}

			throw new Error('Unreachable');
		}, 0);

		const totalUsed = netLP + netIBCTransfers + netTransfers + netSwapVolume + netStaked;

		usageDistributionChart.set({
			type: 'bar',
			data: [
				{
					name: categories.LP,
					value: netLP,
					itemStyle: {
						color: 'red'
					}
				},
				{
					name: categories.IBC,
					value: netIBCTransfers,
					itemStyle: {
						color: 'orange'
					}
				},
				{
					name: categories.TRANSFER,
					value: netTransfers,
					itemStyle: {
						color: 'green'
					}
				},
				{
					name: categories.SWAP,
					value: netSwapVolume,
					itemStyle: {
						color: 'lime'
					}
				},
				{
					name: categories.STAKE,
					value: netStaked,
					itemStyle: {
						color: 'yellow'
					}
				},
				{
					name: categories.LIQUID,
					value: Math.round(totalTransferredOut - totalUsed),
					itemStyle: {
						color: 'lightblue'
					}
				},
				{
					name: categories.UNTOUCHED,
					value: Math.round(totalDeveloperMint - totalTransferredOut),
					itemStyle: {
						color: 'lightblue'
					}
				}
			]
		});
	}
</script>

<div class="grid grid-cols-1 gap-6">
	<div
		class="flex flex-col lg:flex-row items-center lg:items-start justify-around transparent-background p-5 rounded-xl gap-5"
	>
		<div class="flex flex-col gap-2">
			<h2 class="font-bold text-2xl border-b-2 border-black mb-1 pb-1">Methodology</h2>
			<p>
				Tracking where funds of a group of wallets went is a difficult process and you need to start
				doing some estimations & simplifications to do it at this scale!
			</p>
			<h3 class="text-lg font-bold">1. Identifying Dev Wallets</h3>
			<p>
				The first step is to get the list of wallets to be classified as developer owned. To do that
				we start off with the 15 vesting receivers and look for wallets that funds were transfered
				to. Then recursively repeat the process till no new wallets are added to the list. Sounds
				simple but in practice this blows up too much, so we also add the requirement that in order
				for a wallet to be developer owned, at least 50% of all incoming $OSMO must come from other
				wallets classified as developer owned.
			</p>
			<p>
				This is also visualized in the <a
					class="text-blue-700 font-bold"
					href="/osmosis/dev-wallets/wallets">Wallets page</a
				> and the image on the right.
			</p>
			<h3 class="text-lg font-bold">2. Filtering Transactions</h3>
			<p>
				For accurate data we don't want to include all transactions of those wallets but restrict it
				so that a wallet that e.g. received 1000$ OSMO from developers can at most be counted for up
				to $OSMO of selling.
			</p>
			<p>
				Besides that there is also a need to filter out incoming transfers from non-developer
				wallets in some shape or form.
			</p>
			<p>
				Answering those questions in detail is beyond the scope of this analysis. Therefore I went
				for a rather simplistic approach of just substracting incoming funds from the outgoing ones
				to get the <b>Net-$OSMO-Usage.</b>
				Those numbers are likely not super accurate but should give a general idea that is close enough
				to the actual values!
			</p>
		</div>
		<img src={WalletNetwork} alt="Network of Wallet" class="w-3/4 lg:w-1/2" />
	</div>

	<div class="transparent-background p-5 rounded-xl">
		<CategoryChart
			categories={Object.values(categories)}
			queryName={null}
			title={{ text: 'Developer Wallet $OSMO Usage' }}
			showToolTip={true}
			class="h-128"
			series={$usageDistributionChart}
			yAxis={{ type: 'log' }}
		/>
	</div>
</div>
