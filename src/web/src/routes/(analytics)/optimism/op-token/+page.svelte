<script lang="ts">
	import TimeSeriesChart from '$lib/charts/TimeSeriesChart.svelte';
	import ZoomableChart from '$lib/charts/ZoomableChart.svelte';
	import type { OptimismAddressBalanceDTO } from '$lib/models/DTOs/OptimismDTO';
	import type { TerraAddressBalanceDTO } from '$lib/models/DTOs/TerraDTOs';
	import type { MarketDataDTO, TimeSeriesEntryDTO } from '$lib/models/SharedDTOs';
	import {
		createQueryListener,
		QueryName,
		QuerySubscriptionBuilder
	} from '$lib/service/querysubscription';
	import { DaySeriesToWeekSeriesByAvg, type TimeSeriesEntry } from '$lib/service/transform';
	import { isWeeklyModeStoreName } from '$lib/utils/Utils';
	import { HubConnectionBuilder } from '@microsoft/signalr';
	import type { XAXisComponentOption, SeriesOption } from 'echarts';
	import { getContext, onDestroy, onMount } from 'svelte';
	import { writable, type Readable } from 'svelte/store';
	import RefreshAnimation from '$lib/components/RefreshAnimation.svelte';

	import priceIcon from '$lib/static/logo/price.svg';
	import mcapIcon from '$lib/static/logo/market_cap.png';
	import pileIcon from '$lib/static/logo/pile.svg';
	import smallPileIcon from '$lib/static/logo/small_pile.webp';

	const subscriptionBuilder = new QuerySubscriptionBuilder();
	const priceHistoryQuery = createQueryListener(
		subscriptionBuilder,
		QueryName.OptimismPriceHistory
	);

	const marketData = writable<MarketDataDTO>();
	const marketDataAnimations: RefreshAnimation[] = [];

	onMount(async () => {
		await subscriptionBuilder.start();

		let connection = new HubConnectionBuilder()
			.withUrl('/api/hub/optimism')
			.withAutomaticReconnect()
			.build();

		connection.on('MarketData', (newMarketData: MarketDataDTO) => {
			marketData.set(newMarketData);
			marketDataAnimations.forEach((x) => x.play());
		});

		await connection.start();
	});
	onDestroy(() => {
		subscriptionBuilder.dispose();
	});

	const isWeeklyModeStore = getContext<Readable<boolean>>(isWeeklyModeStoreName);
	const priceHistoryChart = writable<SeriesOption[]>([]);

	$: makePriceHistoryChart($priceHistoryQuery, $isWeeklyModeStore);
	function makePriceHistoryChart(values: TimeSeriesEntryDTO[], isWeeklyMode: boolean) {
		if (values.length == 0) {
			return;
		}

		var opPriceSeries = values
			.map<TimeSeriesEntry>((x) => {
				return {
					timestamp: new Date(x.timestamp),
					value: x.value
				};
			})
			.sort((a, b) => a.timestamp.getTime() - b.timestamp.getTime());

		if (isWeeklyMode) {
			opPriceSeries = DaySeriesToWeekSeriesByAvg(opPriceSeries);
		}

		priceHistoryChart.set([
			{
				type: 'line',
				name: 'Price',
				data: opPriceSeries.map((x) => [x.timestamp, x.value]),
				sampling: 'lttb',
				markLine: {
					data: [
						{ type: 'max', name: 'Max', lineStyle: { color: 'lime' } },
						{ type: 'min', name: 'Min', lineStyle: { color: 'red' } }
					]
				}
			}
		]);
	}
</script>

<ul
	class="text-center grid grid-cols-2 justify-items-center gap-4 mb-4 transparent-background p-4 rounded-lg
			   [&>li]:bg-gray-400 [&>li]:p-3 [&>li]:rounded-xl [&>li]:w-full [&>li]:relative [&>li]
			   [&>li>h2]:font-bold
			   [&>li>svg]:absolute [&>li>svg]:w-12 [&>li>svg]:right-1
			   [&>li>img]:absolute [&>li>img]:h-12"
>
	<li>
		<img alt="icon" class="h-1/2" src={priceIcon} />
		<RefreshAnimation bind:this={marketDataAnimations[0]} />
		<h2>Price</h2>
		<p>{$marketData?.price?.toLocaleString() ?? 'Loading...'} $USD</p>
	</li>
	<li>
		<img alt="icon" class="h-1/2" src={mcapIcon} />
		<RefreshAnimation bind:this={marketDataAnimations[1]} />
		<h2>Market Cap</h2>
		<p>{$marketData?.marketCap?.toLocaleString() ?? 'Loading...'} $USD</p>
	</li>
	<li>
		<img alt="icon" class="h-1/2" src={pileIcon} />
		<RefreshAnimation bind:this={marketDataAnimations[2]} />
		<h2>Total Supply</h2>
		<p>{$marketData?.totalSupply?.toLocaleString() ?? 'Loading...'} $OP</p>
	</li>
	<li>
		<img alt="icon" class="h-1/2" src={smallPileIcon} />
		<RefreshAnimation bind:this={marketDataAnimations[3]} />
		<h2>Circulating Supply</h2>
		<p>{$marketData?.circulatingSupply?.toLocaleString() ?? 'Loading...'} $OP</p>
	</li>
</ul>

<div class="transparent-background rounded-lg p-3 w-full grid grid-cols-1">
	<TimeSeriesChart
		title={{ text: '$OP / USD' }}
		legend={{}}
		class="h-128"
		series={$priceHistoryChart}
		queryName={QueryName.OptimismPriceHistory}
	/>
</div>
