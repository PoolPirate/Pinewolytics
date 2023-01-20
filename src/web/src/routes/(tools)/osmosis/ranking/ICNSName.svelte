<script lang="ts">
	import { getICNSNameByAddress } from '$lib/service/queries';

	export let address: string;

	var loadingPromise: Promise<string | null> = null!;

	$: loadingPromise = loadICNSName(address);
	async function loadICNSName(address: string): Promise<string | null> {
		return await getICNSNameByAddress(address);
	}
</script>

<div class="flex flex-row items-center gap-4 border border-gray-400 p-3 rounded-md bg-gray-200">
	<b>ICNS:</b>
	<div class="w-full text-right">
		{#await loadingPromise}
			<p>Looking for ICNS Name</p>
		{:then name}
			{#if name == null}
				<p>No ICNS Name Found</p>
			{:else}
				<p>{name}.OSMO</p>
			{/if}
		{/await}
	</div>
</div>
