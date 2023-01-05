<script lang="ts">
	import ZoomableChart from '$lib/charts/ZoomableChart.svelte';
	import type { TerraAddressBalanceDTO } from '$lib/models/DTOs/TerraDTOs';
	import {
		createQueryListener,
		QueryName,
		QuerySubscriptionBuilder
	} from '$lib/service/querysubscription';
	import type { XAXisComponentOption, SeriesOption } from 'echarts';
	import { getContext, onDestroy, onMount } from 'svelte';
	import { writable, type Readable } from 'svelte/store';
	import { isWeeklyModeStoreName } from '../../+layout.svelte';

	const subscriptionBuilder = new QuerySubscriptionBuilder();
	const richlistQuery = createQueryListener(subscriptionBuilder, QueryName.TerraRichList);

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
	function makeRichlistChart(values: TerraAddressBalanceDTO[], searchAddress: string) {
		if (values.length == 0) {
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

				data: values.map((x) => {
					return {
						value: [values.indexOf(x) + 1, x.balance],
						name: x.address,
						symbolSize: Math.log10(x.balance) ** 2.5,
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
					formatter: (x) => (x.value as number[])[1].toLocaleString() + ' $LUNA<br>' + x.name
				}
			}
		]);
	}
</script>

<div class="absolute w-full flex justify-center z-100">
	<input
		class="px-2 py-1 rounded-md w-1/2"
		bind:value={searchAddress}
		on:keydown={(param) => makeRichlistChart($richlistQuery, param.currentTarget.value)}
		type="text"
		placeholder="Highlight Address"
	/>
</div>

<ZoomableChart
	on:chartclick={(params) => console.log(params)}
	showToolTip={true}
	class="h-full"
	series={$richlistChart[0]}
/>

<style>
	div {
		z-index: 100;
	}
</style>
