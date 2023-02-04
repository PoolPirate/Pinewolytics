<script lang="ts">
	import { HubConnectionBuilder } from '@microsoft/signalr';
	import { onDestroy, onMount } from 'svelte';
	import { writable } from 'svelte/store';
	import RefreshAnimation from '../../../lib/components/RefreshAnimation.svelte';

	import blockIcon from '$lib/static/logo/block.svg';
	import gasStationIcon from '$lib/static/logo/gas-station.svg';
	import fireIcon from '$lib/static/logo/fire.svg';
	import {
		createRealtimeValueListener,
		SocketSubscriptionBuilder
	} from '$lib/service/subscriptions';
	import { RealtimeValueName } from '$lib/service/realtime-value-definitions';

	const subscriptionBuilder = new SocketSubscriptionBuilder();

	const peakBlockHeight = createRealtimeValueListener(
		subscriptionBuilder,
		RealtimeValueName.OptimismBlockHeight,
		() => [peakBlockHeightAnimation]
	);
	var peakBlockHeightAnimation: RefreshAnimation;

	const gasPrices = createRealtimeValueListener(
		subscriptionBuilder,
		RealtimeValueName.OptimismGasPrices,
		() => [l1GasPriceAnimation, l2GasPriceAnimation]
	);
	var l1GasPriceAnimation: RefreshAnimation;
	var l2GasPriceAnimation: RefreshAnimation;

	onMount(async () => {
		await subscriptionBuilder.start();
	});
	onDestroy(async () => {
		subscriptionBuilder.dispose();
	});
</script>

<div
	class="ml-auto mr-auto bg-gray-200 mx-8 rounded-b-3xl p-4 opacity-75 h-full border border-t-0 w-10/12 lg:w-8/12 2xl:w-1/2"
>
	<ul
		class="text-center grid justify-items-center gap-4
			   [&>li]:bg-gray-400 [&>li]:p-3 [&>li]:rounded-xl [&>li]:w-full [&>li]:relative [&>li]
			   [&>li>h2]:font-bold
			   [&>li>svg]:absolute [&>li>svg]:w-12 [&>li>svg]:right-1
			   [&>li>img]:absolute [&>li>img]:h-12"
	>
		<li>
			<img alt="icon" class="h-1/2" src={blockIcon} />
			<RefreshAnimation bind:this={peakBlockHeightAnimation} />
			<h2>Peak Block Height</h2>
			<p>#{$peakBlockHeight}</p>
		</li>
		<li>
			<img alt="icon" class="h-1/2" src={fireIcon} />
			<RefreshAnimation bind:this={l1GasPriceAnimation} />
			<h2>L1 Gas Price</h2>
			<p>{Math.round($gasPrices?.l1GasPrice / Math.pow(10, 6)) / 1000} gwei</p>
		</li>
		<li>
			<img alt="icon" class="h-1/2" src={gasStationIcon} />
			<RefreshAnimation bind:this={l2GasPriceAnimation} />
			<h2>L2 Gas Price</h2>
			<p>{$gasPrices?.l2GasPrice / Math.pow(10, 9)} gwei</p>
		</li>
	</ul>
</div>
