<script lang="ts">
	import ICNSName from '../ICNSName.svelte';
	import type {
		OsmosisPoolInfoDTO,
		OsmosisWalletPoolRankingDTO,
		OsmosisWalletRankingDTO
	} from '$lib/models/DTOs/OsmosisDTOs';
	import { getOsmosisPoolInfosAsync, getOsmosisWalletRanking } from '$lib/service/queries';
	import externalLinkLogo from '$lib/static/logo/external-link.svg';
	import { goto } from '$app/navigation';
	import { onMount } from 'svelte';
	import Loading from '$lib/static/loading.svg';
	import Rank from '../Rank.svelte';
	import Warning from '$lib/static/warning.svg';

	var loadingPromise: Promise<{
		walletRanking: OsmosisWalletRankingDTO | null;
		poolRankings: OsmosisWalletPoolRankingDTO[];
		poolInfos: OsmosisPoolInfoDTO[];
	}> = Promise.resolve({ walletRanking: null!, poolInfos: [], poolRankings: [] });

	onMount(async () => {
		const urlParams = new URLSearchParams(window.location.search);
		const addr = urlParams.get('address');
		if (addr == null) {
			goto('/osmosis/ranking');
			return;
		}

		address = addr;
		loadingPromise = load(address);
	});

	var address = '';

	async function load(address: string | null) {
		if (address == null) {
			goto('/osmosis/ranking');
			return null!;
		}

		const walletRanking = await getOsmosisWalletRanking(address);

		if (walletRanking == null) {
			return {
				walletRanking: null,
				poolRankings: [],
				poolInfos: [],
				tokenInfos: []
			};
		}

		const poolInfos = await getOsmosisPoolInfosAsync(
			walletRanking.poolRankings.map((x) => x.poolId)
		);

		return {
			walletRanking: walletRanking,
			poolRankings: walletRanking.poolRankings,
			poolInfos: poolInfos
		};
	}

	function getAgeString(timestamp: Date) {
		var diff = new Date().getTime() - timestamp.getTime();

		var hours = Math.floor(diff / (1000 * 60 * 60));
		diff -= hours * (1000 * 60 * 60);

		var mins = Math.floor(diff / (1000 * 60));
		diff -= mins * (1000 * 60);

		return hours + ' hours, ' + mins + ' minutes';
	}
</script>

<div class="px-4 pt-4">
	{#await loadingPromise}
		<img class="w-64 p-4 invert" src={Loading} alt="Loading" />
	{:then { walletRanking, poolRankings, poolInfos }}
		{#if walletRanking != null}
			<div
				class="flex flex-row items-center gap-4 border border-gray-400 p-3 rounded-md bg-gray-200 mb-2"
			>
				<p class="font-bold">{walletRanking.address}</p>
				<a
					rel="noreferrer"
					target="_blank"
					href="https://www.mintscan.io/osmosis/account/{address}"
				>
					<img class="h-4" src={externalLinkLogo} alt="Open in Mintscan" />
				</a>
			</div>

			<ICNSName address={walletRanking.address} />

			<hr class="my-4" />

			<div class="flex flex-col gap-1">
				<div
					class="flex flex-row justify-between items-center gap-4 border border-gray-400 p-3 rounded-md bg-gray-200"
				>
					<h1 class="text-lg font-bold">$OSMO Balance</h1>
					{#if walletRanking.balanceAmount > 0}
						<p>{Math.round(1000 * walletRanking.balanceAmount) / 1000} $OSMO</p>
						<Rank rank={walletRanking.balanceRank} class="text-lg font-bold w-4-12" />
					{:else}
						<p>0 $OSMO</p>
						<p>No Rank</p>
					{/if}
				</div>

				<div
					class="flex flex-row justify-between items-center gap-4 border border-gray-400 p-3 rounded-md bg-gray-200"
				>
					<h1 class="text-lg font-bold">Staked $OSMO</h1>

					{#if walletRanking.stakedAmount > 0}
						<p>{Math.round(100 * walletRanking.stakedAmount) / 100} $OSMO</p>
						<Rank rank={walletRanking.stakedRank} class="text-lg font-bold w-4-12" />
					{:else}
						<p>0 $OSMO</p>
						<p>No Rank</p>
					{/if}
				</div>

				<div
					class="flex flex-row justify-between items-center gap-4 border border-gray-400 p-3 rounded-md bg-gray-200"
				>
					<h1 class="text-lg font-bold">Total LP Value</h1>
					<p>Soon TM</p>
				</div>
			</div>

			<hr class="mt-8 p-1" />

			<p class="text-right font-thin">
				Last Updated: {getAgeString(new Date(walletRanking.lastUpdatedAt))} ago
			</p>
		{:else}
			<div class="flex flex-col place-items-center">
				<img class="w-64 p-8" src={Warning} alt="Error" />
				<h1 class="font-bold text-xl">Wallet not Found</h1>
				<h2 class="font-bold text-lg">Make sure that:</h2>
				<ul class="list-disc list-inside">
					<li>Your wallet has made at least 10 transactions on Osmosis</li>
					<li>Your wallet was created at least 48 hours ago</li>
				</ul>
			</div>
		{/if}
	{:catch err}
		<div class="text-center">
			<img class="w-64 p-8" src={Warning} alt="Loading" />
			<h1 class="font-bold text-xl">FlipsideCrypto API Unavailable</h1>
			<h2>Try again later :/</h2>
		</div>
	{/await}
</div>

<style>
	.invert {
		filter: invert(-1);
	}
</style>
