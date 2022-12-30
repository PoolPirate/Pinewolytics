<script lang="ts">
	import { getOsmosisFlowSankey } from '$lib/service/queries';
	import WalletSankey from './WalletSankey.svelte';

	const wallet: string = 'osmo13f7pp05xvn432zrywly70anq0ktsfsmyhzumny';
	const currency: string = 'uosmo';

	const sankeyPromise = getOsmosisFlowSankey(wallet, currency);
</script>

<h1>Wallet Tracker</h1>

{#await sankeyPromise}
	<p>Loading</p>
{:then sankeyData}
	<WalletSankey parameters={sankeyData} />
{:catch e}
	<p>Error</p>
{/await}
