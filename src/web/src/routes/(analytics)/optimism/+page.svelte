<script lang="ts">
	import { HubConnectionBuilder } from '@microsoft/signalr';
	import { onMount } from 'svelte';
	import { writable } from 'svelte/store';
	import RefreshAnimation from '../terra/RefreshAnimation.svelte';

	import blockIcon from '$lib/static/logo/block.svg';
	import gasStationIcon from '$lib/static/logo/gas-station.svg';
	import fireIcon from '$lib/static/logo/fire.svg';
	const price = writable<number>(0);
	var priceAnimation: RefreshAnimation;

	const peakBlockHeight = writable<number>(0);
	var peakBlockHeightAnimation: RefreshAnimation;

	const l1GasPrice = writable<number>(0);
	var l1GasPriceAnimation: RefreshAnimation;
	const l2GasPrice = writable<number>(0);
	var l2GasPriceAnimation: RefreshAnimation;

	onMount(async () => {
		let connection = new HubConnectionBuilder()
			.withUrl('/api/hub/optimism')
			.withAutomaticReconnect()
			.build();

		connection.on('Price', (newPrice) => {
			price.set(newPrice);
			priceAnimation.play();
		});
		connection.on('PeakBlockHeight', (newBlockHeight) => {
			peakBlockHeight.set(newBlockHeight);
			peakBlockHeightAnimation.play();
		});
		connection.on('GasPrice', (newL1GasPrice, newL2GasPrice) => {
			l1GasPrice.set(newL1GasPrice);
			l2GasPrice.set(newL2GasPrice);

			l1GasPriceAnimation.play();
			l2GasPriceAnimation.play();
		});

		await connection.start();
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
			<p>{Math.round($l1GasPrice / Math.pow(10, 6)) / 1000} gwei</p>
		</li>
		<li>
			<img alt="icon" class="h-1/2" src={gasStationIcon} />
			<RefreshAnimation bind:this={l2GasPriceAnimation} />
			<h2>L2 Gas Price</h2>
			<p>{$l2GasPrice / Math.pow(10, 9)} gwei</p>
		</li>
	</ul>
</div>
