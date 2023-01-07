<script lang="ts">
	import Chart from '$lib/components/Chart.svelte';
	import { getQuerySrc } from '$lib/service/queries';
	import type { QueryName } from '$lib/service/querysubscription';
	import type {
		EChartsOption,
		SeriesOption,
		YAXisComponentOption,
		LegendComponentOption,
		TitleComponentOption,
		TooltipComponentOption
	} from 'echarts';
	import { createEventDispatcher } from 'svelte';
	import queryIcon from '$lib/static/logo/query.png';

	export let series: SeriesOption | null;
	export let title: TitleComponentOption | undefined = undefined;
	export let legend: LegendComponentOption | undefined = undefined;

	export let showToolTip: boolean = false;
	export let queryName: QueryName;

	let clazz: string = '';
	export { clazz as class };

	var options: EChartsOption;

	const dispatch = createEventDispatcher<{ chartclick: echarts.ECElementEvent }>();

	$: makeOptions(series);
	function makeOptions(series: SeriesOption | null) {
		options = {
			legend: legend,
			series: series == null ? {} : series,
			title: title,
			tooltip: showToolTip ? {} : undefined,
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

<Chart
	on:chartclick={(params) => dispatch('chartclick', params.detail)}
	class={clazz}
	isLoading={series == null}
	{options}
/>
