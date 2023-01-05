<script lang="ts" context="module">
	import * as echarts from 'echarts';
	import { createEventDispatcher, onDestroy, onMount } from 'svelte';
	export type EChartsTheme = string | object;
	export type EChartsRenderer = 'canvas' | 'svg';
	export type ChartOptions = {
		theme?: EChartsTheme;
		renderer?: EChartsRenderer;
	};
	export type EChartsLoadingOption = {
		text?: string;
		color?: string;
		textColor?: string;
		maskColor?: string;

		fontSize?: number;
		showSpinner?: boolean;
		spinnerRadius?: number;
	};

	const DEFAULT_OPTIONS: Partial<ChartOptions> = {
		theme: undefined,
		renderer: 'canvas'
	};
	const DEFAULT_LOADING_OPTIONS: Partial<EChartsLoadingOption> = {
		text: 'loading',
		color: '#c23531',
		textColor: '#000',
		maskColor: 'rgba(255, 255, 255, 0.8)',
		fontSize: 12,
		showSpinner: true,
		spinnerRadius: 10
	};
</script>

<script lang="ts">
	export let chartOptions: ChartOptions = DEFAULT_OPTIONS;
	export let options: echarts.EChartsOption;

	export let isLoading: boolean;

	let clazz: string;
	export { clazz as class };

	const dispatch = createEventDispatcher<{ chartclick: echarts.ECElementEvent }>();

	var element: HTMLElement;
	var instance: echarts.ECharts | null;
	var observer: ResizeObserver | null;

	$: instance?.setOption(options);
	$: if (isLoading) {
		instance?.showLoading('default', DEFAULT_LOADING_OPTIONS);
	} else instance?.hideLoading();

	onMount(() => {
		const settings = {
			...DEFAULT_OPTIONS,
			...chartOptions
		};
		instance = echarts.init(element, settings.theme, settings);
		instance.setOption(options);

		if (isLoading) {
			instance.showLoading('default', DEFAULT_LOADING_OPTIONS);
		} else {
			instance.hideLoading();
		}

		instance.on('click', (params) => dispatch('chartclick', params));

		observer = new ResizeObserver(() => instance?.resize());
		observer.observe(element);
	});

	onDestroy(() => {
		instance?.dispose();
		observer?.disconnect();
	});
</script>

<div class="{clazz} w-full h-full" bind:this={element} />
