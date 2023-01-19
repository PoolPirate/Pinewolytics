<script lang="ts">
	import { writable } from 'svelte/store';
	import AddressList from './AddressList.svelte';

	export let pool: number;
	export let assets: string[] | null;
	export let rank: number;
	export let gammAmount: number;

	const color: string =
		rank == 1
			? 'gold'
			: rank == 2
			? 'silver'
			: rank == 3
			? 'bronze'
			: rank < 20
			? 'green'
			: rank < 50
			? 'blue'
			: rank < 100
			? 'yellow'
			: 'red';

	const expanded = writable<boolean>(false);
</script>

<div
	class="border my-2 flex flex-col justify-between border-gray-400 p-2 rounded-md cursor-pointer"
	on:click={() => expanded.update((x) => !x)}
	on:keydown={() => {}}
>
	<div class="relative">
		<div class="flex flex-row">
			<p class="rounded-md text-left" style="color: {color};">{pool}</p>
			{#if assets == null}
				<p />
			{:else}
				<AddressList {assets} />
			{/if}
		</div>

		<div class="absolute w-1/5 right-0 top-0 h-full flex items-center" style="color: {color};">
			<p class="text-center font-bold text-lg">
				#{rank}
			</p>
		</div>

		{#if !$expanded}
			<p class="text-center">▼</p>
		{:else}
			<p class="text-center">▲</p>
		{/if}
	</div>
	<div class:hidden={!$expanded}>
		<p>EXPANDED</p>
		<p>EXPANDED</p>
		<p>EXPANDED</p>
	</div>
</div>
