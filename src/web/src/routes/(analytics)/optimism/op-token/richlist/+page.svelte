<script lang="ts">
	import ZoomableChart from '$lib/charts/ZoomableChart.svelte';
	import type { OptimismAddressBalanceDTO } from '$lib/models/DTOs/OptimismDTO';
	import { QueryName } from '$lib/service/query-definitions';
	import { createQueryListener, SocketSubscriptionBuilder } from '$lib/service/subscriptions';
	import { isWeeklyModeStoreName } from '$lib/utils/Utils';
	import type { XAXisComponentOption, SeriesOption } from 'echarts';
	import { getContext, onDestroy, onMount } from 'svelte';
	import { writable, type Readable } from 'svelte/store';

	const subscriptionBuilder = new SocketSubscriptionBuilder();
	const richlistQuery = createQueryListener(subscriptionBuilder, QueryName.OptimismRichList);

	onMount(async () => {
		await subscriptionBuilder.start();
	});
	onDestroy(() => {
		subscriptionBuilder.dispose();
	});

	const isWeeklyModeStore = getContext<Readable<boolean>>(isWeeklyModeStoreName);

	const richlistChart = writable<SeriesOption[]>([]);
	const richlistXAxis = writable<XAXisComponentOption>();

	var searchAddress: string = '';

	$: makeRichlistChart($richlistQuery, searchAddress);
	function makeRichlistChart(values: OptimismAddressBalanceDTO[] | null, searchAddress: string) {
		if (values == null) {
			return;
		}

		richlistChart.set([
			{
				type: 'graph',

				layout: 'force',
				roam: true,
				force: {
					repulsion: 2000,
					initLayout: 'circular'
				},

				data: values.map((x) => {
					return {
						value: [values.indexOf(x) + 1, x.balance],
						name: x.address,
						symbolSize: Math.log10(x.balance) ** 2.2,
						itemStyle: {
							color:
								x.address?.toLowerCase().startsWith(searchAddress?.toLowerCase()) &&
								searchAddress?.length > 5
									? 'red'
									: 'rgb(' + (150 - values.indexOf(x)) + ',0,' + (150 - values.indexOf(x)) + ')'
						}
					};
				}),
				links: [],
				categories: [],
				label: {
					show: true,
					formatter: (x) => '#' + (x.value as number[])[0].toString(),
					fontSize: 12,
					lineHeight: 16
				},
				tooltip: {
					formatter: (x) => (x.value as number[])[1].toLocaleString() + ' $OP<br>' + x.name
				}
			}
		]);
	}

	function handleChartClick(param: CustomEvent<echarts.ECElementEvent>) {
		const name = (param.detail.data as { name: string }).name;
		navigator.clipboard.writeText(name);

		searchAddress = name;
	}
</script>

<div class="ontop absolute w-full flex justify-center">
	<input
		class="px-2 py-1 rounded-md w-1/2"
		bind:value={searchAddress}
		on:keydown={(param) => makeRichlistChart($richlistQuery, param.currentTarget.value)}
		type="text"
		placeholder="Highlight Address"
	/>
</div>

<div class="grid grid-cols-1 h-full">
	<ZoomableChart
		on:chartclick={handleChartClick}
		showToolTip={true}
		series={$richlistChart[0]}
		queryName={QueryName.OptimismRichList}
	/>
</div>

<style>
	.ontop {
		z-index: 100;
	}
</style>
