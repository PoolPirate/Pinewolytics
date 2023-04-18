<script lang="ts">
	import SingleValueChart from '$lib/charts/SingleValueChart.svelte';
	import type { OsmosisDenominatedAmountDTO } from '$lib/models/DTOs/OsmosisDTOs';
	import { RealtimeFeedName } from '$lib/service/realtime-feed-definitions';
	import { RealtimeValueName } from '$lib/service/realtime-value-definitions';
	import {
		SocketSubscriptionBuilder,
		createRealtimeFeedListener,
		createRealtimeValueListener
	} from '$lib/service/subscriptions';
	import type { SeriesOption } from 'echarts';
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

	const currentTime = writable<Date>(new Date());

	const protoRevTotalRevenueChart = writable<SeriesOption | null>(null);
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

	$: makeProtoRevTotalRevenueChart($protoRevTotalRevenue);
	function makeProtoRevTotalRevenueChart(
		totalProtoRevRevenue: OsmosisDenominatedAmountDTO[] | null
	) {
		if (totalProtoRevRevenue == null) {
			return;
		}

		protoRevTotalRevenueChart.set({
			type: 'pie',

			data: totalProtoRevRevenue.map((x) => {
				return {
					name: x.denom,
					value: x.amount
				};
			})
		});
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

<SingleValueChart series={$protoRevTotalRevenueChart} queryName={null} />

<div class="h-1/3 flex flex-col w-full transparent-background p-2 rounded-md">
	<h2 class="font-bold text-xl">Live Trade Feed</h2>
	<div class="overflow-y-auto px-4">
		<table class="w-full">
			<thead class="sticky top-0 z-2 bg-slate-400">
				<tr>
					<th class="text-left">Wen?</th>
					<th class="text-center">Profit?</th>
					<th class="text-right">Link?</th>
				</tr>
			</thead>
			<tbody>
				{#each $protoRevTxs ?? [] as tx}
					<tr>
						<td class="text-left">
							{toTimespanString($currentTime.getTime() - new Date(tx.timestamp).getTime())} ago
						</td>
						<td class="text-center">
							{#each tx.profits as profit}
								<p>
									{profit.amount /
										Math.pow(
											10,
											$allTokenInfos?.find((x) => x.denom == profit.denom)?.exponent ?? 0
										)}
									- {$allTokenInfos?.find((x) => x.denom == profit.denom)?.symbol}
								</p>
							{/each}
						</td>
						<td class="text-right">
							<a
								rel="external noreferrer"
								href="https://www.mintscan.io/osmosis/txs/{tx.hash}"
								target="_blank">See on Mintscan</a
							>
						</td>
					</tr>
				{/each}
			</tbody>
		</table>
	</div>
</div>
