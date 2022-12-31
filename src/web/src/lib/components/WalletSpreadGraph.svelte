<script lang="ts" context="module">
	export interface InitialWallet {
		address: string;
		amount: number | undefined;
	}
</script>

<script lang="ts">
	import { getInternalNetOSMOTransfers, getRelatedWallets } from '$lib/service/queries';
	import { writable } from 'svelte/store';
	import type { OsmosisNetTransferDTO } from '$lib/models/OsmosisDTOs';
	import WalletGraph, { type DeveloperWallet } from './WalletGraph.svelte';

	export let maxDepth: number;
	export let initialWallets: InitialWallet[] | null;

	const depth = writable<number>(0);
	const wallets = writable<DeveloperWallet[]>([]);
	const transfers = writable<OsmosisNetTransferDTO[]>([]);

	const isLoading = writable<boolean>(true);

	$: startTracing(initialWallets);

	async function startTracing(initialWallets: InitialWallet[] | null) {
		if (initialWallets == null) {
			return;
		}

		const initialAddresses = initialWallets.map((x) => x.address);

		const totalAddresses = [...initialAddresses];

		wallets.set(
			initialWallets.map((wallet) => {
				return {
					address: wallet.address,
					level: 0,
					amount: wallet.amount == null ? 0 : wallet.amount
				};
			})
		);

		for (let i = 1; i < maxDepth; i++) {
			const relatedAddresses = await getRelatedWallets(totalAddresses);

			if (relatedAddresses.length == 0) {
				break;
			}

			const devWallets = $wallets;

			relatedAddresses.forEach((address) => {
				totalAddresses.push(address);

				devWallets.push({
					address: address,
					level: i,
					amount: 0
				});
			});

			depth.set(i);
			wallets.set(devWallets);
		}

		transfers.set(await getInternalNetOSMOTransfers(totalAddresses));
		isLoading.set(false);
	}
</script>

<WalletGraph wallets={$wallets} transfers={$transfers} depth={$depth + 1} isLoading={$isLoading} />

<style>
</style>
