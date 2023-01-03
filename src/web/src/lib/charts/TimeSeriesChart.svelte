<script lang="ts">
	import Chart from '$lib/components/Chart.svelte';
	import type { EChartsOption, SeriesOption } from 'echarts';
	import jsonLogo from '$lib/static/logo/json.svg';

	export let series: SeriesOption[];

	var options: EChartsOption;

	$: makeOptions(series);

	function makeOptions(series: SeriesOption[]) {
		options = {
			tooltip: {
				trigger: 'axis'
			},
			xAxis: {
				type: 'time'
			},
			yAxis: {
				type: 'value'
			},
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
					start: 95,
					end: 100
				},
				{
					start: 95,
					end: 100
				}
			],
			series: series
		};
	}
</script>

<Chart isLoading={false} {options} />
