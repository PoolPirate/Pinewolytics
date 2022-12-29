import type { OsmosisFlowSankeyDTO, OsmosisIBCTransferDTO, OsmosisLPJoinDTO, OsmosisNetTransferDTO, OsmosisSwapDTO, OsmosisTransferDTO } from "$lib/models/OsmosisDTOs";

export async function getOSMOSwaps(addresses: string[]): Promise<OsmosisSwapDTO[]> {
    var params = "?";

    addresses.forEach((address) => {
        params += 'addresses=' + address + '&';
    });

    const response = await fetch('http://localhost:4565/Api/Osmosis/Swaps' + params);
    return (await response.json()) as OsmosisSwapDTO[];
}

export async function getOSMOTransfers(addresses: string[]): Promise<OsmosisTransferDTO[]> {
    var params = "?";

    addresses.forEach((address) => {
        params += 'addresses=' + address + '&';
    });

    const response = await fetch('http://localhost:4565/Api/Osmosis/Transfers' + params);
    return (await response.json()) as OsmosisTransferDTO[];
}

export async function getInternalNetOSMOTransfers(addresses: string[]): Promise<OsmosisNetTransferDTO[]> {
    var params = "?";

    addresses.forEach((address) => {
        params += 'addresses=' + address + '&';
    });

    const response = await fetch('http://localhost:4565/Api/Osmosis/InternalNetTransfers?=' + params);
    return (await response.json()) as OsmosisNetTransferDTO[];
}

export async function getExternalNetOSMOTransfers(addresses: string[]): Promise<OsmosisNetTransferDTO[]> {
    var params = "?";

    addresses.forEach((address) => {
        params += 'addresses=' + address + '&';
    });

    const response = await fetch('http://localhost:4565/Api/Osmosis/ExternalNetTransfers?=' + params);
    return (await response.json()) as OsmosisNetTransferDTO[];
}

export async function getOSMOIBCTransfers(addresses: string[]): Promise<OsmosisIBCTransferDTO[]> {
    var params = "?";

    addresses.forEach((address) => {
        params += 'addresses=' + address + '&';
    });

    const response = await fetch('http://localhost:4565/Api/Osmosis/IBCTransfers' + params);
    return (await response.json()) as OsmosisIBCTransferDTO[];
}

export async function getOSMOLPJoins(addresses: string[]): Promise<OsmosisLPJoinDTO[]> {
    var params = "?";

    addresses.forEach((address) => {
        params += 'addresses=' + address + '&';
    });

    const response = await fetch('http://localhost:4565/Api/Osmosis/LPJoins' + params);
    return (await response.json()) as OsmosisLPJoinDTO[];
}

export async function getOsmosisFlowSankey(address: string, currency: string): Promise<OsmosisFlowSankeyDTO> {
    const response = await fetch('http://localhost:4565/Api/Osmosis/FlowSankey?address=' + address + "&currency=" + currency);
    return (await response.json()) as OsmosisFlowSankeyDTO;
}

export async function getOsmosisDeveloperWalletsRecursive(depth: number): Promise<string[]> {
    const response = await fetch('http://localhost:4565/Api/Osmosis/DeveloperWallets?depth=' + depth);
    return (await response.json()) as string[];
}

export async function getRelatedWallets(addresses: string[]): Promise<string[]> {
    var params = "?";

    addresses.forEach((address) => {
        params += 'addresses=' + address + '&';
    });

    const response = await fetch('http://localhost:4565/Api/Osmosis/RelatedWallets' + params);
    return (await response.json()) as string[];
}