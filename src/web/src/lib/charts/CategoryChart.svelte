<script lang="ts">
	import Chart from '$lib/components/Chart.svelte';
	import type { QueryName } from '$lib/service/subscriptions';
	import type {
		EChartsOption,
		SeriesOption,
		LegendComponentOption,
		TitleComponentOption,
		YAXisComponentOption
	} from 'echarts';
	import queryIcon from '$lib/static/logo/query.png';
	import { getQuerySrc } from '$lib/service/queries';

	export let series: SeriesOption | null;
	export let title: TitleComponentOption | undefined = undefined;
	export let yAxis: YAXisComponentOption | YAXisComponentOption[] = {
		type: 'value'
	};
	let clazz: string = '';
	export { clazz as class };
	export let queryName: QueryName | null;

	export let showToolTip: boolean = false;
	export let showLegend: boolean = false;
	export let sidebarLegend: boolean = false;

	export let categories: string[];

	var options: EChartsOption;

	$: makeOptions(series);

	function makeOptions(series: SeriesOption | null) {
		options = {
			legend: showLegend
				? sidebarLegend
					? { orient: 'vertical', top: 'center', left: '2%' }
					: { right: 'center', top: '8%' }
				: undefined,
			tooltip: showToolTip ? { confine: true } : undefined,
			series: series == null ? {} : series,
			title: title,
			xAxis: {
				type: 'category',
				data: categories,
				axisLabel: {
					show: true,
					hideOverlap: false
				}
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
							const src = await getQuerySrc(queryName!);
							const tab = window.open();

							src.split('\n').forEach((x) => {
								tab?.document.writeln(x);
								tab?.document.write('<br />');
							});

							tab?.document.close();
						}
					}
				}
			}
		};
	}
</script>

<Chart class={clazz} isLoading={series == null} {options} />
