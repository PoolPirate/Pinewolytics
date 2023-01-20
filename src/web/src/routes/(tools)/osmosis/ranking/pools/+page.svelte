<script lang="ts">
	import type {
		OsmosisPoolInfoDTO,
		OsmosisWalletPoolRankingDTO,
		OsmosisWalletRankingDTO
	} from '$lib/models/DTOs/OsmosisDTOs';
	import { getOsmosisPoolInfosAsync, getOsmosisWalletRanking } from '$lib/service/queries';
	import SinglePoolRanking from './SinglePoolRanking.svelte';
	import externalLinkLogo from '$lib/static/logo/external-link.svg';
	import { goto } from '$app/navigation';
	import { onMount } from 'svelte';
	import ICNSName from '../ICNSName.svelte';

	var loadingPromise: Promise<{
		walletRanking: OsmosisWalletRankingDTO;
		poolRankings: OsmosisWalletPoolRankingDTO[];
		poolInfos: OsmosisPoolInfoDTO[];
	}> = Promise.resolve({ walletRanking: null!, poolInfos: [], poolRankings: [] });

	onMount(() => {
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
		<p>Loading</p>
	{:then { walletRanking, poolRankings, poolInfos }}
		<div
			class="flex flex-row items-center gap-4 border border-gray-400 p-3 rounded-md bg-gray-200 mb-2"
		>
			<p class="font-bold">{walletRanking.address}</p>
			<a rel="noreferrer" target="_blank" href="https://www.mintscan.io/osmosis/account/{address}">
				<img class="h-4" src={externalLinkLogo} alt="Open in Mintscan" />
			</a>
		</div>

		<ICNSName address={walletRanking.address} />

		<hr class="my-4" />

		<h1 class="text-xl font-bold">Pool Rankings</h1>

		<div class="overflow-y-auto max-h-half pr-4">
			{#if poolRankings.length == 0}
				<p>No LP Positions Found</p>
			{/if}

			{#each poolRankings.sort((a, b) => a.rank - b.rank) as poolRanking}
				<SinglePoolRanking
					pool={poolRanking.poolId}
					rank={poolRanking.rank}
					gammAmount={poolRanking.lpTokenBalance}
					assets={poolInfos.find((x) => x.poolId == poolRanking.poolId)?.assets ?? null}
				/>
			{/each}
		</div>

		<hr class="mt-8 p-1" />

		<p class="text-right font-thin">
			Last Updated: {getAgeString(new Date(walletRanking.lastUpdatedAt))} ago
		</p>
	{/await}
</div>

<style>
	.max-h-half {
		max-height: 50vh;
	}
</style>
