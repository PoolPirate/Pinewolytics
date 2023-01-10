<script lang="ts">
	import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
	import { onDestroy, onMount } from 'svelte';
	import { writable } from 'svelte/store';
	import RefreshAnimation from '../../../lib/components/RefreshAnimation.svelte';

	import blockIcon from '$lib/static/logo/block.svg';
	import clockIcon from '$lib/static/logo/clock.svg';
	import priceIcon from '$lib/static/logo/price.svg';
	import pileIcon from '$lib/static/logo/pile.svg';
	import smallPileIcon from '$lib/static/logo/small_pile.webp';

	const blockHeight = writable<number>(0);
	var blockHeightAnimation: RefreshAnimation;
	const blockTimestamp = writable<Date>(new Date());
	var blockTimestampAnimation: RefreshAnimation;
	const price = writable<number>(0);
	var priceAnimation: RefreshAnimation;
	const totalSupply = writable<number>(0);
	var totalSupplyAnimation: RefreshAnimation;
	const circulatingSupply = writable<number>(0);
	var circulatingSupplyAnimation: RefreshAnimation;

	var connection: HubConnection;

	onMount(async () => {
		connection = new HubConnectionBuilder()
			.withUrl('/api/hub/luna')
			.withAutomaticReconnect()
			.build();

		connection.on('Price', (newPrice) => {
			price.set(newPrice);
			priceAnimation.play();
		});
		connection.on('PeakBlockHeight', (newBlockHeight) => {
			blockHeight.set(newBlockHeight);

			blockHeightAnimation.play();
		});
		connection.on('PeakBlockTimestamp', (newBlockTimestamp) => {
			blockTimestamp.set(new Date(newBlockTimestamp));
			blockTimestampAnimation.play();
		});
		connection.on('TotalSupply', (newTotalSupply) => {
			totalSupply.set(newTotalSupply);
			totalSupplyAnimation.play();
		});
		connection.on('CirculatingSupply', (newCirculatingSupply) => {
			circulatingSupply.set(newCirculatingSupply);
			circulatingSupplyAnimation.play();
		});

		await connection.start();
	});
	onDestroy(() => {
		if (connection == null) {
			return;
		}

		connection.stop();
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
			<RefreshAnimation bind:this={blockHeightAnimation} />
			<h2>Block Height</h2>
			<p>{$blockHeight}</p>
		</li>
		<li>
			<img alt="icon" class="h-1/2" src={clockIcon} />
			<RefreshAnimation bind:this={blockTimestampAnimation} />
			<h2>Block Timestamp</h2>
			<p>{$blockTimestamp.toLocaleString()}</p>
		</li>
		<li>
			<img alt="icon" class="h-1/2" src={priceIcon} />
			<RefreshAnimation bind:this={priceAnimation} />
			<h2>Price</h2>
			<p>{$price}$</p>
		</li>
		<li>
			<img alt="icon" class="h-1/2" src={pileIcon} />
			<RefreshAnimation bind:this={totalSupplyAnimation} />
			<h2>Total Supply</h2>
			<p>{Math.round($totalSupply / 1000000).toLocaleString()} $LUNA</p>
		</li>
		<li>
			<img alt="icon" class="h-1/2" src={smallPileIcon} />
			<RefreshAnimation bind:this={circulatingSupplyAnimation} />
			<h2>Circulating Supply</h2>
			<p>{Math.round($circulatingSupply / 1000000).toLocaleString()} $LUNA</p>
		</li>
	</ul>
</div>
