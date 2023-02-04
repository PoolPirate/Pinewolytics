import type { OptimismGasPricesDTO } from "$lib/models/DTOs/OptimismDTO";
import type { OsmosisEpochInfoDTO } from "$lib/models/DTOs/OsmosisDTOs";
import type { MarketDataDTO } from "$lib/models/SharedDTOs";

export enum RealtimeValueName {
    OsmosisTotalSupply = "Osmosis-Total-Supply",
    OsmosisCommunityPoolBalance = "Osmosis-Community-Pool-Balance",
    OsmosisEpochInfo = "Osmosis-Epoch-Info",

    OptimismMarketData = "Optimism-MarketData",
    OptimismBlockHeight = "Optimism-Block-Height",
    OptimismGasPrices = "Optimism-Gas-Prices",

    LunaBlockHeight = "Luna-Block-Height",
    LunaBlockTimestamp = "Luna-Block-Timestamp",
    LunaPrice = "Luna-Price",
    LunaTotalSupply = "Luna-Total-Supply",
    LunaCirculatingSupply = "Luna-Circulating-Supply"
}

export const realtimeValueTypes = {
    [RealtimeValueName.OsmosisTotalSupply]: null! as number,
    [RealtimeValueName.OsmosisCommunityPoolBalance]: null! as number,
    [RealtimeValueName.OsmosisEpochInfo]: null! as OsmosisEpochInfoDTO,

    [RealtimeValueName.OptimismMarketData]: null! as MarketDataDTO,
    [RealtimeValueName.OptimismBlockHeight]: null! as number,
    [RealtimeValueName.OptimismGasPrices]: null! as OptimismGasPricesDTO,

    [RealtimeValueName.LunaBlockHeight]: null! as number,
    [RealtimeValueName.LunaBlockTimestamp]: null! as string,
    [RealtimeValueName.LunaPrice]: null! as number,
    [RealtimeValueName.LunaTotalSupply]: null! as number,
    [RealtimeValueName.LunaCirculatingSupply]: null! as number,
};