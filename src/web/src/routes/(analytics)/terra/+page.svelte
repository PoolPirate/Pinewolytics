<script lang="ts">
	import { HubConnectionBuilder } from '@microsoft/signalr';
	import { onMount } from 'svelte';
	import { writable } from 'svelte/store';

	const blockHeight = writable<number>(0);
	const price = writable<number>(0);

	onMount(async () => {
		let connection = new HubConnectionBuilder()
			.withUrl('/api/hub/lunadata')
			.withAutomaticReconnect()
			.build();

		connection.on('UpdatePrice', (newPrice) => {
			price.set(newPrice);
		});
		connection.on('UpdateBlockHeight', (newBlockHeight) => {
			blockHeight.set(newBlockHeight);
		});

		await connection.start();
	});
</script>

<div class="bg-gray-400 w-11/12 m-8 rounded-3xl p-4 opacity-75 h-full border">
	<p class="text-center">Add charts here</p>
	<p>Block Height - #{$blockHeight}</p>
	<p>Price - {$price}$</p>
</div>
