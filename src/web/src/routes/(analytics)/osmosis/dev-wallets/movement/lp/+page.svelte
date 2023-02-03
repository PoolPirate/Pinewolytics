<script lang="ts">
	import {
		createQueryListener,
		QueryName,
		QuerySubscriptionBuilder
	} from '$lib/service/querysubscription';
	import { onDestroy, onMount } from 'svelte';

	const subscriptionBuilder = new QuerySubscriptionBuilder();

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
