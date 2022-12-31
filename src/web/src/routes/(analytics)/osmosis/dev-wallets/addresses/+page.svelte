<script lang="ts">
	import WalletSpreadGraph, { type InitialWallet } from '$lib/components/WalletSpreadGraph.svelte';
	import { getDeveloperMintReceivers, getTotalDeveloperMintedOsmo } from '$lib/service/osmosislcd';
	import { onMount } from 'svelte';
	import { writable } from 'svelte/store';

	const initialWallets = writable<InitialWallet[] | null>(null);

	onMount(async () => {
		const totalOMSO = await getTotalDeveloperMintedOsmo();
		const devMintReceivers = await getDeveloperMintReceivers();
		const wallets = devMintReceivers.map((x) => {
			return {
				address: x.address,
				amount: totalOMSO * x.weight
			};
		});

		initialWallets.set(wallets);
	});
</script>

<WalletSpreadGraph maxDepth={10} initialWallets={$initialWallets} />
