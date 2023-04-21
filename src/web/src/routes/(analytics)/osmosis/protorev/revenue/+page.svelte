<script lang="ts">
	import SingleValueChart from '$lib/charts/SingleValueChart.svelte';
	import TimeSeriesChart from '$lib/charts/TimeSeriesChart.svelte';
	import type {
		OsmosisDenominatedAmountDTO,
		OsmosisProtoRevRevenueDTO,
		OsmosisTokenInfoDTO,
		OsomsisProtoRevAssetRevenueDTO
	} from '$lib/models/DTOs/OsmosisDTOs';
	import { QueryName } from '$lib/service/query-definitions';
	import { RealtimeFeedName } from '$lib/service/realtime-feed-definitions';
	import { RealtimeValueName } from '$lib/service/realtime-value-definitions';
	import {
		SocketSubscriptionBuilder,
		createQueryListener,
		createRealtimeFeedListener,
		createRealtimeValueListener
	} from '$lib/service/subscriptions';
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

	const protoRevRevenueChart = writable<SeriesOption[]>([]);
	const protoRevRevenueHistoryQuery = createQueryListener(
		subscriptionBuilder,
		QueryName.OsmosisProtoRevRevenueHistory
	);

	const protoRevRevenuePerAssetChart = writable<SeriesOption | null>(null);
	const protoRevRevenuePerAssetQuery = createQueryListener(
		subscriptionBuilder,
		QueryName.OsmosisProtoRevRevenuePerAsset
	);

	const allTokenInfos = createRealtimeValueListener(
		subscriptionBuilder,
		RealtimeValueName.OsmosisAllTokenInfos,
		() => []
	);

	$: makeProtoRevRevenueChart($protoRevRevenueHistoryQuery, $isWeeklyModeStore);
	function makeProtoRevRevenueChart(
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

		protoRevRevenueChart.set(
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

	$: makeProtoRevRevenuePerAssetChart($protoRevRevenuePerAssetQuery, $allTokenInfos);
	function makeProtoRevRevenuePerAssetChart(
		profits: OsomsisProtoRevAssetRevenueDTO[] | null,
		allTokenInfos: OsmosisTokenInfoDTO[] | null
	) {
		if (profits == null || allTokenInfos == null) {
			return;
		}

		protoRevRevenuePerAssetChart.set({
			type: 'pie',
			radius: ['40%', '70%'],
			avoidLabelOverlap: true,
			label: {
				show: false,
				position: 'center'
			},
			emphasis: {
				label: {
					show: true,
					fontSize: 20,
					fontWeight: 'bold',
					formatter: (x) => Math.round(100 * (x.value as number)) / 100 + ' $'
				}
			},
			data: profits.map((x) => {
				const tokenInfo = allTokenInfos?.find((y) => y.denom == x.currency)!;
				return {
					name: tokenInfo.symbol,
					value: x.totalUSD
				};
			}),
			top: '20px'
		});
	}

	onMount(async () => {
		await subscriptionBuilder.start();
	});
	onDestroy(() => {
		subscriptionBuilder.dispose();
	});
</script>

<SingleValueChart
	series={$protoRevRevenuePerAssetChart}
	showLegend={true}
	queryName={QueryName.OsmosisProtoRevRevenuePerAsset}
/>

<TimeSeriesChart
	series={$protoRevRevenueChart}
	queryName={QueryName.OsmosisProtoRevRevenueHistory}
/>
