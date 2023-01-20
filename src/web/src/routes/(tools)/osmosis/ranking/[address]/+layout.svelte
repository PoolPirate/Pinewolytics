<script lang="ts">
	import { page } from '$app/stores';
	import { getOsmosisWalletRanking } from '$lib/service/queries';
	import { onMount, setContext } from 'svelte';
	import RankingLayout from '../RankingLayout.svelte';
</script>

<RankingLayout>
	<nav slot="nav" class="flex flex-row justify-around gap-4">
		<a href="/osmosis/ranking">◀</a>
		<a href="./overview">Overview</a>
		<a href="./pools">Pools</a>
	</nav>

	<main slot="main">
		<slot />
	</main>
</RankingLayout>
