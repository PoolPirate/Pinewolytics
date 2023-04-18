<script lang="ts">
	import SingleValueChart from '$lib/charts/SingleValueChart.svelte';
	import { RealtimeValueName } from '$lib/service/realtime-value-definitions';
	import {
		SocketSubscriptionBuilder,
		createRealtimeValueListener
	} from '$lib/service/subscriptions';
	import type { SeriesOption } from 'echarts';
	import { onDestroy, onMount } from 'svelte';
	import { writable } from 'svelte/store';
	import mintScanIcon from '$lib/static/logo/mintscan.jpg';

	const moduleAddress = 'osmo17qdmjdumw4xawam4g46gtwzle5rd4zwyfqvvza';
	const moduleInitializationEpoch = 635;
	const split1Percentage = 20;
	const split2Percentage = 10;
	const split3Percentage = 5;

	const subscriptionBuilder = new SocketSubscriptionBuilder();

	onMount(async () => {
		await subscriptionBuilder.start();
	});
	onDestroy(() => {
		subscriptionBuilder.dispose();
	});

	const protoRevIsEnabledChart = writable<SeriesOption | null>(null);
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

	$: makeProtoRevIsEnabledChart($protoRevIsEnabled);
	function makeProtoRevIsEnabledChart(isEnabled: boolean | null) {
		if (isEnabled == null) {
			return;
		}

		protoRevIsEnabledChart.set({
			type: 'gauge',
			data: [
				{
					name: 'Status',
					value: isEnabled ? 100 : 0
				}
			],
			progress: {
				show: true,
				itemStyle: {
					color: isEnabled ? 'green' : 'red'
				}
			}
		});
	}
</script>

<div class="grid grid-cols-1 gap-4">
	<div class="grid grid-cols-2 h-96 transparent-background p-2 items-center place-items-center">
		<SingleValueChart series={$protoRevIsEnabledChart} queryName={null} />
		<ul class="grid grid-cols-1 justify-center items-center gap-4">
			<li
				class="flex flex-row justify-between gap-4 p-2 border-2 border-black transparent-background h-20"
			>
				<div>
					<h3 class="font-bold text-lg">Module Address</h3>
					<p>{moduleAddress}</p>
				</div>

				<a
					href="https://www.mintscan.io/osmosis/account/{moduleAddress}"
					rel="noreferrer external"
					target="_blank"><img class="h-full" src={mintScanIcon} alt="View on Mintscan" /></a
				>
			</li>
			<li
				class="flex flex-row justify-between gap-4 p-2 border-2 border-black transparent-background h-20"
			>
				<div>
					<h3 class="font-bold text-lg">Admin Account</h3>
					<p>{$protoRevAdminAddress}</p>
				</div>

				<a
					href="https://www.mintscan.io/osmosis/account/{$protoRevAdminAddress}"
					rel="noreferrer external"
					target="_blank"><img class="h-full" src={mintScanIcon} alt="View on Mintscan" /></a
				>
			</li>
			<li
				class="flex flex-row justify-between gap-4 p-2 border-2 border-black transparent-background h-20"
			>
				<div>
					<h3 class="font-bold text-lg">Developer Account</h3>
					<p>{$protoRevDeveloperAddress}</p>
				</div>

				<a
					href="https://www.mintscan.io/osmosis/account/{$protoRevDeveloperAddress}"
					rel="noreferrer external"
					target="_blank"><img class="h-full" src={mintScanIcon} alt="View on Mintscan" /></a
				>
			</li>
		</ul>
	</div>

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
</div>
