<script lang="ts">
	import { page } from '$app/stores';
	import type { OsmosisWalletRankingDTO } from '$lib/models/DTOs/OsmosisDTOs';
	import { getOsmosisWalletRanking } from '$lib/service/queries';
	import { beforeUpdate } from 'svelte';
	import LPRankingList from './LPRankingList.svelte';
	import externalLinkLogo from '$lib/static/logo/external-link.svg';

	var loadingPromise: Promise<OsmosisWalletRankingDTO> = null!;

	$: loadingPromise = loadwalletRanking($page.params.address);
	function loadwalletRanking(address: string) {
		return getOsmosisWalletRanking(address);
	}

	beforeUpdate(() => {
		const rootElement = document.querySelector(':root')! as any;

		rootElement.style.setProperty('--color1', '#f200c9');
		rootElement.style.setProperty('--color2', '#0602bf');
	});

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
	{:then ranking}
		<div class="flex flex-row items-center gap-4 border border-gray-400 p-3 rounded-md bg-gray-200">
			<p class="font-bold">{ranking.address}</p>
			<a
				rel="noreferrer"
				target="_blank"
				href="https://www.mintscan.io/osmosis/account/{$page.params.address}"
			>
				<img class="h-4" src={externalLinkLogo} alt="Open in Mintscan" />
			</a>
		</div>

		<h2>$OSMO Staked</h2>

		<p>{ranking.stakedAmount} $OSMO (#{ranking.stakedRank})</p>

		<hr />

		<h2>Pools</h2>

		<LPRankingList poolRankings={ranking.poolRankings.sort((a, b) => a.rank - b.rank)} />

		<hr class="mt-8 p-1" />

		<p class="text-right font-thin">
			Last Updated: {getAgeString(new Date(ranking.lastUpdatedAt))} ago
		</p>
	{/await}
</div>
