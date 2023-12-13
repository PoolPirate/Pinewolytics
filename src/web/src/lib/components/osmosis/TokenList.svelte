<script lang="ts">
	import type {
		OsmosisDenominatedAmountDTO,
		OsmosisTokenInfoDTO
	} from '$lib/models/DTOs/OsmosisDTOs';

	export let balance: OsmosisDenominatedAmountDTO[] | null;
	export let allTokenInfos: OsmosisTokenInfoDTO[] | null;
</script>

{#if balance != null && allTokenInfos != null}
	<ul class="flex flex-row justify-around">
		{#each balance as balance, i}
			<li class="flex flex-row items-center gap-2 text-lg font-bold">
				<p>
					{Math.round(
						(100 * balance.amount) /
							Math.pow(10, allTokenInfos.find((x) => x.denom == balance.denom)?.exponent ?? 1)
					) / 100}
				</p>
				<img
					class="h-11"
					src="https://app.osmosis.zone/tokens/generated/{allTokenInfos
						.find((x) => x.denom == balance.denom)
						?.symbol?.toLowerCase()}.svg"
					alt="token_symbol"
				/>
			</li>
		{/each}
	</ul>
{:else}
	<p>Loading...</p>
{/if}
