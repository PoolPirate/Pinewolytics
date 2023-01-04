<script lang="ts">
	import Chart from '$lib/components/Chart.svelte';
	import type { EChartsOption, SeriesOption } from 'echarts';
	import jsonLogo from '$lib/static/logo/json.svg';

	export let series: SeriesOption[];
	var options: EChartsOption;

	$: makeOptions(series);

	function makeOptions(series: SeriesOption[]) {
		options = {
			legend: {
				inactiveColor: '#fff',
				textStyle: {
					fontSize: 16
                },
				data: ['minimumFee', 'maximumFee', 'medianFee', 'averageFee']
			},
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
				type: 'time',
			},
			yAxis: {
				type: 'log',
                min: 0.0001,
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
			grid: {
				left: '3%',
				right: '4%',
				bottom: '3%',
				containLabel: true
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
