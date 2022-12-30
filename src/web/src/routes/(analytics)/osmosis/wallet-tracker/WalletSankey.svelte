<script lang="ts" context="module">
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
	import type { EChartsOption } from 'echarts';
	import type { SankeySeriesOption } from 'echarts';
	import type { OsmosisFlowSankeyDTO } from '$lib/models/OsmosisDTOs';
	import Chart from '$lib/components/Chart.svelte';

	export let parameters: OsmosisFlowSankeyDTO;

	const USER_WALLET_NODE = 'Your Wallet';
	const SWAP_OUT_NODE = 'Swapped to other assets';
	const SWAP_IN_NODE = 'Swapped from other assets';
	const LP_DEPOSIT_NODE = 'Deposited into LP';
	const LP_EXIT_NODE = 'Removed from LP';
	const LP_REWARDS_NODE = 'LP Rewards';
	const IBC_IN_NODE = 'Inbound IBC Transfer';
	const IBC_OUT_NODE = 'Outbound IBC Transfer';
	const TRANSFER_IN_NODE = 'Inbound Transfer';
	const TRANSFER_OUT_NODE = 'Outbound  Transfer';
	const STAKING_REWARDS_NODE = 'Staking Rewards';
	const STAKED_NODE = 'Staked';
	const UNSTAKED_NODE = 'Unstaked';

	const nodes: SankeyNode[] = [
		{ name: USER_WALLET_NODE },
		{ name: SWAP_OUT_NODE },
		{ name: SWAP_IN_NODE, value: parameters.netSwapIn },
		{ name: LP_DEPOSIT_NODE, value: parameters.netLPDeposit },
		{ name: LP_EXIT_NODE, value: parameters.netLPExit },
		{ name: LP_REWARDS_NODE, value: 0 },
		{ name: IBC_IN_NODE, value: parameters.netIBCIn },
		{ name: IBC_OUT_NODE, value: parameters.netIBCOut },
		{ name: TRANSFER_IN_NODE, value: parameters.netTransferIn },
		{ name: TRANSFER_OUT_NODE, value: parameters.netTransferOut },
		{ name: STAKING_REWARDS_NODE, value: parameters.netStakingRewards },
		{ name: STAKED_NODE, value: parameters.netStaked },
		{ name: UNSTAKED_NODE, value: parameters.netUnstaked }
	];

	const links: SankeyLink[] = [
		{
			source: SWAP_IN_NODE,
			target: USER_WALLET_NODE,
			value: parameters.netSwapIn
		},
		{
			source: USER_WALLET_NODE,
			target: SWAP_OUT_NODE,
			value: parameters.netSwapOut
		},
		{
			source: LP_EXIT_NODE,
			target: USER_WALLET_NODE,
			value: parameters.netLPExit
		},
		{
			source: LP_REWARDS_NODE,
			target: USER_WALLET_NODE,
			value: 0
		},
		{
			source: IBC_IN_NODE,
			target: USER_WALLET_NODE,
			value: parameters.netIBCIn
		},
		{
			source: TRANSFER_IN_NODE,
			target: USER_WALLET_NODE,
			value: parameters.netTransferIn
		},
		{
			source: STAKING_REWARDS_NODE,
			target: USER_WALLET_NODE,
			value: parameters.netStakingRewards
		},
		{
			source: UNSTAKED_NODE,
			target: USER_WALLET_NODE,
			value: parameters.netUnstaked
		},
		{
			source: USER_WALLET_NODE,
			target: LP_DEPOSIT_NODE,
			value: parameters.netLPDeposit
		},
		{
			source: USER_WALLET_NODE,
			target: IBC_OUT_NODE,
			value: parameters.netIBCOut
		},
		{
			source: USER_WALLET_NODE,
			target: TRANSFER_OUT_NODE,
			value: parameters.netTransferOut
		},
		{
			source: USER_WALLET_NODE,
			target: STAKED_NODE,
			value: parameters.netStaked
		}
	];

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

	const options: EChartsOption = {
		tooltip: {
			trigger: 'item',
			triggerOn: 'mousemove'
		},
		series: series
	};
</script>

<p class="mb-48">OUTPUT</p>
<Chart isLoading={false} {options} />

<style>
</style>
