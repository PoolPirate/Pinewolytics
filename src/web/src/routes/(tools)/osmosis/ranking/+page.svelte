<script lang="ts">
	import { goto } from '$app/navigation';
	import RankingLayout from './RankingLayout.svelte';
	import OsmosisLogo from '$lib/static/logo/osmosis.png';
	import InfoLogo from '$lib/static/logo/info.svg';

	var addressInput: string = localStorage.getItem('address') ?? '';

	function goToRanking() {
		localStorage.setItem('address', addressInput);
		goto('./ranking/' + addressInput + '/overview');
	}
</script>

<RankingLayout>
	<nav slot="nav" class="flex flex-row justify-between px-3 gap-4">
		<a href="/osmosis">Back Main Menu</a>
		<a href="./ranking/info"><img class="h-6" src={InfoLogo} alt="Info" /></a>
	</nav>
	<main slot="main" class="flex flex-col items-center text-center my-4 gap-4">
		<img alt="Osmosis Logo" src={OsmosisLogo} />
		<p class="text-xl font-bold">Enter Osmosis address</p>
		<input
			placeholder="osmo1"
			class="border w-full p-2 rounded-xl"
			type="text"
			bind:value={addressInput}
		/>
		<button on:click={goToRanking} class="font-bold bg-gray-400 border rounded-md p-2"
			>Show Your Ranking</button
		>
	</main>
</RankingLayout>

<style>
	main {
		min-width: 50vw;
	}

	img {
		max-height: 20vw;
	}
</style>
