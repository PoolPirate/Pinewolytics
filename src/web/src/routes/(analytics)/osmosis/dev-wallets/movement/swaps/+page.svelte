<script lang="ts">
	import {
		createQueryListener,
		QueryName,
		QuerySubscriptionBuilder
	} from '$lib/service/querysubscription';
	import { onDestroy, onMount } from 'svelte';

	const subscriptionBuilder = new QuerySubscriptionBuilder();

	const osmosisDevSwapsQuery = createQueryListener(
		subscriptionBuilder,
		QueryName.OsmosisL5DevSwaps
	);

	onMount(async () => {
		await subscriptionBuilder.start();
	});
	onDestroy(() => {
		subscriptionBuilder.dispose();
	});
</script>
