<script lang="ts">
	import RefreshAnimation from '$lib/components/RefreshAnimation.svelte';
	import type {
		OsmosisProtoRevRouteStatisticsDTO,
		OsmosisTokenInfoDTO
	} from '$lib/models/DTOs/OsmosisDTOs';
	import { RealtimeValueName } from '$lib/service/realtime-value-definitions';
	import {
		SocketSubscriptionBuilder,
		createRealtimeValueListener
	} from '$lib/service/subscriptions';
	import { isWeeklyModeStoreName } from '$lib/utils/Utils';
	import { getContext, onDestroy, onMount } from 'svelte';
	import { writable, type Readable } from 'svelte/store';
	import Loading from '$lib/static/loading.svg';

	const subscriptionBuilder = new SocketSubscriptionBuilder();
	const isWeeklyModeStore = getContext<Readable<boolean>>(isWeeklyModeStoreName);

	onMount(async () => {
		await subscriptionBuilder.start();
	});
	onDestroy(() => {
		subscriptionBuilder.dispose();
	});

	enum OrderDirection {
		NumberOfTrades,
		ProfitUSD
	}

	interface RouteStatistic extends OsmosisProtoRevRouteStatisticsDTO {
		profitUSD: number;
	}

	const pageSize = 20;
	const page = writable<number>(0);
	const orderDirection = writable<OrderDirection>(OrderDirection.NumberOfTrades);
	const reverseOrdering = writable<boolean>(false);

	const protoRevAllRouteStatistics = createRealtimeValueListener(
		subscriptionBuilder,
		RealtimeValueName.OsmosisProtoRevAllRouteStatistics,
		() => []
	);
	const allTokenInfos = createRealtimeValueListener(
		subscriptionBuilder,
		RealtimeValueName.OsmosisAllTokenInfos,
		() => []
	);

	const selectedRoutes = writable<RouteStatistic[] | null>(null);

	$: selectRoutes(
		$protoRevAllRouteStatistics,
		$allTokenInfos,
		$orderDirection,
		$reverseOrdering,
		$page,
		pageSize
	);
	function selectRoutes(
		allRoutes: OsmosisProtoRevRouteStatisticsDTO[] | null,
		allTokenInfos: OsmosisTokenInfoDTO[] | null,
		orderDirection: OrderDirection,
		reverseOrdering: boolean,
		page: number,
		pageSize: number
	) {
		if (allRoutes == null || allTokenInfos == null) {
			return;
		}

		selectedRoutes.set(
			allRoutes
				.map((x) => {
					return {
						profitUSD: x.profits.reduce<number>((prev, curr) => {
							const tokenInfo = allTokenInfos.find((x) => x.denom == curr.denom)!;
							return prev + (curr.amount * tokenInfo.price) / Math.pow(10, tokenInfo.exponent);
						}, 0),
						...x
					} as RouteStatistic;
				})
				.sort((a, b) => {
					if (orderDirection == OrderDirection.NumberOfTrades)
						return reverseOrdering
							? a.numberOfTrades - b.numberOfTrades
							: b.numberOfTrades - a.numberOfTrades;
					if (orderDirection == OrderDirection.ProfitUSD)
						return reverseOrdering ? a.profitUSD - b.profitUSD : b.profitUSD - a.profitUSD;

					throw 'Bad Ordering';
				})
				.slice(page * pageSize, page * pageSize + pageSize)
		);
	}

	function setOrdering(newOrderDirection: OrderDirection) {
		if ($orderDirection == newOrderDirection) {
			reverseOrdering.set(!$reverseOrdering);
		} else {
			orderDirection.set(newOrderDirection);
			reverseOrdering.set(false);
		}
	}

	function getOrderingSuffic(
		currentOrderDirection: OrderDirection,
		isReverse: boolean,
		orderDirection: OrderDirection
	) {
		if (currentOrderDirection != orderDirection) {
			return '';
		}

		return isReverse ? '▴' : '▾';
	}

	function incrementPage() {
		const maxPage = Math.floor(($protoRevAllRouteStatistics?.length ?? 0) / pageSize);
		page.set(Math.min(maxPage, $page + 1));
	}
	function decrementPage() {
		page.set(Math.max(0, $page - 1));
	}
</script>

<h1 class="font-bold text-2xl">Routes</h1>

{#if $selectedRoutes == null}
	<img class="w-64 p-4 invert" src={Loading} alt="Loading" />
{:else}
	<table class="table-auto">
		<thead>
			<tr>
				<th
					class="hover:cursor-pointer"
					on:click={() => setOrdering(OrderDirection.NumberOfTrades)}
				>
					Number Of Trades
					{getOrderingSuffic($orderDirection, $reverseOrdering, OrderDirection.NumberOfTrades)}
				</th>
				<th class="hover:cursor-pointer" on:click={() => setOrdering(OrderDirection.ProfitUSD)}>
					Profit $USD
					{getOrderingSuffic($orderDirection, $reverseOrdering, OrderDirection.ProfitUSD)}
				</th>
				<th>Pools</th>
			</tr>
		</thead>
		<tbody>
			{#each $selectedRoutes as routeStatistics}
				<tr>
					<td>{routeStatistics.numberOfTrades}</td>
					<td>
						{Math.round(100 * routeStatistics.profitUSD) / 100} $
					</td>
					<td>
						{routeStatistics.route.reduce((prev, curr) => prev + ' -> ' + curr)}
					</td>
				</tr>
			{/each}
		</tbody>
	</table>

	<div class="flex flex-row gap-5">
		<button class="bg-gray-200 px-4 py-1" on:click={decrementPage}>Back</button>
		<p class="bg-gray-200 px-4 py-1">{$page + 1}</p>
		<button class="bg-gray-200 px-4 py-1" on:click={incrementPage}>Next</button>
	</div>
{/if}
