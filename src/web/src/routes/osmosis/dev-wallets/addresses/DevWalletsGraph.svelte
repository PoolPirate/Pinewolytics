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
	import { Chart } from 'svelte-echarts';
	import type { EChartsOptions } from 'svelte-echarts';
	import type { OsmosisNetTransferDTO } from '$lib/models/OsmosisDTOs';
	import type { GraphSeriesOption } from 'echarts';

    export let wallets: DeveloperWallet[];
    export let transfers: OsmosisNetTransferDTO[];
    export let depth: number;

    $: createNodes(wallets);
    $: createLinks(transfers);

    function createNodes(wallets: DeveloperWallet[]) {
        const max = wallets.reduce((max, curr, _) => Math.max(max, curr.amount), 0);
        const min = 0;

        (options.series as GraphSeriesOption).data = wallets.map(wallet => {
            return {
                name: wallet.address,
                symbolSize: transfers.length == 0 ? 12 : map(wallet.amount, min, max, 5, 60),
                category: wallet.level.toString()
            } as GraphNode;
        });;

        console.log(options);
    }
    function createLinks(transfers: OsmosisNetTransferDTO[]) {
        var links = transfers
            .filter(x => x.receiver != x.sender)
            .map(transfer => {
            const senderIdx = wallets.findIndex(wallet => wallet.address == transfer.sender);
            const receiverIdx = wallets.findIndex(wallet => wallet.address == transfer.receiver);

            if (wallets[senderIdx].level < wallets[receiverIdx].level) {
                wallets[receiverIdx].amount += transfer.amount;
            }
            if (wallets[senderIdx].level == wallets[receiverIdx].level) {
                wallets[receiverIdx].amount += transfer.amount;
                wallets[senderIdx].amount -= transfer.amount;
            }
            if (wallets[senderIdx].level > wallets[receiverIdx].level) {
                wallets[senderIdx].amount -= transfer.amount;
                return null;
            }

            return {
                source: transfer.sender,
                target: transfer.receiver,
            } as GraphLink;
        }).filter(x => x !== null).map(x => x!);

        wallets = wallets.filter(x => x.amount > 0);
        links = links
            .filter(link => wallets.some(x => x.address == link.source))
            .filter(link => wallets.some(x => x.address == link.target));

        (options.series as GraphSeriesOption).links = links;

        createNodes(wallets);
    }
        
    const categories = [...Array(depth).keys()].map(i => 
    {
        return {
            name: i.toString(),
        } as GraphCategory;
    });

	const options: EChartsOptions = {
        title: {
            text: 'Basic Graph'
        },
        tooltip: {
            show: false
        },
        legend: [
        {
            data: categories.map(x => x.name),
        }],
		series: {
            type: 'graph',
            layout: 'force',
            roam: true,
            force: {
                repulsion: 100,
            },

            edgeSymbol: ['circle', 'arrow'],
            edgeSymbolSize: [3, 10],
            lineStyle: {
                width: 4,
                color: "gray",
                opacity: 1
            },

            data: [],
            links: [],
            categories: categories
        }
	};

    export function clamp(input: number, min: number, max: number): number {
        return input < min ? min : input > max ? max : input;
    }

    export function map(current: number, in_min: number, in_max: number, out_min: number, out_max: number): number {
        const mapped: number = ((current - in_min) * (out_max - out_min)) / (in_max - in_min) + out_min;
        return clamp(mapped, out_min, out_max);
    }
</script>

<Chart {options} />

<style>
</style>
