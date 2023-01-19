<script lang="ts">
	import type {
		OsmosisPoolInfoDTO,
		OsmosisWalletPoolRankingDTO
	} from '$lib/models/DTOs/OsmosisDTOs';
	import { getOsmosisPoolInfosAsync } from '$lib/service/queries';
	import PoolRanking from './SinglePoolRanking.svelte';

	export let poolRankings: OsmosisWalletPoolRankingDTO[];

	var poolInfosPromise: Promise<OsmosisPoolInfoDTO[]>;

	$: poolInfosPromise = updatePoolInfos(poolRankings);
	function updatePoolInfos(poolRankings: OsmosisWalletPoolRankingDTO[]) {
		return getOsmosisPoolInfosAsync(poolRankings.map((x) => x.poolId));
	}
</script>

<div class="overflow-y-auto max-h-half">
	{#await poolInfosPromise}
		{#each poolRankings as poolRanking}
			<PoolRanking
				pool={poolRanking.poolId}
				rank={poolRanking.rank}
				gammAmount={poolRanking.lpTokenBalance}
				assets={null}
			/>
		{/each}
	{:then poolInfos}
		{#each poolRankings as poolRanking}
			<PoolRanking
				pool={poolRanking.poolId}
				rank={poolRanking.rank}
				gammAmount={poolRanking.lpTokenBalance}
				assets={poolInfos.find((x) => x.poolId == poolRanking.poolId)?.assets ?? null}
			/>
		{/each}
	{/await}
</div>

<style>
	.max-h-half {
		max-height: 50vh;
	}
</style>
