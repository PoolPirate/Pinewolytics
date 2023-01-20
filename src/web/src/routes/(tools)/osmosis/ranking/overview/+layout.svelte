<script lang="ts">
	import { goto } from '$app/navigation';
	import { onMount } from 'svelte';
	import RankingLayout from '../RankingLayout.svelte';

	onMount(() => {
		const urlParams = new URLSearchParams(window.location.search);
		const addr = urlParams.get('address');
		if (addr == null) {
			goto('/osmosis/ranking');
			return;
		}

		address = addr;
	});

	var address = '';
</script>

<RankingLayout>
	<nav slot="nav" class="flex flex-row justify-around gap-4">
		<a href="/osmosis/ranking">◀</a>
		<a href="./overview?address={address}">Overview</a>
		<a href="./pools?address={address}">Pools</a>
	</nav>

	<main slot="main">
		<slot />
	</main>
</RankingLayout>
