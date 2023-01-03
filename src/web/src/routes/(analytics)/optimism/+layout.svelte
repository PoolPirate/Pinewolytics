<script lang="ts">
	import BackToMenuLink from '$lib/links/BackToMenuLink.svelte';
	import AnalyticsLayout from '../AnalyticsLayout.svelte';
	import optimismLogo from '$lib/static/logo/optimism.svg';
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

		<h2 class="ml-auto font-bold text-xl">Optimism</h2>
		<img src={optimismLogo} alt="Optimism" class="ml-6 max-w-full max-h-full p-1" />
	</nav>
	<slot slot="main" />
</AnalyticsLayout>
