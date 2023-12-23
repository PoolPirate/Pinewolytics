<script lang="ts">
	import type {
		OsmosisDenominatedAmountDTO,
		OsmosisTokenInfoDTO
	} from '$lib/models/DTOs/OsmosisDTOs';

	export let balance: OsmosisDenominatedAmountDTO;
	export let tokenInfo: OsmosisTokenInfoDTO;
</script>

<li class="flex flex-row items-center gap-2 text-lg font-bold">
	<p>
		{(Math.round((100 * balance.amount) / Math.pow(10, tokenInfo.exponent)) / 100).toLocaleString()}
	</p>
	<img
		class="h-11"
		src="https://app.osmosis.zone/tokens/generated/{tokenInfo.symbol.toLowerCase()}.svg"
		alt="token_symbol"
	/>
</li>
