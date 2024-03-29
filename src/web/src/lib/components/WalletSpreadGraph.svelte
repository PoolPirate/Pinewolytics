<script lang="ts" context="module">
	export interface InitialWallet {
		address: string;
		amount: number | undefined;
	}
</script>

<script lang="ts">
	import { getInternalNetOSMOTransfers, getRelatedWallets } from '$lib/service/queries';
	import { writable } from 'svelte/store';
	import WalletGraph, { type DeveloperWallet } from './WalletGraph.svelte';
	import { browser } from '$app/environment';
	import type { OsmosisNetTransferDTO } from '$lib/models/DTOs/OsmosisDTOs';
	import Warning from '$lib/static/warning.svg';

	export let maxDepth: number;
	export let initialWallets: InitialWallet[] | null;

	export { clazz as class };
	let clazz: string;

	const depth = writable<number>(0);
	const wallets = writable<DeveloperWallet[]>([]);
	const transfers = writable<OsmosisNetTransferDTO[]>([]);

	const isLoading = writable<boolean>(true);
	const isFailed = writable<boolean>(false);

	$: startTracing(initialWallets);

	async function startTracing(initialWallets: InitialWallet[] | null) {
		try {
			if (initialWallets == null || !browser) {
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
		} catch (error) {
			isFailed.set(true);
		}
	}
</script>

{#if !$isFailed}
	<WalletGraph
		class={clazz}
		wallets={$wallets}
		transfers={$transfers}
		depth={$depth + 1}
		isLoading={$isLoading}
	/>
{:else}
	<div>
		<div
			class="text-center transparent-background grid grid-cols-1 place-items-center p-5 rounded-xl place-content-start"
		>
			<img class="w-64 p-8" src={Warning} alt="Loading" />
			<h1 class="font-bold text-xl">FlipsideCrypto API Unavailable</h1>
			<h2>Try again later :/</h2>
		</div>
	</div>
{/if}
