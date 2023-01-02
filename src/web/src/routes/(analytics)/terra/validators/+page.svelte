<script lang="ts">
	import { QueryName, QuerySubscriptionBuilder } from '$lib/service/querysubscription';
	import { onMount } from 'svelte';

	onMount(async () => {
		await new QuerySubscriptionBuilder()
			.addQuery(QueryName.TerraValidatorCountHistory, (data) => {
				console.log('Received new data ' + data);
			})
			.start();
	});
</script>
