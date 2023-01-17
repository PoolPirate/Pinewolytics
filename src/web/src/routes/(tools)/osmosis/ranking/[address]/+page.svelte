<script lang="ts">
	import { page } from '$app/stores';
	import type { OsmosisWalletRankingDTO } from '$lib/models/DTOs/OsmosisDTOs';
	import { getOsmosisWalletRanking } from '$lib/service/queries';

	$: loadwalletRanking($page.params.address);
	var loadingPromise: Promise<OsmosisWalletRankingDTO> = null!;

	function loadwalletRanking(address: string) {
		loadingPromise = getOsmosisWalletRanking(address);
	}
</script>

{#await loadingPromise}
	<p>Loading</p>
{:then ranking}
	<p>{ranking.address}</p>
	<p>{ranking.stakedAmount} (#{ranking.stakedRank})</p>

	{#each ranking.poolRankings.sort((a, b) => a.rank - b.rank) as poolRanking}
		<p>{poolRanking.poolId} - {poolRanking.lpTokenBalance} (#{poolRanking.rank})</p>
	{/each}

	<p>Last Updated: {ranking.lastUpdatedAt}</p>
{/await}
