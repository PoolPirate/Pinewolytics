<script lang="ts">
	import BackToMenuLink from '$lib/links/BackToMenuLink.svelte';
	import AnalyticsLayout from '../AnalyticsLayout.svelte';
	import luna2logo from '$lib/static/logo/luna2.png';
	import { beforeUpdate } from 'svelte';

	beforeUpdate(() => {
		const rootElement = document.querySelector(':root')! as any;

		rootElement.style.setProperty('--color1', '#fe0420');
		rootElement.style.setProperty('--color2', '#ffffff');
	});
</script>

<AnalyticsLayout>
	<nav slot="nav" class="h-full w-full flex flex-row items-center">
		<BackToMenuLink />

		<h2 class="ml-auto font-bold text-xl">Terra</h2>
		<img src={luna2logo} alt="Luna 2.0" class="ml-6 max-w-full max-h-full p-1" />
	</nav>
	<slot slot="main" />
</AnalyticsLayout>
