<script lang="ts">
	import Chart from '$lib/components/Chart.svelte';
	import type {
		EChartsOption,
		SeriesOption,
		YAXisComponentOption,
		LegendComponentOption,
		TitleComponentOption,
		TooltipComponentOption
	} from 'echarts';
	import { createEventDispatcher } from 'svelte';

	export let series: SeriesOption | null;
	export let title: TitleComponentOption | undefined = undefined;
	export let legend: LegendComponentOption | undefined = undefined;

	export let showToolTip: boolean = false;

	let clazz: string = '';
	export { clazz as class };

	var options: EChartsOption;

	const dispatch = createEventDispatcher<{ chartclick: {} }>();

	$: makeOptions(series);
	function makeOptions(series: SeriesOption | null) {
		options = {
			legend: legend,
			series: series == null ? {} : series,
			title: title,
			tooltip: showToolTip ? {} : undefined,
			toolbox: {
				itemSize: 40,
				top: 'bottom',
				feature: {
					saveAsImage: {
						show: true
					},
					myFeature: {
						show: false,
						title: 'Export JSON',
						onclick: function () {
							const tab = window.open();
							tab?.document.write(JSON.stringify(''));
							tab?.document.close();
						}
					}
				}
			}
		};
	}
</script>

<Chart
	on:chartclick={(params) => dispatch('chartclick', params.detail)}
	class={clazz}
	isLoading={series == null}
	{options}
/>
