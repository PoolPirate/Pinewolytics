<script lang="ts">
	import TimeSeriesChart from '$lib/charts/TimeSeriesChart.svelte';
	import type { OsmosisProtoRevRevenueDTO } from '$lib/models/DTOs/OsmosisDTOs';
	import { QueryName } from '$lib/service/query-definitions';
	import { SocketSubscriptionBuilder, createQueryListener } from '$lib/service/subscriptions';
	import {
		DaySeriesToWeekSeriesBySum,
		onlyUnique,
		type TimeSeriesEntry
	} from '$lib/service/transform';
	import { isWeeklyModeStoreName } from '$lib/utils/Utils';
	import type { SeriesOption } from 'echarts';
	import { getContext, onDestroy, onMount } from 'svelte';
	import { writable, type Readable } from 'svelte/store';

	const subscriptionBuilder = new SocketSubscriptionBuilder();
	const isWeeklyModeStore = getContext<Readable<boolean>>(isWeeklyModeStoreName);

	const osmosisProtoRevRevenueChart = writable<SeriesOption[]>([]);
	const osmosisProtoRevRevenueHistoryQuery = createQueryListener(
		subscriptionBuilder,
		QueryName.OsmosisProtoRevRevenueHistory
	);

	$: makeOsmosisProtoRevRevenueChart($osmosisProtoRevRevenueHistoryQuery, $isWeeklyModeStore);
	function makeOsmosisProtoRevRevenueChart(
		revenueHistory: OsmosisProtoRevRevenueDTO[],
		isWeeklyMode: boolean
	) {
		if (revenueHistory.length == 0) {
			return;
		}

		const symbols = revenueHistory.map((x) => x.symbol).filter(onlyUnique);

		const incomeSeries = symbols.map((symbol) => {
			return {
				symbol: symbol,
				data: revenueHistory
					.filter((x) => x.symbol == symbol)
					.map<TimeSeriesEntry>((x) => {
						return {
							timestamp: new Date(x.date),
							value: x.totalAmountUSD
						};
					})
					.sort((a, b) => a.timestamp.getTime() - b.timestamp.getTime())
			};
		});

		if (isWeeklyMode) {
			symbols.forEach((symbol) => {
				const series = incomeSeries.find((x) => x.symbol == symbol)!;
				series.data = DaySeriesToWeekSeriesBySum(series.data);
			});
		}

		osmosisProtoRevRevenueChart.set(
			incomeSeries.map((x) => {
				return {
					type: 'bar',
					stack: 'income',
					name: x.symbol,
					data: x.data.map((x) => [x.timestamp, Math.round(100 * x.value) / 100])
				};
			})
		);
	}

	onMount(async () => {
		await subscriptionBuilder.start();
	});
	onDestroy(() => {
		subscriptionBuilder.dispose();
	});
</script>

<TimeSeriesChart
	series={$osmosisProtoRevRevenueChart}
	queryName={QueryName.OsmosisProtoRevRevenueHistory}
/>
