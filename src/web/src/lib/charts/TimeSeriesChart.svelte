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
			tooltip: {
				trigger: 'axis',
				axisPointer: {
					type: 'cross',
					label: {
						backgroundColor: '#6a7985'
					}
				}
			},
			xAxis: {
				type: 'time'
			},
			yAxis: yAxis,
			toolbox: {
				itemSize: 40,
				left: 'center',
				feature: {
					saveAsImage: {
						show: true
					},
					myFeature: {
						show: false,
						title: 'Export JSON',
						icon: 'image://' + jsonLogo,
						onclick: function () {
							const tab = window.open();
							tab?.document.write(JSON.stringify(''));
							tab?.document.close();
						}
					}
				}
			},
			dataZoom: [
				{
					type: 'inside',
					start: 50,
					end: 100
				},
				{
					start: 50,
					end: 100
				}
			],
			series: series
		};
	}
</script>

<Chart isLoading={false} {options} />
