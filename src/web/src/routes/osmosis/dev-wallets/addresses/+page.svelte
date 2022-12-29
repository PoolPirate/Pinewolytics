<script lang="ts">
	import { getInternalNetOSMOTransfers, getOsmosisDeveloperWalletsRecursive, getRelatedWallets } from "$lib/service/queries";
	import DevWalletsGraph from "./DevWalletsGraph.svelte";
    import type { DeveloperWallet } from "./DevWalletsGraph.svelte";
	import { writable } from "svelte/store";
	import { onMount } from "svelte";
	import type { OsmosisNetTransferDTO } from "$lib/models/OsmosisDTOs";
	import { getDeveloperMintReceivers, getTotalDeveloperMintedOsmo } from "$lib/service/osmosislcd";

    const depth = 5;

    const wallets = writable<DeveloperWallet[]>([]);
    const transfers = writable<OsmosisNetTransferDTO[]>([]);

    onMount(async () => {
        const totalOSMO = await getTotalDeveloperMintedOsmo();
        const devMintReceivers = await getDeveloperMintReceivers();
        
        const devAddresses = devMintReceivers.map(x => x.address);
        const totalAddresses = [...devAddresses];

        wallets.set(devMintReceivers.map(wallet => {
            return {
                address: wallet.address,
                level: 0,
                amount: wallet.weight * totalOSMO
            }
        }));

        for(let i = 1; i < depth; i++) {
            const relatedAddresses = await getRelatedWallets(totalAddresses);
            const devWallets = $wallets;

            relatedAddresses.forEach(address => {
                totalAddresses.push(address);

                devWallets.push({
                    address: address,
                    level: i,
                    amount: 0
                });
            });

            wallets.set(devWallets);
        }

        transfers.set(await getInternalNetOSMOTransfers(totalAddresses));


    });
</script>

<DevWalletsGraph wallets={$wallets} transfers={$transfers} depth={depth}></DevWalletsGraph>

<style>

</style>