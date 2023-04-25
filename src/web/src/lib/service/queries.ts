import type {
	OsmosisFlowSankeyDTO,
	OsmosisIBCTransferDTO,
	OsmosisLPJoinDTO,
	OsmosisNetTransferDTO,
	OsmosisPoolInfoDTO,
	OsmosisProtoRevTransactionDTO,
	OsmosisSwapDTO,
	OsmosisTransferDTO,
	OsmosisWalletRankingDTO
} from '$lib/models/DTOs/OsmosisDTOs';
import type { SubscriptionValueDTO } from '$lib/models/SharedDTOs';
import type { QueryName } from './query-definitions';
import type { RealtimeFeedName } from './realtime-feed-definitions';
import type { RealtimeValueName } from './realtime-value-definitions';

export async function getQuerySrc(queryName: QueryName): Promise<string> {
	const response = await fetch('/Api/Query/Src/' + queryName);
	return (await response.json()).src; 
}

export async function getOSMOSwaps(addresses: string[]): Promise<OsmosisSwapDTO[]> {
	const response = await fetch('/Api/Osmosis/Swaps' + makeListParams("addresses", addresses));
	return (await response.json()) as OsmosisSwapDTO[];
}

export async function getOSMOTransfers(addresses: string[]): Promise<OsmosisTransferDTO[]> {
	const response = await fetch('/Api/Osmosis/Transfers' + makeListParams("addresses", addresses));
	return (await response.json()) as OsmosisTransferDTO[];
}

export async function getInternalNetOSMOTransfers(
	addresses: string[]
): Promise<OsmosisNetTransferDTO[]> {
	const response = await fetch('/Api/Osmosis/InternalNetTransfers' + makeListParams("addresses", addresses));
	return (await response.json()) as OsmosisNetTransferDTO[];
}

export async function getExternalNetOSMOTransfers(
	addresses: string[]
): Promise<OsmosisNetTransferDTO[]> {
	const response = await fetch('/Api/Osmosis/ExternalNetTransfers' + makeListParams("addresses", addresses));
	return (await response.json()) as OsmosisNetTransferDTO[];
}

export async function getOSMOIBCTransfers(addresses: string[]): Promise<OsmosisIBCTransferDTO[]> {
	const response = await fetch('/Api/Osmosis/IBCTransfers' + makeListParams("addresses", addresses));
	return (await response.json()) as OsmosisIBCTransferDTO[];
}

export async function getOSMOLPJoins(addresses: string[]): Promise<OsmosisLPJoinDTO[]> {
	const response = await fetch('/Api/Osmosis/LPJoins' + makeListParams("addresses", addresses));
	return (await response.json()) as OsmosisLPJoinDTO[];
}

export async function getOsmosisFlowSankey(
	address: string,
	currency: string
): Promise<OsmosisFlowSankeyDTO> {
	const response = await fetch(
		'/Api/Osmosis/FlowSankey?address=' + address + '&currency=' + currency
	);
	return (await response.json()) as OsmosisFlowSankeyDTO;
}

export async function getOsmosisDeveloperWalletsRecursive(depth: number): Promise<string[]> {
	const response = await fetch('/Api/Osmosis/DeveloperWallets?depth=' + depth);
	return (await response.json()) as string[];
}

export async function getRelatedWallets(addresses: string[]): Promise<string[]> {
	const response = await fetch('/Api/Osmosis/RelatedWallets' + makeListParams("addresses", addresses));
	return (await response.json()) as string[];
}

export async function getOsmosisWalletRanking(address: string): Promise<OsmosisWalletRankingDTO | null> {
	const response = await fetch("/Api/Osmosis/WalletRankings/" + address);

	if (response.ok) {
		return (await response.json()) as OsmosisWalletRankingDTO;
	}
	if (response.status == 404) {
		return null;
	}

	throw response;
}

export async function getOsmosisPoolInfosAsync(poolIds: number[]): Promise<OsmosisPoolInfoDTO[]> {
	const response = await fetch("/Api/Osmosis/PoolInfos/"  + makeListParams("poolIds", poolIds));
	return (await response.json()) as OsmosisPoolInfoDTO[];
}

export async function getOsmosisProtoRevTransactions(address: string): Promise<OsmosisProtoRevTransactionDTO[]> {
	const response = await fetch("/Api/Osmosis/ProtoRevTx/" + address);
	return (await response.json()) as OsmosisProtoRevTransactionDTO[];
}

export async function getICNSNameByAddress(address: string): Promise<string | null> {
	const response = await fetch("/Api/ICNS/Reverse/" + address);
	
	if (!response.ok) {
		return null;
	}

	return await response.text();
}

export async function getQueryValue(queryName: QueryName) {
	const response = await fetch("/Api/Subscriptions/Query/" + queryName);
	return (await response.json() as SubscriptionValueDTO);
}

export async function getRealtimeValueValue(realtimeValueName: RealtimeValueName) {
	const response = await fetch("/Api/Subscriptions/RealtimeValue/" + realtimeValueName);
	return (await response.json() as SubscriptionValueDTO);
}

export async function getRealtimeFeedValue(realtimeFeedName: RealtimeFeedName) {
	const response = await fetch("/Api/Subscriptions/RealtimeFeed/" + realtimeFeedName);
	return (await response.json() as SubscriptionValueDTO);
}

function makeListParams(paramName: string, values: any[]) {
	var params = '?' + paramName + '=';

	values.forEach((values) => {
		params += values;
		params += ',';
	});

	return params.slice(0, params.length - 1);
}
