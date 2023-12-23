<script lang="ts">
	import type {
		OsmosisDenominatedAmountDTO,
		OsmosisTokenInfoDTO
	} from '$lib/models/DTOs/OsmosisDTOs';
	import TokenInfo from './TokenInfo.svelte';

	export let balance: OsmosisDenominatedAmountDTO[] | null;
	export let allTokenInfos: OsmosisTokenInfoDTO[] | null;

	function shouldIncludeToken(amount: number, denom: string) {
		var tokenInfo = allTokenInfos?.find((x) => x.denom == denom);

		if (tokenInfo == null) {
			return false;
		}

		return (tokenInfo.price * amount) / Math.pow(10, tokenInfo.exponent) > 100;
	}

	function forceGetTokenInfo(denom: string) {
		return allTokenInfos!.find((x) => x.denom == denom)!;
	}
</script>

{#if balance != null && allTokenInfos != null}
	<ul class="flex flex-row justify-around">
		{#each balance as balance, i}
			{#if shouldIncludeToken(balance.amount, balance.denom)}
				<TokenInfo {balance} tokenInfo={forceGetTokenInfo(balance.denom)} />
			{/if}
		{/each}
	</ul>
{:else}
	<p>Loading...</p>
{/if}
