<script lang="ts">
	import RankingLayout from '../RankingLayout.svelte';
	import InfoLogo from '$lib/static/logo/info.svg';
	import FlipsideLogo from '$lib/static/logo/flipside.png';
</script>

<RankingLayout>
	<nav slot="nav" class="flex flex-row justify-between px-3 gap-4">
		<a href="/osmosis/ranking">Address Selection</a>
		<a href="/osmosis/ranking/info"><img class="h-6" src={InfoLogo} alt="Info" /></a>
	</nav>
	<main slot="main" class="flex flex-col items-center text-center mt-4 gap-4">
		<a
			href="https://flipsidecrypto.xyz/"
			target="_blank"
			rel="noreferrer"
			class="flex flex-row items-center gap-4 border rounded-lg p-2 border-gray-700 border-2 bg-gray-400"
		>
			<p class="text-xl">Data Provided By FlipsideCrypto</p>
			<img alt="FlipsideCrypto Logo" class="h-16" src={FlipsideLogo} />
		</a>

		<div class="flex flex-row items-center w-full justify-end font-bold ">
			<p>Made with</p>
			<b class="mx-2 text-red-500 text-4xl">❤</b>
			<p>by Playwo</p>
		</div>
	</main>
</RankingLayout>
