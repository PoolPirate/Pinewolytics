import type { OsmosisProtoRevTransactionDTO } from "$lib/models/DTOs/OsmosisDTOs";

export enum RealtimeFeedName {
    OsmosisProtoRevTransactions = "Osmosis-ProtoRev-Tx-Feed",
}

export const realtimeFeedTypes = {
    [RealtimeFeedName.OsmosisProtoRevTransactions]: null! as OsmosisProtoRevTransactionDTO,
};

export const realtimeFeedLengths = {
    [RealtimeFeedName.OsmosisProtoRevTransactions]: 20,
};