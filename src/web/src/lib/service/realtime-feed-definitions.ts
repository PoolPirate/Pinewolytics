import type { OsmosisTransactionDTO } from "$lib/models/DTOs/OsmosisDTOs";

export enum RealtimeFeedName {
    OsmosisProtoRevTransactions = "Osmosis-ProtoRev-Tx-Feed",
}

export const realtimeFeedTypes = {
    [RealtimeFeedName.OsmosisProtoRevTransactions]: null! as OsmosisTransactionDTO,
};

export const realtimeFeedLengths = {
    [RealtimeFeedName.OsmosisProtoRevTransactions]: 10,
};