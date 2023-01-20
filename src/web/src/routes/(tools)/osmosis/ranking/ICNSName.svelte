<script lang="ts">
	import { getICNSNameByAddress } from '$lib/service/queries';
	import Loading from '$lib/static/loading.svg';

	export let address: string;

	var loadingPromise: Promise<string | null> = null!;

	$: loadingPromise = loadICNSName(address);
	async function loadICNSName(address: string): Promise<string | null> {
		return await getICNSNameByAddress(address);
	}
</script>

<div class="flex flex-row items-center gap-4 border border-gray-400 p-3 rounded-md bg-gray-200">
	<b>ICNS:</b>
	<div class="w-full flex flex-row justify-end">
		{#await loadingPromise}
			<img class="h-6 invert" src={Loading} alt="Loading" />
		{:then name}
			{#if name == null}
				<p>No ICNS Name Found</p>
			{:else}
				<p>{name}.OSMO</p>
			{/if}
		{:catch}
			<p class="text-red-500 font-bold">Error</p>
		{/await}
	</div>
</div>

<style>
	.invert {
		filter: invert(-1);
	}
</style>
