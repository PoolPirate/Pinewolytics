<script lang="ts">
	import WalletSpreadGraph from "$lib/components/WalletSpreadGraph.svelte";
	import LoadingSpinner from "$lib/LoadingSpinner.svelte";
	import { getDeveloperMintReceivers, getTotalDeveloperMintedOsmo } from "$lib/service/osmosislcd";

    const totalOSMOPromise = getTotalDeveloperMintedOsmo();
    const devMintReceiversPromise = getDeveloperMintReceivers();
    const initialWalletsPromise = Promise.all([totalOSMOPromise, devMintReceiversPromise]).then(results => {
        const totalOMSO = results[0];
        const devMintReceivers = results[1];

        return devMintReceivers.map(x => {
            return {
                address: x.address, 
                amount: totalOMSO * x.weight
            };
        })
    });
</script>

{#await initialWalletsPromise}
    <LoadingSpinner></LoadingSpinner>
{:then initialWallets} 
    <WalletSpreadGraph maxDepth={10} initialWallets={initialWallets}></WalletSpreadGraph>
{/await}

