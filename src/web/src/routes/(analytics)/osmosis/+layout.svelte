<script lang="ts">
	import BackToMenuLink from '$lib/links/BackToMenuLink.svelte';
	import AnalyticsLayout from '../AnalyticsLayout.svelte';
	import osmologo from '$lib/static/logo/osmosis.png';
	import { beforeUpdate } from 'svelte';

	beforeUpdate(() => {
		const rootElement = document.querySelector(':root')! as any;

		rootElement.style.setProperty('--color1', '#f200c9');
		rootElement.style.setProperty('--color2', '#0602bf');
	});
</script>

<AnalyticsLayout>
	<nav slot="nav" class="h-full w-full flex flex-row items-center">
		<h2 class="ml-auto font-bold text-xl">Osmosis</h2>
		<img src={osmologo} alt="Luna 2.0" class="ml-6 max-w-full max-h-full p-1" />
	</nav>
	<slot slot="main" />
</AnalyticsLayout>
