<script lang="ts">
	import { RealtimeValueName } from '$lib/service/realtime-value-definitions';
	import {
		SocketSubscriptionBuilder,
		createRealtimeValueListener
	} from '$lib/service/subscriptions';
	import { onDestroy, onMount } from 'svelte';
	import { writable } from 'svelte/store';

	const moduleInitializationEpoch = 635;
	const split1Percentage = 20;
	const split2Percentage = 10;
	const split3Percentage = 5;

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
	const epochInfo = createRealtimeValueListener(
		subscriptionBuilder,
		RealtimeValueName.OsmosisEpochInfo,
		() => []
	);

	const split = writable<number>(0);
	$: if ($epochInfo != null) {
		split.set(
			$epochInfo.currentEpoch < moduleInitializationEpoch
				? 0
				: $epochInfo.currentEpoch <= moduleInitializationEpoch + 365
				? 1
				: $epochInfo.currentEpoch <= moduleInitializationEpoch + 730
				? 2
				: 3
		);
	}

	onMount(async () => {
		await subscriptionBuilder.start();
	});
	onDestroy(() => {
		subscriptionBuilder.dispose();
	});
</script>

<div class="grid grid-cols-1 p-2 transparent-background">
	<h2 class="text-xl font-bold">Fee Phase</h2>
	<ul
		class="flex flex-row p-2 text-center gap-2
			   [&>li]:border-2 [&>li]:border-emerald-800 [&>li]:w-full [&>li]:p-2"
	>
		<li class="transparent-background hover:border-green-400" class:bg-lime-400={$split == 1}>
			<h3 class="font-bold text-lg">Year 1 {$split == 1 ? '(Active)' : ''}</h3>
			<hr class="border-black border-2" />
			<p>Profit Share: {split1Percentage}%</p>
			<p>Epochs: {moduleInitializationEpoch} - {moduleInitializationEpoch + 365}</p>
		</li>
		<li class="transparent-background hover:border-green-400" class:bg-lime-400={$split == 2}>
			<h3 class="font-bold text-lg">Year 2 {$split == 2 ? '(Active)' : ''}</h3>
			<hr class="border-black border-2" />
			<p>Profit Share: {split2Percentage}%</p>
			<p>Epochs: {moduleInitializationEpoch + 366} - {moduleInitializationEpoch + 730}</p>
		</li>
		<li class="transparent-background hover:border-green-400" class:bg-lime-400={$split == 3}>
			<h3 class="font-bold text-lg">Beyond {$split == 3 ? '(Active)' : ''}</h3>
			<hr class="border-black border-2" />
			<p>Profit Share: {split3Percentage}%</p>
			<p>Epochs: {moduleInitializationEpoch + 731}+</p>
		</li>
	</ul>
</div>

<p>isEnabled: {$protoRevIsEnabled}</p>
<p>Admin: {$protoRevAdminAddress}</p>
<p>Developer: {$protoRevDeveloperAddress}</p>
