<script lang="ts">
	import { RealtimeValueName } from '$lib/service/realtime-value-definitions';
	import {
		SocketSubscriptionBuilder,
		createRealtimeValueListener
	} from '$lib/service/subscriptions';
	import { onDestroy, onMount } from 'svelte';

	const subscriptionBuilder = new SocketSubscriptionBuilder();

	const protoRevIsEnabled = createRealtimeValueListener(
		subscriptionBuilder,
		RealtimeValueName.OsmosisProtoRevIsEnabled,
		() => []
	);
	const protoRevAdminAddress = createRealtimeValueListener(
		subscriptionBuilder,
		RealtimeValueName.OsmosisProtoRevAdminAddress,
		() => []
	);
	const protoRevDeveloperAddress = createRealtimeValueListener(
		subscriptionBuilder,
		RealtimeValueName.OsmosisProtoRevDeveloperAddress,
		() => []
	);

	onMount(async () => {
		await subscriptionBuilder.start();
	});
	onDestroy(() => {
		subscriptionBuilder.dispose();
	});
</script>

<p>isEnabled: {$protoRevIsEnabled}</p>
<p>Admin: {$protoRevAdminAddress}</p>
<p>Developer: {$protoRevDeveloperAddress}</p>
