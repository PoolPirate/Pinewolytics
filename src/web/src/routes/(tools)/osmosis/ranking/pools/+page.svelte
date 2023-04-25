<script lang="ts">
	import type {
		OsmosisPoolInfoDTO,
		OsmosisTokenInfoDTO,
		OsmosisWalletPoolRankingDTO,
		OsmosisWalletRankingDTO
	} from '$lib/models/DTOs/OsmosisDTOs';
	import {
		getOsmosisPoolInfosAsync,
		getOsmosisWalletRanking,
		getQueryValue,
		getRealtimeValueValue
	} from '$lib/service/queries';
	import SinglePoolRanking from './SinglePoolRanking.svelte';
	import externalLinkLogo from '$lib/static/logo/external-link.svg';
	import { goto } from '$app/navigation';
	import { onDestroy, onMount } from 'svelte';
	import ICNSName from '../ICNSName.svelte';
	import Loading from '$lib/static/loading.svg';
	import Warning from '$lib/static/warning.svg';
	import {
		SocketSubscriptionBuilder,
		createRealtimeValueListener
	} from '$lib/service/subscriptions';
	import { RealtimeValueName } from '$lib/service/realtime-value-definitions';

	var loadingPromise: Promise<{
		walletRanking: OsmosisWalletRankingDTO | null;
		poolRankings: OsmosisWalletPoolRankingDTO[];
		poolInfos: OsmosisPoolInfoDTO[];
		tokenInfos: OsmosisTokenInfoDTO[];
	}> = Promise.resolve({ walletRanking: null!, poolInfos: [], poolRankings: [], tokenInfos: [] });

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

		const tokenInfosResult = await getRealtimeValueValue(RealtimeValueName.OsmosisAllTokenInfos);
		const tokenInfos = tokenInfosResult.value as OsmosisTokenInfoDTO[];

		return {
			walletRanking: walletRanking,
			poolRankings: walletRanking.poolRankings,
			poolInfos: poolInfos,
			tokenInfos: tokenInfos
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
		<img class="w-64 p-8 invert" src={Loading} alt="Loading" />
	{:then { walletRanking, poolRankings, poolInfos, tokenInfos }}
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
						{tokenInfos}
					/>
				{/each}
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
			<img class="w-64 p-8" src={Warning} alt="Error" />
			<h1 class="font-bold text-xl">API Unavailable</h1>
			<h2>Try again later :/</h2>
		</div>
	{/await}
</div>

<style>
	.max-h-half {
		max-height: 50vh;
	}
	.invert {
		filter: invert(-1);
	}
</style>
