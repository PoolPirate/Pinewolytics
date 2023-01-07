<script lang="ts">
	import SingleValueChart from '$lib/charts/SingleValueChart.svelte';
	import TimeSeriesChart from '$lib/charts/TimeSeriesChart.svelte';
	import type {
		OptimismGasMetricsDTO,
		OptimismL1SubmissionMetricsDTO
	} from '$lib/models/DTOs/OptimismDTO';
	import {
		createQueryListener,
		QueryName,
		QuerySubscriptionBuilder
	} from '$lib/service/querysubscription';
	import {
		DaySeriesToWeekSeriesByLast,
		DaySeriesToWeekSeriesBySum,
		type TimeSeriesEntry
	} from '$lib/service/transform';
	import { isWeeklyModeStoreName } from '$lib/utils/Utils';
	import type { SeriesOption } from 'echarts';
	import { getContext, onDestroy, onMount } from 'svelte';
	import { writable, type Readable } from 'svelte/store';

	const subscriptionBuilder = new QuerySubscriptionBuilder();

	onMount(async () => {
		await subscriptionBuilder.start();
	});
	onDestroy(() => {
		subscriptionBuilder.dispose();
	});

	const l1SubmissionsQuery = createQueryListener(
		subscriptionBuilder,
		QueryName.OptimismL1SubmissionsHistory
	);

	const gasMetricsHistoryQuery = createQueryListener(
		subscriptionBuilder,
		QueryName.OptimismGasMetricsHistory
	);

	const l1SubmissionsChart = writable<SeriesOption[]>([]);
	const monthlyTotalFeeChart = writable<SeriesOption>();
	const totalL1vsL2FeeChart = writable<SeriesOption>();

	const isWeeklyModeStore = getContext<Readable<boolean>>(isWeeklyModeStoreName);

	$: makeL1SubmissionsChart($l1SubmissionsQuery, $isWeeklyModeStore);
	function makeL1SubmissionsChart(values: OptimismL1SubmissionMetricsDTO[], isWeeklyMode: boolean) {
		if (values.length == 0) {
			return;
		}

		var totalSubmissionsSeries = values
			.sort((a, b) => new Date(a.timestamp).getTime() - new Date(b.timestamp).getTime())
			.map<TimeSeriesEntry>((x) => {
				return {
					timestamp: new Date(x.timestamp),
					value: x.totalSubmissions
				};
			});

		var newSubmissionsSeries = values
			.sort((a, b) => new Date(a.timestamp).getTime() - new Date(b.timestamp).getTime())
			.map<TimeSeriesEntry>((x, i, arr) => {
				return {
					timestamp: new Date(x.timestamp),
					value: x.totalSubmissions - (i == 0 ? 0 : arr[i - 1].totalSubmissions)
				};
			})
			.slice(1);

		if (isWeeklyMode) {
			totalSubmissionsSeries = DaySeriesToWeekSeriesByLast(totalSubmissionsSeries);
			newSubmissionsSeries = DaySeriesToWeekSeriesBySum(newSubmissionsSeries);
		}

		l1SubmissionsChart.set([
			{
				type: 'line',
				name: 'Total L1 Submissions',
				data: totalSubmissionsSeries.map((x) => [x.timestamp, x.value]),
				yAxisIndex: 0
			},
			{
				type: 'bar',
				name: 'New L1 Submissions',
				data: newSubmissionsSeries.map((x) => [x.timestamp, x.value]),
				yAxisIndex: 1
			}
		]);
	}

	$: makeMonthlyTotalFeeChart($gasMetricsHistoryQuery);
	function makeMonthlyTotalFeeChart(values: OptimismGasMetricsDTO[]) {
		if (values.length == 0) {
			return;
		}

		const gasFeeSeries = values
			.sort((a, b) => new Date(b.timestamp).getTime() - new Date(a.timestamp).getTime())
			.map((x) => {
				return {
					timestamp: new Date(x.timestamp),
					totalFee: x.totalL1GasFee + x.totalL2GasFee
				};
			});

		const boundaries = [];

		var offset = 0;

		for (let i = 0; i < 3; i++) {
			const lastDayIndex =
				gasFeeSeries.slice(offset).findIndex((value, index, arr) => {
					const nextMonth = arr[index + 1].timestamp.getMonth();
					const currMonth = value.timestamp.getMonth();
					return nextMonth != currMonth;
				}) + offset;

			offset = lastDayIndex + 1;

			boundaries.push(lastDayIndex);
		}

		var lastMonths = [
			gasFeeSeries[0].totalFee - gasFeeSeries[boundaries[0]].totalFee,
			gasFeeSeries[boundaries[0]].totalFee - gasFeeSeries[boundaries[1]].totalFee,
			gasFeeSeries[boundaries[1]].totalFee - gasFeeSeries[boundaries[2]].totalFee
		];

		const limit = Math.max(...lastMonths);

		const gaugeData = [
			{
				value: lastMonths[0],
				name: new Date(gasFeeSeries[boundaries[0]].timestamp).toLocaleDateString('en', {
					month: 'long'
				}),
				title: {
					offsetCenter: ['0%', '-33%']
				},
				detail: {
					valueAnimation: true,
					offsetCenter: ['0%', '-20%']
				}
			},
			{
				value: lastMonths[1],
				name: new Date(gasFeeSeries[boundaries[1]].timestamp).toLocaleDateString('en', {
					month: 'long'
				}),
				title: {
					offsetCenter: ['0%', '-3%']
				},
				detail: {
					valueAnimation: true,
					offsetCenter: ['0%', '10%']
				}
			},
			{
				value: lastMonths[2],
				itemStyle: {
					color: 'purple'
				},
				name: new Date(gasFeeSeries[boundaries[2]].timestamp).toLocaleDateString('en', {
					month: 'long'
				}),
				title: {
					offsetCenter: ['0%', '27%']
				},
				detail: {
					valueAnimation: true,
					offsetCenter: ['0%', '40%']
				}
			}
		];

		monthlyTotalFeeChart.set({
			type: 'gauge',
			startAngle: 90,
			endAngle: -270,
			max: limit,
			pointer: {
				show: false
			},
			progress: {
				show: true,
				overlap: false,
				roundCap: true,
				clip: false
			},
			axisLine: {
				lineStyle: {
					width: 40
				}
			},
			splitLine: {
				show: false,
				distance: 0,
				length: 10
			},
			axisTick: {
				show: false
			},
			axisLabel: {
				show: false
			},
			data: gaugeData,
			title: {
				fontSize: 14
			},
			detail: {
				width: 100,
				height: 14,
				fontSize: 14,
				color: 'inherit',
				borderColor: 'inherit',
				borderRadius: 20,
				borderWidth: 1,
				formatter: (value) => Math.round(value).toLocaleString() + ' $LUNA'
			}
		});
	}

	$: makeTotalL1vsL2FeeChart($gasMetricsHistoryQuery);
	function makeTotalL1vsL2FeeChart(values: OptimismGasMetricsDTO[]) {
		if (values.length == 0) {
			return;
		}

		const sortedSeries = values.sort(
			(a, b) => new Date(b.timestamp).getTime() - new Date(a.timestamp).getTime()
		);

		const totalL1GasFee = sortedSeries[0].totalL1GasFee;
		const totalL2GasFee = sortedSeries[0].totalL2GasFee;

		totalL1vsL2FeeChart.set({
			type: 'pie',
			radius: ['40%', '70%'],
			label: {
				show: false,
				position: 'center'
			},
			emphasis: {
				label: {
					show: true,
					fontSize: 40,
					fontWeight: 'bold'
				}
			},
			tooltip: {
				show: true,
				trigger: 'item',
				formatter: (x) => (x.data as any)?.value.toLocaleString() + ' $ETH'
			},
			data: [
				{
					value: totalL1GasFee,
					name: 'Total L1 Gas Fee'
				},
				{
					value: totalL2GasFee,
					name: 'Total L2 Gas Fee'
				}
			]
		});
	}
</script>

<div class="grid grid-cols-1 mt-2 p-2 w-full transparent-background rounded-lg">
	<h1 class="text-xl font-bold">Optimism Gas Fees</h1>
	<p>
		Gas fees in Optimism are made up of 2 components. The L2 execution fee as well as the L1
		security fee. The execution fee is typically very low while the L1 fee depends entirely on the
		gas price on Ethereum.
	</p>
	<p>
		The L2 fee represents the actual execution of the transaction while the L1 fee is used to pay a
		portion of the <b>L1 Submission</b>.
	</p>

	<hr class="font-black" />

	<div class="my-3">
		<a
			class="text-center p-2 border border-white rounded-md bg-blue-400"
			href="https://help.optimism.io/hc/en-us/articles/4411895794715-How-do-transaction-fees-on-Optimism-work-"
			target="_blank"
			rel="noreferrer">Gas on Optimism</a
		>
		<a
			class="text-center p-2 mt-2 border border-white rounded-md bg-blue-400"
			href="https://ethereum.org/en/developers/docs/scaling/"
			target="_blank"
			rel="noreferrer">Optimistic Rollups</a
		>
	</div>
</div>
<div class="mt-3 p-3 transparent-background rounded-lg">
	<div class="grid grid-cols-1 lg:grid-cols-2">
		<SingleValueChart
			title={{ text: 'Total L1 vs L2 Fees' }}
			class="h-128"
			showToolTip={true}
			showLegend={true}
			series={$totalL1vsL2FeeChart}
		/>
		<SingleValueChart
			title={{ text: 'Total Fees Paid In Last Months' }}
			class="h-128"
			series={$monthlyTotalFeeChart}
		/>
	</div>

	<TimeSeriesChart
		yAxis={[{}, {}]}
		legend={{}}
		title={{ text: 'L1 Submissions' }}
		class="h-128"
		series={$l1SubmissionsChart}
	/>
</div>

<style>
	.transparent-background {
		position: relative;
	}

	.transparent-background::before {
		content: ' ';
		position: absolute;
		left: 0;
		right: 0;
		top: 0;
		bottom: 0;
		background: white;
		opacity: 50%;
		border-radius: inherit;
		pointer-events: none;
		z-index: -1;
	}
</style>
