<script lang="ts">
	import BackToMenuLink from '$lib/links/BackToMenuLink.svelte';
	import AnalyticsLayout from '../AnalyticsLayout.svelte';
	import optimismLogo from '$lib/static/logo/optimism.svg';
	import { beforeUpdate } from 'svelte';
	import NavLink from '$lib/links/NavLink.svelte';

	beforeUpdate(() => {
		const rootElement = document.querySelector(':root')! as any;

		rootElement.style.setProperty('--color1', '#ffffff');
		rootElement.style.setProperty('--color2', '#fe0420');
	});
</script>

<AnalyticsLayout>
	<aside slot="sidebar">
		<NavLink href="/optimism">Overview</NavLink>
		<NavLink href="/optimism/transactions">Transactions</NavLink>
		<NavLink href="/optimism/op-token">$OP Token</NavLink>
		<BackToMenuLink />
	</aside>

	<nav slot="nav" class="h-full w-full flex flex-row items-center">
		<h2 class="ml-auto font-bold text-xl">Optimism</h2>
		<img src={optimismLogo} alt="Optimism" class="ml-6 max-w-full max-h-full p-1" />
	</nav>
	<nav />
	<slot slot="main" />
</AnalyticsLayout>
