import type { OsmosisFlowSankeyDTO, OsmosisIBCTransferDTO, OsmosisLPJoinDTO, OsmosisNetTransferDTO, OsmosisSwapDTO, OsmosisTransferDTO } from "$lib/models/OsmosisDTOs";

export async function getOSMOSwaps(addresses: string[]): Promise<OsmosisSwapDTO[]> {
    const response = await fetch('/Api/Osmosis/Swaps' + makeAddressParams(addresses));
    return (await response.json()) as OsmosisSwapDTO[];
}

export async function getOSMOTransfers(addresses: string[]): Promise<OsmosisTransferDTO[]> {
    const response = await fetch('/Api/Osmosis/Transfers' + makeAddressParams(addresses));
    return (await response.json()) as OsmosisTransferDTO[];
}

export async function getInternalNetOSMOTransfers(addresses: string[]): Promise<OsmosisNetTransferDTO[]> {
    const response = await fetch('/Api/Osmosis/InternalNetTransfers' + makeAddressParams(addresses));
    return (await response.json()) as OsmosisNetTransferDTO[];
}

export async function getExternalNetOSMOTransfers(addresses: string[]): Promise<OsmosisNetTransferDTO[]> {
    const response = await fetch('/Api/Osmosis/ExternalNetTransfers' + makeAddressParams(addresses));
    return (await response.json()) as OsmosisNetTransferDTO[];
}

export async function getOSMOIBCTransfers(addresses: string[]): Promise<OsmosisIBCTransferDTO[]> {
    const response = await fetch('/Api/Osmosis/IBCTransfers' + makeAddressParams(addresses));
    return (await response.json()) as OsmosisIBCTransferDTO[];
}

export async function getOSMOLPJoins(addresses: string[]): Promise<OsmosisLPJoinDTO[]> {
    const response = await fetch('/Api/Osmosis/LPJoins' + makeAddressParams(addresses));
    return (await response.json()) as OsmosisLPJoinDTO[];
}

export async function getOsmosisFlowSankey(address: string, currency: string): Promise<OsmosisFlowSankeyDTO> {
    const response = await fetch('/Api/Osmosis/FlowSankey?address=' + address + "&currency=" + currency);
    return (await response.json()) as OsmosisFlowSankeyDTO;
}

export async function getOsmosisDeveloperWalletsRecursive(depth: number): Promise<string[]> {
    const response = await fetch('/Api/Osmosis/DeveloperWallets?depth=' + depth);
    return (await response.json()) as string[];
}

export async function getRelatedWallets(addresses: string[]): Promise<string[]> {
    const response = await fetch('/Api/Osmosis/RelatedWallets' + makeAddressParams(addresses));
    return (await response.json()) as string[];
}

function makeAddressParams(addresses: string[]) {
    var params = "?addresses=";

    addresses.forEach((address) => {
        params += address;
        params += ","
    });

    return params.slice(0, params.length - 1)
}