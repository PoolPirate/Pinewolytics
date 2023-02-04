<script lang="ts">
	import { QueryName } from '$lib/service/query-definitions';
	import { createQueryListener, SocketSubscriptionBuilder } from '$lib/service/subscriptions';
	import { onDestroy, onMount } from 'svelte';

	const subscriptionBuilder = new SocketSubscriptionBuilder();

	const osmosisDevLPJoinsQuery = createQueryListener(
		subscriptionBuilder,
		QueryName.OsmosisL5DevLPJoins
	);

	onMount(async () => {
		await subscriptionBuilder.start();
	});
	onDestroy(() => {
		subscriptionBuilder.dispose();
	});
</script>
