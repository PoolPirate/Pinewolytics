<script lang="ts">
	import type { OsmosisProtoRevTransactionDTO } from '$lib/models/DTOs/OsmosisDTOs';
	import { getOsmosisProtoRevTransactions } from '$lib/service/queries';
	import { writable } from 'svelte/store';

	var selectedAddress: string;

	const protoRevTransactions = writable<OsmosisProtoRevTransactionDTO[] | null>(null);

	$: loadProtoRevTransactions(selectedAddress);
	async function loadProtoRevTransactions(selectedAddress: string) {
		protoRevTransactions.set(null);

		const txs = await getOsmosisProtoRevTransactions(selectedAddress);
		protoRevTransactions.set(txs);
	}
</script>

<input type="text" bind:value={selectedAddress} />

<p>{selectedAddress}</p>

<table class:hidden={$protoRevTransactions == null}>
	<thead>
		<tr>
			<th>Transaction</th>
			<th>Revenue at tx time ($USD)</th>
		</tr>
	</thead>
	<tbody>
		{#each $protoRevTransactions ?? [] as tx}
			<tr>
				<td>
					<a
						class="text-blue-400"
						rel="noreferrer external"
						href="https://www.mintscan.io/osmosis/txs/{tx.hash}"
						>{tx.hash.substring(0, 6)}...{tx.hash.substring(tx.hash.length - 6, tx.hash.length)}</a
					>
				</td>
				<td>
					{#each tx.swaps as swap}
						<p>{swap.profit.amount} {swap.profit.denom} ({swap.profitUSD}$)</p>
					{/each}
				</td>
			</tr>
		{/each}
	</tbody>
</table>
