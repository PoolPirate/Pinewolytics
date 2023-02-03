import type { OsmosisEpochInfoDTO } from "$lib/models/DTOs/OsmosisDTOs";
import { developerMintShare, premintAmount } from "$lib/utils/OsmosisChainParams";


export async function getDeveloperMintReceivers(): Promise<{ address: string; weight: number }[]> {
	const response = await fetch('https://lcd-osmosis.imperator.co/osmosis/mint/v1beta1/params');
	const result = await response.json();

	const wallets: { address: string; weight: number }[] =
		result.params.weighted_developer_rewards_receivers;

	return wallets;
}

export async function getTotalMintedOsmo() {
	const response = await fetch('https://lcd-osmosis.imperator.co/cosmos/bank/v1beta1/supply/uosmo');
	const result = await response.json();

	return result.amount.amount / Math.pow(10, 6);
}

export async function getTotalDeveloperMintedOsmo() {
	const response = await fetch('https://lcd-osmosis.imperator.co/cosmos/bank/v1beta1/supply/uosmo');
	const result = await response.json();

	return developerMintShare * (result.amount.amount / Math.pow(10, 6) - premintAmount);
}

export async function getEpochInfosAsync(): Promise<OsmosisEpochInfoDTO[]> {
	const reponse = await fetch("https://lcd.osmosis.zone/osmosis/epochs/v1beta1/epochs");
	const results = await reponse.json();
	console.log(results);

	return results.epochs.map((result: any) => {
		return {
			currentEpoch: Number.parseInt(result.current_epoch),
			currentEpochStartHeight: Number.parseInt(result.current_epoch_start_height),
			identifier: result.identifier,
			startTime: new Date(result.start_time),
			duration: result.duration,
			currentEpochStartTime:  new Date(result.current_epoch_start_time)
		};
	});
}