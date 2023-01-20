<script lang="ts">
	import { writable } from 'svelte/store';
	import AddressList from '../AddressList.svelte';
	import externalLinkLogo from '$lib/static/logo/external-link.svg';
	import Rank from '../Rank.svelte';

	export let pool: number;
	export let assets: string[] | null;
	export let rank: number;
	export let gammAmount: number;

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
		<p class="w-5/12">{Math.round(gammAmount / Math.pow(10, 16)) / 100} Shares</p>
		<Rank {rank} class="text-lg font-bold w-4-12" />
	</div>

	<div class:hidden={true}>
		<p />
	</div>
</div>
