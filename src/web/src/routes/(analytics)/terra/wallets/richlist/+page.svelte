<script lang="ts">
	import ZoomableChart from '$lib/charts/ZoomableChart.svelte';
	import type { TerraAddressBalanceDTO } from '$lib/models/DTOs/TerraDTOs';
	import { QueryName } from '$lib/service/query-definitions';
	import { createQueryListener, SocketSubscriptionBuilder } from '$lib/service/subscriptions';
	import type { SeriesOption } from 'echarts';
	import { onDestroy, onMount } from 'svelte';
	import { writable, type Readable } from 'svelte/store';

	const subscriptionBuilder = new SocketSubscriptionBuilder();
	const richlistQuery = createQueryListener(subscriptionBuilder, QueryName.TerraRichList);

	onMount(async () => {
		await subscriptionBuilder.start();
	});
	onDestroy(() => {
		subscriptionBuilder.dispose();
	});

	const richlistChart = writable<SeriesOption[]>([]);

	var searchAddress: string = '';

	$: makeRichlistChart($richlistQuery, searchAddress);
	function makeRichlistChart(values: TerraAddressBalanceDTO[] | null, searchAddress: string) {
		if (values == null) {
			return;
		}

		values = values.sort((a, b) => b.balance - a.balance);

		richlistChart.set([
			{
				type: 'graph',

				layout: 'force',
				roam: true,
				force: {
					repulsion: 700,
					initLayout: 'circular'
				},

				data: values.map((x, i, arr) => {
					return {
						value: [arr.indexOf(x) + 1, x.balance],
						name: x.address,
						symbolSize: Math.log10(x.balance) ** 2.5,
						itemStyle: {
							color:
								x.address?.toLowerCase().startsWith(searchAddress?.toLowerCase()) &&
								searchAddress?.length > 5
									? 'red'
									: 'rgb(' + (150 - arr.indexOf(x)) + ',0,' + (150 - arr.indexOf(x)) + ')'
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
					formatter: (x) => (x.value as number[])[1].toLocaleString() + ' $LUNA<br>' + x.name
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
