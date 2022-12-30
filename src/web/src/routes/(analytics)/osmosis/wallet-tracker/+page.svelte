<script lang="ts">
	import { getOsmosisFlowSankey } from '$lib/service/queries';
	import WalletSankey from './WalletSankey.svelte';

	const wallet: string = 'osmo13f7pp05xvn432zrywly70anq0ktsfsmyhzumny';
	const currency: string = 'uosmo';

	const sankeyPromise = getOsmosisFlowSankey(wallet, currency);
</script>

<h1>Dev Wallet Tracker</h1>

<h1>Osmosis Tokenomics</h1>
<img src="https://miro.medium.com/max/720/0*hjY8neGuW_2qxKTa" />

{#await sankeyPromise}
	<p>Loading</p>
{:then sankeyData}
	<WalletSankey parameters={sankeyData} />
{:catch e}
	<p>Error</p>
{/await}
