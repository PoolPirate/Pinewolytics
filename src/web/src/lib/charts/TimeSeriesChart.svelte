<script lang="ts">
	import Chart from '$lib/components/Chart.svelte';
	import type {
		EChartsOption,
		SeriesOption,
		YAXisComponentOption,
		LegendComponentOption,
		TitleComponentOption
	} from 'echarts';
	import queryIcon from '$lib/static/logo/query.png';

	import { getQuerySrc } from '$lib/service/queries';
	import type { QueryName } from '$lib/service/subscriptions';

	export let series: SeriesOption[];
	export let yAxis: YAXisComponentOption | YAXisComponentOption[] = {
		type: 'value'
	};
	export let title: TitleComponentOption = {};
	export let legend: LegendComponentOption = {};
	let clazz: string = '';
	export { clazz as class };
	export let queryName: QueryName | null;

	var options: EChartsOption;

	$: makeOptions(series);

	function makeOptions(series: SeriesOption[]) {
		options = {
			legend: {
				right: '2%',
				...legend
			},
			title: {
				left: '2%',
				...title
			},
			tooltip: {
				confine: true,
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
				bottom: 0,
				orient: 'vertical',
				feature: {
					saveAsImage: {
						show: true
					},
					myOpenSql: {
						show: queryName != null,
						title: 'Show SQL Query',
						icon: 'image://' + queryIcon,
						onclick: async () => {
							if (queryName == null) return;

							const src = await getQuerySrc(queryName);
							const tab = window.open();

							src.split('\n').forEach((x) => {
								tab?.document.writeln(x);
								tab?.document.write('<br />');
							});

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

<Chart class={clazz} isLoading={series.length == 0} {options} />
