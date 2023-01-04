<script lang="ts">
	import { HubConnectionBuilder } from '@microsoft/signalr';
	import { onMount } from 'svelte';
	import { writable } from 'svelte/store';
	import RefreshAnimation from '../terra/RefreshAnimation.svelte';

	const price = writable<number>(0);
	var priceAnimation: RefreshAnimation;

	onMount(async () => {
		let connection = new HubConnectionBuilder()
			.withUrl('/api/hub/optimism')
			.withAutomaticReconnect()
			.build();

		connection.on('Price', (newPrice) => {
			price.set(newPrice);
			priceAnimation.play();
		});

		await connection.start();
	});
</script>

<div
	class="ml-auto mr-auto bg-gray-200 mx-8 rounded-b-3xl p-4 opacity-75 h-full border border-t-0 w-10/12 lg:w-8/12 2xl:w-1/2"
>
	<ul
		class="text-center grid justify-items-center 2xl:grid-cols-3 gap-4
			   [&>li]:bg-gray-400 [&>li]:p-3 [&>li]:rounded-xl [&>li]:w-full [&>li]:relative [&>li]
			   [&>li>h2]:font-bold
			   [&>li>svg]:absolute [&>li>svg]:w-12"
	>
		<li>
			<RefreshAnimation bind:this={priceAnimation} />
			<h2>Price</h2>
			<p>{$price}$</p>
		</li>
	</ul>
</div>
