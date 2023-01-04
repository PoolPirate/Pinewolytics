<script lang="ts">
	import Chart from '$lib/components/Chart.svelte';
	import type {
		EChartsOption,
		SeriesOption,
		YAXisComponentOption,
		LegendComponentOption
	} from 'echarts';
	import jsonLogo from '$lib/static/logo/json.svg';

	export let series: SeriesOption[];
	export let yAxis: YAXisComponentOption = {
		type: 'value'
	};
	export let legend: LegendComponentOption = {};

	var options: EChartsOption;

	$: makeOptions(series);

	function makeOptions(series: SeriesOption[]) {
		options = {
			legend: legend,
			toolbox: {
				itemSize: 40,
				left: 'right',
			},
			series: series
		};
	}
</script>

<Chart isLoading={false} {options} />
