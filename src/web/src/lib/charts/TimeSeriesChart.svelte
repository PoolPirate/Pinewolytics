<script lang="ts">
	import Chart from '$lib/components/Chart.svelte';
	import type {
		EChartsOption,
		SeriesOption,
		YAXisComponentOption,
		LegendComponentOption,
		TitleComponentOption
	} from 'echarts';
	import jsonLogo from '$lib/static/logo/json.svg';

	export let series: SeriesOption[];
	export let yAxis: YAXisComponentOption = {
		type: 'value'
	};
	export let title: TitleComponentOption = {};
	export let legend: LegendComponentOption = {};
	let clazz: string = '';
	export { clazz as class };

	var options: EChartsOption;

	$: makeOptions(series);

	function makeOptions(series: SeriesOption[]) {
		options = {
			legend: legend,
			title: title,
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
				left: 'right',
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

<Chart class={clazz} isLoading={false} {options} />
