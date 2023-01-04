<script lang="ts" context="module">
	import type { EChartsOption } from 'echarts';
	import * as echarts from 'echarts';
	export type EChartsTheme = string | object;
	export type EChartsRenderer = 'canvas' | 'svg';
	export type ChartOptions = {
		theme?: EChartsTheme;
		renderer?: EChartsRenderer;
		options: EChartsOption;

		isLoading: boolean;
		loadingOptions?: EChartsLoadingOption;
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

	export function chartable(element: HTMLElement, echartOptions: ChartOptions) {
		const { theme, renderer, options } = {
			...DEFAULT_OPTIONS,
			...echartOptions
		};

		const { isLoading, loadingOptions } = {
			...DEFAULT_LOADING_OPTIONS,
			...echartOptions
		};

		const echartsInstance = echarts.init(element, theme, { renderer });
		echartsInstance.setOption(options);

		if (isLoading) {
			echartsInstance.showLoading('default', loadingOptions);
		} else {
			echartsInstance.hideLoading();
		}

		function handleResize() {
			echartsInstance.resize();
		}

		new ResizeObserver(handleResize).observe(element);

		return {
			destroy() {
				echartsInstance.dispose();
				window.removeEventListener('resize', handleResize);
			},
			update(newOptions: ChartOptions) {
				echartsInstance.setOption({
					...echartOptions.options,
					...newOptions.options
				});

				const { isLoading, loadingOptions } = {
					...DEFAULT_LOADING_OPTIONS,
					...echartOptions.options,
					...newOptions
				};

				if (isLoading) {
					echartsInstance.showLoading('default', loadingOptions);
				} else {
					echartsInstance.hideLoading();
				}
			}
		};
	}
</script>

<script lang="ts">
	export let options: echarts.EChartsOption;
	export let loadingOptions: EChartsLoadingOption = null!;

	export let isLoading: boolean;
	export let { theme, renderer } = DEFAULT_OPTIONS;
</script>

<div class="chart" use:chartable={{ renderer, theme, options, isLoading, loadingOptions }} />

<style>
	.chart {
		height: 100%;
		width: 100%;
	}
</style>
