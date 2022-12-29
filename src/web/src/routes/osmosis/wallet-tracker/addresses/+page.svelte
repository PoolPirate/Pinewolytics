 <script lang="ts">
	import type { InitialWallet } from "$lib/components/WalletSpreadGraph.svelte";
	import WalletSpreadGraph from "$lib/components/WalletSpreadGraph.svelte";


    const wallet: InitialWallet[] = [
        {
            address: "osmo13f7pp05xvn432zrywly70anq0ktsfsmyhzumny",
            amount: 0
        }
    ];

</script>
 
 <WalletSpreadGraph maxDepth={10} initialWallets={wallet}></WalletSpreadGraph>