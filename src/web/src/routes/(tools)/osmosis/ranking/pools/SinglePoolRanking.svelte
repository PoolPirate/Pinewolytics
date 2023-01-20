<script lang="ts">
	import { writable } from 'svelte/store';
	import AddressList from '../AddressList.svelte';
	import externalLinkLogo from '$lib/static/logo/external-link.svg';

	export let pool: number;
	export let assets: string[] | null;
	export let rank: number;
	export let gammAmount: number;

	var rankColor =
		rank < 100
			? 'gold'
			: rank < 200
			? 'silver'
			: rank < 300
			? 'orange'
			: rank < 500
			? 'green'
			: rank < 1000
			? 'skyblue'
			: rank < 5000
			? 'blue'
			: rank < 10000
			? 'purple'
			: 'red';

	const expanded = writable<boolean>(false);
</script>

<div
	class="border my-2 flex flex-col justify-between border-gray-700 bg-gray-400 p-2 rounded-md"
	on:click={() => expanded.update((x) => !x)}
	on:keydown={() => {}}
>
	<div class="flex flex-row justify-between items-center">
		<a
			class="flex flex-col justify-center w-1/12 text-center"
			href="https://app.osmosis.zone/pool/{pool}"
			target="_blank"
			rel="noreferrer"
		>
			<p>{pool}</p>
			<img class="h-4" src={externalLinkLogo} alt="Open Pool Site" />
		</a>

		<AddressList class="w-3/12" {assets} />
		<p class="w-4/12">{Math.round(gammAmount / Math.pow(10, 18))}</p>
		<p class="text-lg font-bold w-4-12" style="color: {rankColor}">#{rank}</p>
	</div>

	<div class:hidden={true}>
		<p>EXPANDED</p>
		<p>EXPANDED</p>
		<p>EXPANDED</p>
	</div>
</div>
