<script lang="ts">
	import Chart from '$lib/components/Chart.svelte';
	import type {
		EChartsOption,
		SeriesOption,
		YAXisComponentOption,
		LegendComponentOption,
		TitleComponentOption
	} from 'echarts';

	export let series: SeriesOption;
	export let yAxis: YAXisComponentOption = {
		type: 'value'
	};
	export let title: TitleComponentOption = null!;
	export let legend: LegendComponentOption = null!;
	let clazz: string = '';
	export { clazz as class };

	var options: EChartsOption;

	$: makeOptions(series);

	function makeOptions(series: SeriesOption) {
		options = {
			legend: legend,
			series: series,
			title: title
		};
	}
</script>

<Chart class={clazz} isLoading={false} {options} />
