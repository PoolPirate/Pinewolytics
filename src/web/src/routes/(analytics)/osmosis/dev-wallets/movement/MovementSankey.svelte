<script lang="ts" context="module">
	export interface SankeyLevelData {
		netFlowFromAbove: number;
		netFlowFromUnknown: number;

		netFlowToBelow: number;

		netFlowFromSwaps: number;
		netFlowToSwaps: number;

		netFlowFromIBC: number;
		netFlowToIBC: number;

		netFlowFromLP: number;
		netFlowToLP: number;
	}

	export interface SankeyNode {
		name: string;
		value?: number;
	}

	export interface SankeyLink {
		source: string;
		target: string;
		value: number;
	}
</script>

<script lang="ts">
	import type { SankeySeriesOption } from 'echarts';

	export let levels: SankeyLevelData[];

	const MINT_MECHANISM_NODE = 'Mint Mechanism';
	const IBC_IN_NODE = 'IBC IN';
	const IBC_OUT_NODE = 'IBC OUT';
	const SWAPPED_FROM_NODE = 'Swapped to other assets';
	const SWAPPED_TO_NODE = 'Swapped to OSMO';
	const DEVELOPER_WALLETS_NODE = 'Developer Wallets';
	const FURTHER_LEVELS_NODE = 'Further Levels';
	const UNKNOWN_SOURCE_NODE = 'Unknown Source';
	const LPED_NODE = 'Added to LP';
	const UNLPED_NODE = 'Removed from LP';

	const LEVEL_NAMES = ['Developer Wallets', '1st Level Subwallets', '2nd Level Subwallets'];

	const nodes: SankeyNode[] = [
		{ name: MINT_MECHANISM_NODE },
		{ name: IBC_IN_NODE },
		{ name: IBC_OUT_NODE },
		{ name: SWAPPED_FROM_NODE },
		{ name: SWAPPED_TO_NODE },
		{ name: FURTHER_LEVELS_NODE },
		{ name: UNKNOWN_SOURCE_NODE },
		{ name: LPED_NODE },
		{ name: UNLPED_NODE }
	];
	const links: SankeyLink[] = [
		{
			source: MINT_MECHANISM_NODE,
			target: DEVELOPER_WALLETS_NODE,
			value: levels[0].netFlowFromAbove ?? 0
		}
	];

	for (let i = 0; i < levels.length; i++) {
		const levelData = levels[i];

		const nodeName = LEVEL_NAMES[i];

		if (!nodes.map((x) => x.name).includes(nodeName)) {
			nodes.push({
				name: nodeName,
				value: levelData.netFlowFromAbove
			});
		}

		links.push({
			source: nodeName,
			target: IBC_OUT_NODE,
			value: levelData.netFlowToIBC
		});
		links.push({
			source: IBC_IN_NODE,
			target: nodeName,
			value: levelData.netFlowFromIBC
		});
		links.push({
			source: SWAPPED_TO_NODE,
			target: nodeName,
			value: levelData.netFlowFromSwaps
		});
		links.push({
			source: nodeName,
			target: SWAPPED_FROM_NODE,
			value: levelData.netFlowToSwaps
		});
		links.push({
			source: UNKNOWN_SOURCE_NODE,
			target: nodeName,
			value: levelData.netFlowFromUnknown
		});
		links.push({
			source: UNLPED_NODE,
			target: nodeName,
			value: levelData.netFlowFromLP
		});
		links.push({
			source: nodeName,
			target: LPED_NODE,
			value: levelData.netFlowToLP
		});

		if (levels.length == i + 1) {
			links.push({
				source: nodeName,
				target: FURTHER_LEVELS_NODE,
				value: levelData.netFlowToBelow
			});
		} else {
			links.push({
				source: nodeName,
				target: LEVEL_NAMES[i + 1],
				value: levelData.netFlowToBelow
			});
		}
	}

	const data = {
		nodes: nodes,
		links: links
	};

	const series: SankeySeriesOption = {
		type: 'sankey',
		data: data.nodes,
		links: data.links,
		emphasis: {
			focus: 'adjacency'
		},
		levels: [
			{
				depth: 0,
				itemStyle: {
					color: '#fbb4ae'
				},
				lineStyle: {
					color: 'source',
					opacity: 0.6
				}
			},
			{
				depth: 1,
				itemStyle: {
					color: '#b3cde3'
				},
				lineStyle: {
					color: 'source',
					opacity: 0.6
				}
			}
		],
		lineStyle: {
			curveness: 0.5
		}
	};
</script>

<style>
</style>
