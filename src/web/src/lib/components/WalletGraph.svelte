<script lang="ts" context="module">
	interface GraphNode {
		name: string;
		symbolSize: number;
		category: string;
	}
	interface GraphLink {
		source: string;
		target: string;
	}
	interface GraphCategory {
		name: string;
	}

	export interface DeveloperWallet {
		address: string;
		level: number;
		amount: number;
	}
</script>

<script lang="ts">
	import type { OsmosisNetTransferDTO } from '$lib/models/DTOs/OsmosisDTOs';

	import type { EChartsOption } from 'echarts';

	import type { GraphSeriesOption, LegendComponentOption } from 'echarts';
	import Chart, { type EChartsLoadingOption } from './Chart.svelte';

	export let wallets: DeveloperWallet[];
	export let transfers: OsmosisNetTransferDTO[];
	export let depth: number;
	export let isLoading: boolean;

	export { clazz as class };
	let clazz: string;

	$: createNodes(wallets);
	$: createLinks(transfers);
	$: createCategories(depth);

	function createNodes(wallets: DeveloperWallet[]) {
		const max = wallets.reduce((max, curr, _) => Math.max(max, curr.amount), 0);
		const min = wallets.reduce((min, curr, _) => Math.min(min, curr.amount), 0);

		(options.series as GraphSeriesOption).data = wallets.map((wallet) => {
			return {
				name: wallet.address,
				symbolSize:
					transfers.length == 0
						? 12
						: map(
								wallet.amount == 0
									? transfers
											.filter((x) => x.sender == wallet.address)
											.reduce((total, curr, _) => total + curr.amount, 0)
									: wallet.amount,
								min,
								max,
								5,
								110
						  ),
				category: wallet.level.toString()
			} as GraphNode;
		});
	}
	function createLinks(transfers: OsmosisNetTransferDTO[]) {
		var links = transfers
			.filter((x) => x.receiver != x.sender)
			.map((transfer) => {
				const senderIdx = wallets.findIndex((wallet) => wallet.address == transfer.sender);
				const receiverIdx = wallets.findIndex((wallet) => wallet.address == transfer.receiver);

				if (wallets[senderIdx].level < wallets[receiverIdx].level) {
					wallets[receiverIdx].amount += transfer.amount;
				}
				if (wallets[senderIdx].level == wallets[receiverIdx].level) {
					wallets[receiverIdx].amount += transfer.amount;
					wallets[senderIdx].amount -= transfer.amount;
				}
				if (wallets[senderIdx].level > wallets[receiverIdx].level) {
					wallets[senderIdx].amount -= transfer.amount;
				}

				return {
					source: transfer.sender,
					target: transfer.receiver
				} as GraphLink;
			});

		(options.series as GraphSeriesOption).links = links;

		createNodes(wallets);
	}
	function createCategories(depth: number) {
		const categories = [...Array(depth).keys()].map((i) => {
			return {
				name: i.toString()
			} as GraphCategory;
		});

		(options.legend as LegendComponentOption).data = categories.map((x) => x.name);
		(options.series as GraphSeriesOption).categories = categories;
	}

	const options: EChartsOption = {
		tooltip: {
			show: false
		},
		legend: [
			{
				data: []
			}
		],
		series: {
			type: 'graph',
			layout: 'force',
			roam: true,
			force: {
				repulsion: 250,
				initLayout: 'circular'
			},

			edgeSymbol: ['circle', 'arrow'],
			edgeSymbolSize: [3, 9],
			lineStyle: {
				width: 3,
				color: 'black',
				opacity: 0.2
			},

			data: [],
			links: [],
			categories: []
		}
	};

	export function clamp(input: number, min: number, max: number): number {
		return input < min ? min : input > max ? max : input;
	}

	export function map(
		current: number,
		in_min: number,
		in_max: number,
		out_min: number,
		out_max: number
	): number {
		const mapped: number = ((current - in_min) * (out_max - out_min)) / (in_max - in_min) + out_min;
		return clamp(mapped, out_min, out_max);
	}
</script>

<Chart {isLoading} {options} class={clazz} />
