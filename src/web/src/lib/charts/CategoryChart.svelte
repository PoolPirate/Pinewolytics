<script lang="ts">
	import Chart from '$lib/components/Chart.svelte';
	import type { QueryName } from '$lib/service/querysubscription';
	import type {
		EChartsOption,
		SeriesOption,
		LegendComponentOption,
		TitleComponentOption
	} from 'echarts';
	import queryIcon from '$lib/static/logo/query.png';
	import { getQuerySrc } from '$lib/service/queries';

	export let series: SeriesOption | null;
	export let title: TitleComponentOption | undefined = undefined;
	export let legend: LegendComponentOption | undefined = undefined;
	let clazz: string = '';
	export { clazz as class };
	export let queryName: QueryName;

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
			},
			toolbox: {
				itemSize: 40,
				bottom: 0,
				orient: 'vertical',
				feature: {
					saveAsImage: {
						show: true
					},
					myOpenSql: {
						show: true,
						title: 'Show SQL Query',
						icon: 'image://' + queryIcon,
						onclick: async () => {
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
			}
		};
	}
</script>

<Chart class={clazz} isLoading={series == null} {options} />
