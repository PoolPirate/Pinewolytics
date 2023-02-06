<script lang="ts">
	import WalletSpreadGraph, { type InitialWallet } from '$lib/components/WalletSpreadGraph.svelte';
	import { getDeveloperMintReceivers, getTotalDeveloperMintedOsmo } from '$lib/service/osmosislcd';
	import { onMount } from 'svelte';
	import { writable } from 'svelte/store';
	import Warning from '$lib/static/warning.svg';

	const initialWallets = writable<InitialWallet[] | null>(null);
	const loadingFailure = writable<boolean>(false);

	onMount(async () => {
		try {
			const totalOMSO = await getTotalDeveloperMintedOsmo();
			const devMintReceivers = await getDeveloperMintReceivers();
			const wallets = devMintReceivers.map((x) => {
				return {
					address: x.address,
					amount: totalOMSO * x.weight
				};
			});
			initialWallets.set(wallets);
		} catch (error) {
			loadingFailure.set(true);
		}
	});
</script>

{#if !$loadingFailure}
	<div class="grid grid-cols-1 w-full h-full">
		<WalletSpreadGraph class="h-full" maxDepth={10} initialWallets={$initialWallets} />
	</div>
{:else}
	<div
		class="text-center transparent-background grid grid-cols-1 place-items-center p-5 rounded-xl"
	>
		<img class="w-64 p-8" src={Warning} alt="Loading" />
		<h1 class="font-bold text-xl">FlipsideCrypto API Unavailable</h1>
		<h2>Try again later :/</h2>
	</div>
{/if}
