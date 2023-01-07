<script lang="ts">
	import Chart from '$lib/components/Chart.svelte';
	import type {
		EChartsOption,
		SeriesOption,
		LegendComponentOption,
		TitleComponentOption
	} from 'echarts';

	export let series: SeriesOption | null;
	export let title: TitleComponentOption | undefined = undefined;
	export let legend: LegendComponentOption | undefined = undefined;
	let clazz: string = '';
	export { clazz as class };

	export let categories: string[];

	var options: EChartsOption;

	$: makeOptions(series);

	function makeOptions(series: SeriesOption | null) {
		options = {
			legend: legend,
			series: series == null ? {} : series,
			title: title,
			xAxis: {
				type: 'category',
				data: categories
			}
		};
	}
</script>

<Chart class={clazz} isLoading={series == null} {options} />
