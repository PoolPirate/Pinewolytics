import type {
	OsmosisDelegateDTO,
	OsmosisIBCTransferDTO,
	OsmosisLPExitDTO,
	OsmosisLPJoinDTO,
	OsmosisSwapDTO,
	OsmosisTransferDTO,
	OsmosisUndelegateDTO
} from '$lib/models/DTOs/OsmosisDTOs';

export function netSwapBalanceChange(addresses: string[], swaps: OsmosisSwapDTO[]) {
	const fromOsmoTotal = swaps
		.filter((x) => x.fromCurrency == 'uosmo')
		.reduceRight((p, swap, _) => p + swap.fromAmount, 0);

	const toOsmoTotal = swaps
		.filter((x) => x.toCurrency == 'uosmo')
		.reduceRight((p, swap, _) => p + swap.fromAmount, 0);

	return toOsmoTotal - fromOsmoTotal;
}

export function netTransferBalanceChange(addresses: string[], swaps: OsmosisTransferDTO[]) {
	const sentAmount = swaps
		.filter((x) => addresses.includes(x.sender))
		.reduceRight((p, swap, _) => p + swap.amount, 0);

	const receivedAmount = swaps
		.filter((x) => addresses.includes(x.receiver))
		.reduceRight((p, swap, _) => p + swap.amount, 0);

	return receivedAmount - sentAmount;
}

export function netIBCTransferBalanceChange(addresses: string[], swaps: OsmosisIBCTransferDTO[]) {
	const sentAmount = swaps
		.filter((x) => addresses.includes(x.sender))
		.reduceRight((p, swap, _) => p + swap.amount, 0);

	const receivedAmount = swaps
		.filter((x) => addresses.includes(x.receiver))
		.reduceRight((p, swap, _) => p + swap.amount, 0);

	return receivedAmount - sentAmount;
}

export function sumTransferVolume(transfers: OsmosisTransferDTO[]) {
	return transfers.reduce((last, curr, _) => last + curr.amount, 0);
}

export function sumIBCTransferVolume(ibcTransfers: OsmosisIBCTransferDTO[]) {
	return ibcTransfers.reduce((last, curr, _) => last + curr.amount, 0);
}

export function sumSwapFromVolume(swaps: OsmosisSwapDTO[]) {
	return swaps.reduce((last, curr, _) => last + curr.fromAmount, 0);
}

export function sumSwapToVolume(swaps: OsmosisSwapDTO[]) {
	return swaps.reduce((last, curr, _) => last + curr.toAmount, 0);
}

export function sumLPJoinVolume(lpJoins: OsmosisLPJoinDTO[]) {
	return lpJoins.reduce((last, curr, _) => last + curr.amount, 0);
}

export function sumLPExitVolume(lpJoins: OsmosisLPExitDTO[]) {
	return lpJoins.reduce((last, curr, _) => last + curr.amount, 0);
}

export function sumDelegateVolume(deleagtes: OsmosisDelegateDTO[]) {
	return deleagtes.reduce((last, curr, _) => last + curr.amount, 0);
}

export function sumUndelegateVolume(undelegates: OsmosisUndelegateDTO[]) {
	return undelegates.reduce((last, curr, _) => last + curr.amount, 0);
}
