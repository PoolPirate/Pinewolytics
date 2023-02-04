<script lang="ts">
	import { QueryName } from '$lib/service/query-definitions';
	import { createQueryListener, SocketSubscriptionBuilder } from '$lib/service/subscriptions';
	import { onDestroy, onMount } from 'svelte';

	const subscriptionBuilder = new SocketSubscriptionBuilder();

	const osmosisDevStakingRewardsQuery = createQueryListener(
		subscriptionBuilder,
		QueryName.OsmosisL5DevStakingRewards
	);
	const osmosisDevDelegationsQuery = createQueryListener(
		subscriptionBuilder,
		QueryName.OsmosisL5Delegations
	);

	const osmosisDevUndelegationsQuery = createQueryListener(
		subscriptionBuilder,
		QueryName.OsmosisL5DevUndelegations
	);

	onMount(async () => {
		await subscriptionBuilder.start();
	});
	onDestroy(() => {
		subscriptionBuilder.dispose();
	});
</script>

{#if $osmosisDevDelegationsQuery.length == 0}
	<p>Loading Total Delegations</p>
{:else}
	<p>Delegated:</p>
	<p>{$osmosisDevDelegationsQuery.reduce((total, curr) => total + curr.amount, 0)} $OSMO</p>
{/if}

{#if $osmosisDevUndelegationsQuery.length == 0}
	<p>Loading Total Undelegations</p>
{:else}
	<p>Undelegated:</p>
	<p>{$osmosisDevUndelegationsQuery.reduce((total, curr) => total + curr.amount, 0)} $OSMO</p>
{/if}

{#if $osmosisDevStakingRewardsQuery.length == 0}
	<p>Loading Total Rewards</p>
{:else}
	<p>Rewards</p>
	<p>{$osmosisDevStakingRewardsQuery.reduce((total, curr) => total + curr.amount, 0)} $OSMO</p>
{/if}
