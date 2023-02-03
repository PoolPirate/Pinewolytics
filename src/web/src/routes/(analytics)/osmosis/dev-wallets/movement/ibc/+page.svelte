<script lang="ts">
	import {
		createQueryListener,
		QueryName,
		QuerySubscriptionBuilder
	} from '$lib/service/querysubscription';
	import { onDestroy, onMount } from 'svelte';

	const subscriptionBuilder = new QuerySubscriptionBuilder();

	const osmosisDevIBCTransfersQuery = createQueryListener(
		subscriptionBuilder,
		QueryName.OsmosisL5DevIBCTransfers
	);

	onMount(async () => {
		await subscriptionBuilder.start();
	});
	onDestroy(() => {
		subscriptionBuilder.dispose();
	});
</script>
