<script lang="ts">
	import TokenList from '$lib/components/osmosis/TokenList.svelte';
	import { RealtimeFeedName } from '$lib/service/realtime-feed-definitions';
	import { RealtimeValueName } from '$lib/service/realtime-value-definitions';
	import {
		SocketSubscriptionBuilder,
		createRealtimeFeedListener,
		createRealtimeValueListener
	} from '$lib/service/subscriptions';
	import { onDestroy, onMount } from 'svelte';
	import { writable } from 'svelte/store';

	const subscriptionBuilder = new SocketSubscriptionBuilder();
	var timer: NodeJS.Timeout;

	onMount(async () => {
		await subscriptionBuilder.start();
		updateTime();
	});
	onDestroy(() => {
		subscriptionBuilder.dispose();
		clearTimeout(timer);
	});

	function updateTime() {
		currentTime.set(new Date());
		timer = setTimeout(updateTime, 1000);
	}

	const protoRevTotalRevenue = createRealtimeValueListener(
		subscriptionBuilder,
		RealtimeValueName.OsmosisProtoRevTotalRevenue,
		() => []
	);

	const protoRevTotalTrades = createRealtimeValueListener(
		subscriptionBuilder,
		RealtimeValueName.OsmosisProtoRevTotalTradeCount,
		() => []
	);

	const protoRevTxs = createRealtimeFeedListener(
		subscriptionBuilder,
		RealtimeFeedName.OsmosisProtoRevTransactions
	);

	const allTokenInfos = createRealtimeValueListener(
		subscriptionBuilder,
		RealtimeValueName.OsmosisAllTokenInfos,
		() => []
	);

	const protoRevModuleBalance = createRealtimeValueListener(
		subscriptionBuilder,
		RealtimeValueName.OsmosisProtoRevModuleBalance,
		() => []
	);

	const currentTime = writable<Date>(new Date());
	const protoRevTotalRevenueUSD = writable<number | null>(null);

	$: if ($protoRevTotalRevenue != null && $allTokenInfos != null) {
		protoRevTotalRevenueUSD.set(
			$protoRevTotalRevenue.reduce((prev, x) => {
				const tokenInfo = $allTokenInfos?.find((y) => x.denom == y.denom)!;
				return prev + (tokenInfo.price * x.amount) / Math.pow(10, tokenInfo.exponent);
			}, 0)
		);
	}

	function toTimespanString(millis: number) {
		if (millis < 60000) {
			return Math.floor(millis / 1000) + ' second' + (millis >= 2000 ? 's' : '');
		}
		if (millis < 60000 * 60) {
			return Math.floor(millis / 1000 / 60) + ' minute' + (millis >= 120000 ? 's' : '');
		}
		if (millis < 60000 * 60 * 24) {
			return Math.floor(millis / 1000 / 60 / 24) + ' hour' + (millis >= 120000 * 60 ? 's' : '');
		}
	}
</script>

<div class="grid grid-cols-1 gap-8 p-3">
	<div class="grid grid-cols-1 sm:grid-cols-2 transparent-background p-3 rounded-md gap-4">
		<div class="transparent-background p-2">
			<h3 class="text-lg font-bold">All Time Profit (current prices)</h3>
			{#if $protoRevTotalRevenueUSD == null}
				<p>Loading...</p>
			{:else}
				{Math.round(100 * $protoRevTotalRevenueUSD) / 100} $
			{/if}
		</div>
		<div class="transparent-background p-2">
			<h3 class="text-lg font-bold">All Time Trade Count</h3>
			{$protoRevTotalTrades}
		</div>
	</div>

	<div class="transparent-background p-3 rounded-md">
		<h2 class="text-xl font-bold">Current Module Balance</h2>
		<hr class="m-2 border-black border-t-2" />
		<TokenList balance={$protoRevModuleBalance} allTokenInfos={$allTokenInfos} />
	</div>

	<div class="h-full flex flex-col w-full transparent-background p-3 rounded-md gap-2">
		<h2 class="font-bold text-xl">Live Trade Feed</h2>
		<div class="overflow-y-auto px-4">
			<table class="w-full">
				<thead class="sticky top-0 z-2 bg-slate-400">
					<tr>
						<th class="text-left  p-2">Wen?</th>
						<th class="text-center  p-2">Profit?</th>
						<th class="text-right  p-2">Link?</th>
					</tr>
				</thead>
				<tbody>
					{#each $protoRevTxs ?? [] as tx}
						<tr>
							<td class="text-left p-2 border-b-2 border-black">
								{toTimespanString($currentTime.getTime() - new Date(tx.timestamp).getTime())} ago
							</td>
							<td class="text-center p-2 border-b-2 border-black">
								{#each tx.swaps as swap}
									<p>
										{swap.profit.amount /
											Math.pow(
												10,
												$allTokenInfos?.find((x) => x.denom == swap.profit.denom)?.exponent ?? 0
											)}
										- {$allTokenInfos?.find((x) => x.denom == swap.profit.denom)?.symbol}
										({Math.round(
											1000 *
												((swap.profit.amount *
													($allTokenInfos?.find((x) => x.denom == swap.profit.denom)?.price ?? 0)) /
													Math.pow(
														10,
														$allTokenInfos?.find((x) => x.denom == swap.profit.denom)?.exponent ?? 0
													))
										) / 1000} $)
									</p>
								{/each}
							</td>
							<td class="text-right p-2 border-b-2 border-black">
								<a
									class="text-blue-600 font-bold"
									rel="external noreferrer"
									href="https://www.mintscan.io/osmosis/txs/{tx.hash}"
									target="_blank">-> Mintscan</a
								>
							</td>
						</tr>
					{/each}
				</tbody>
			</table>
		</div>
	</div>
</div>
