<script lang="ts">
	import {
		sumIBCTransferVolume,
		sumSwapFromVolume,
		sumSwapToVolume,
		sumTransferVolume
	} from '$lib/service/decode';
	import {
		getOSMOIBCTransfers,
		getOSMOLPJoins,
		getOsmosisDeveloperWalletsRecursive,
		getOSMOSwaps,
		getOSMOTransfers
	} from '$lib/service/queries';
	import { onMount } from 'svelte';

	const devAddressesPromise = getOsmosisDeveloperWalletsRecursive(5);

	const transfersPromise = devAddressesPromise.then(async (addresses) => {
		return {
			addresses,
			transfers: await getOSMOTransfers(addresses)
		};
	});
	const ibcTransfersPromise = devAddressesPromise.then(async (addresses) => {
		return {
			addresses,
			ibcTransfers: await getOSMOIBCTransfers(addresses)
		};
	});
	const swapsPromise = devAddressesPromise.then(async (addresses) => {
		return {
			addresses,
			swaps: await getOSMOSwaps(addresses)
		};
	});
	const lpJoinsPromise = devAddressesPromise.then(async (addresses) => {
		return {
			addresses,
			lpJoins: await getOSMOLPJoins(addresses)
		};
	});

	function onlyUnique(value: string, index: number, self: string[]) {
		return self.indexOf(value) === index;
	}
</script>

<h1>Dev Wallet Tracker</h1>

<h1>Osmosis Tokenomics</h1>
<img src="https://miro.medium.com/max/720/0*hjY8neGuW_2qxKTa" />

{#await devAddressesPromise}
	<p>Tracing dev addresses...</p>
{:then devAddresses}
	<p>Found a network of {devAddresses.length} addresses</p>
{:catch error}
	<p>Failed loading</p>
{/await}

<br />
<hr />
<br />

{#await transfersPromise}
	<p>Loading OSMO transfers</p>
{:then { addresses, transfers }}
	<p>Found {transfers.length} transfers from and to it</p>
	<p>{sumTransferVolume(transfers.filter((x) => addresses.includes(x.sender)))} Outwards</p>
	<p>{sumTransferVolume(transfers.filter((x) => !addresses.includes(x.sender)))} Inwards</p>
{:catch error}
	<p>Failed loading</p>
{/await}

<br />
<hr />
<br />

{#await ibcTransfersPromise}
	<p>Loading IBC OSMO transfers</p>
{:then { addresses, ibcTransfers }}
	<p>Found {ibcTransfers.length} IBC transfers from and to it</p>
	<p>{sumIBCTransferVolume(ibcTransfers.filter((x) => addresses.includes(x.sender)))} Outwards</p>
	<p>{sumIBCTransferVolume(ibcTransfers.filter((x) => !addresses.includes(x.sender)))} Inwards</p>
{:catch error}
	<p>Failed loading</p>
{/await}

<br />
<hr />
<br />

{#await swapsPromise}
	<p>Loading Swaps</p>
{:then { addresses, swaps }}
	<p>Found {swaps.length} swaps from and to OSMO</p>
	<p>{sumSwapFromVolume(swaps.filter((x) => x.fromCurrency == 'uosmo'))} To other assets</p>
	<p>{sumSwapToVolume(swaps.filter((x) => x.toCurrency == 'uosmo'))} From other assets</p>
{:catch error}
	<p>Failed loading</p>
{/await}

<br />
<hr />
<br />

{#await lpJoinsPromise}
	<p>Loading LP Joins</p>
{:then { addresses, lpJoins }}
	<p>Found {lpJoins.length} LP positions from and to it</p>
{:catch error}
	<p>Failed loading</p>
{/await}
