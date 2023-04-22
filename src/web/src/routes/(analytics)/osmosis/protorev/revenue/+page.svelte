<script lang="ts">
	import SingleValueChart from '$lib/charts/SingleValueChart.svelte';
	import TimeSeriesChart from '$lib/charts/TimeSeriesChart.svelte';
	import type {
		OsmosisProtoRevRevenueDTO,
		OsmosisTokenInfoDTO,
		OsomsisProtoRevAssetRevenueDTO
	} from '$lib/models/DTOs/OsmosisDTOs';
	import { QueryName } from '$lib/service/query-definitions';
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

	onMount(async () => {
		await subscriptionBuilder.start();
	});
	onDestroy(() => {
		subscriptionBuilder.dispose();
	});

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

	$: makeProtoRevRevenueChart(
		$protoRevRevenueHistoryQuery,
		$protoRevRevenuePerAssetQuery,
		$isWeeklyModeStore
	);
	function makeProtoRevRevenueChart(
		revenueHistory: OsmosisProtoRevRevenueDTO[] | null,
		revenuePerAsset: OsomsisProtoRevAssetRevenueDTO[] | null,
		isWeeklyMode: boolean
	) {
		if (revenueHistory == null || revenuePerAsset == null) {
			return;
		}

		const currencies = revenueHistory
			.map((x) => x.symbol)
			.filter(onlyUnique)
			.map((x) => {
				return {
					symbol: x,
					currency: revenueHistory.find((y) => y.symbol == x)!.currency
				};
			});

		const incomeSeries = currencies.map((currency) => {
			return {
				symbol: currency.symbol,
				currency: currency.currency,
				data: revenueHistory
					.filter((x) => x.symbol == currency.symbol)
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
			currencies.forEach((currency) => {
				const series = incomeSeries.find((x) => x.symbol == currency.symbol)!;
				series.data = DaySeriesToWeekSeriesBySum(series.data);
			});
		}

		const incomeCharts = incomeSeries.map<SeriesOption>((x) => {
			return {
				type: 'bar',
				stack: 'income',
				yAxisId: '1',
				name: x.symbol,
				data: x.data.map((x) => [x.timestamp, Math.round(100 * x.value) / 100])
			};
		});
		const totalProfitCharts = incomeSeries.map<SeriesOption>((x) => {
			const assetProfit = revenuePerAsset.find((y) => y.currency == x.currency)!;
			return {
				type: 'line',
				areaStyle: {},
				yAxisId: '2',
				stack: 'total',
				name: x.symbol,
				z: 0,
				data: x.data
					.map((x, i, arr) => {
						return {
							cumulativeUSD: arr
								.filter((y) => y.timestamp > x.timestamp)
								.reduce((total, y) => total + y.value, 0),
							...x
						};
					})
					.map((x) => [
						x.timestamp,
						Math.round(100 * (assetProfit.totalUSD - x.cumulativeUSD)) / 100
					])
			};
		});

		protoRevRevenueChart.set(totalProfitCharts.concat(incomeCharts));
	}

	$: makeProtoRevRevenuePerAssetChart($protoRevRevenuePerAssetQuery, $allTokenInfos);
	function makeProtoRevRevenuePerAssetChart(
		profits: OsomsisProtoRevAssetRevenueDTO[] | null,
		allTokenInfos: OsmosisTokenInfoDTO[] | null
	) {
		if (profits == null || allTokenInfos == null) {
			return;
		}

		console.log(profits);

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
				console.log(tokenInfo);
				return {
					name: tokenInfo.symbol,
					value: x.totalUSD
				};
			}),
			top: '20px'
		});
	}
</script>

<div class="flex flex-col h-full w-full transparent-background p-2 rounded-lg">
	<div class="grid grid-cols-2 h-full">
		<div class="flex flex-col justify-center items-center">
			<h2 class="font-bold text-lg">Note:</h2>
			<p>All values in $USD at <b>at time of the trasaction!</b></p>
		</div>
		<SingleValueChart
			series={$protoRevRevenuePerAssetChart}
			showLegend={true}
			queryName={QueryName.OsmosisProtoRevRevenuePerAsset}
		/>
	</div>

	<TimeSeriesChart
		series={$protoRevRevenueChart}
		queryName={QueryName.OsmosisProtoRevRevenueHistory}
		yAxis={[{ id: 1 }, { id: 2 }]}
	/>
</div>
