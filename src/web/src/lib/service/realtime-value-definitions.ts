import type { OptimismGasPricesDTO } from "$lib/models/DTOs/OptimismDTO";
import type { OsmosisDenominatedAmountDTO, OsmosisEpochInfoDTO } from "$lib/models/DTOs/OsmosisDTOs";
import type { MarketDataDTO } from "$lib/models/SharedDTOs";

export enum RealtimeValueName {
    OptimismMarketData = "Optimism-MarketData",
    OptimismBlockHeight = "Optimism-Block-Height",
    OptimismGasPrices = "Optimism-Gas-Prices",

    LunaBlockHeight = "Luna-Block-Height",
    LunaBlockTimestamp = "Luna-Block-Timestamp",
    LunaPrice = "Luna-Price",
    LunaTotalSupply = "Luna-Total-Supply",
    LunaCirculatingSupply = "Luna-Circulating-Supply",

    OsmosisTotalSupply = "Osmosis-Total-Supply",
    OsmosisCommunityPoolBalance = "Osmosis-Community-Pool-Balance",
    OsmosisEpochInfo = "Osmosis-Epoch-Info",
    OsmosisTotalSuperfluidDelegations = "Osmosis-Total-Superfluid-Delegations",
    OsmosisTotalProtoRevRevenue = "Osmosis-Total-ProtoRev-Revenue",
    OsmosisProtoRevIsEnabled = "Osmosis-ProtoRev-Enabled",
    OsmosisProtoRevAdminAddress = "Osmosis-ProtoRev-Admin-Address",
    OsmosisProtoRevDeveloperAddress = "Osmosis-ProtoRev-Developer-Address"
}

export const realtimeValueTypes = {
    [RealtimeValueName.OptimismMarketData]: null! as MarketDataDTO,
    [RealtimeValueName.OptimismBlockHeight]: null! as number,
    [RealtimeValueName.OptimismGasPrices]: null! as OptimismGasPricesDTO,

    [RealtimeValueName.LunaBlockHeight]: null! as number,
    [RealtimeValueName.LunaBlockTimestamp]: null! as string,
    [RealtimeValueName.LunaPrice]: null! as number,
    [RealtimeValueName.LunaTotalSupply]: null! as number,
    [RealtimeValueName.LunaCirculatingSupply]: null! as number,

    [RealtimeValueName.OsmosisTotalSupply]: null! as number,
    [RealtimeValueName.OsmosisCommunityPoolBalance]: null! as number,
    [RealtimeValueName.OsmosisEpochInfo]: null! as OsmosisEpochInfoDTO,
    [RealtimeValueName.OsmosisTotalSuperfluidDelegations]: null! as number,
    [RealtimeValueName.OsmosisTotalProtoRevRevenue]: null! as OsmosisDenominatedAmountDTO[],
    [RealtimeValueName.OsmosisProtoRevIsEnabled]: null! as boolean,
    [RealtimeValueName.OsmosisProtoRevAdminAddress]: null! as string,
    [RealtimeValueName.OsmosisProtoRevDeveloperAddress]: null! as string
};