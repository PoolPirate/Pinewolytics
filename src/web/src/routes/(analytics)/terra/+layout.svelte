<script lang="ts">
	import BackToMenuLink from '$lib/links/BackToMenuLink.svelte';
	import AnalyticsLayout from '../AnalyticsLayout.svelte';
	import luna2logo from '$lib/static/logo/luna2.png';
	import { beforeUpdate } from 'svelte';
	import NavLink from '$lib/links/NavLink.svelte';
	import LinkBumper from '$lib/links/LinkBumper.svelte';

	beforeUpdate(() => {
		const rootElement = document.querySelector(':root')! as any;

		rootElement.style.setProperty('--color1', '#f2e373');
		rootElement.style.setProperty('--color2', '#de3633');
	});
</script>

<AnalyticsLayout>
	<aside slot="sidebar">
		<NavLink href="/terra">Overview</NavLink>
		<NavLink href="/terra/transactions/speed">Speed</NavLink>
		<NavLink href="/terra/transactions/fees">Transaction Fees</NavLink>
		<NavLink href="/terra/wallets">Wallets</NavLink>

		<LinkBumper />
		<NavLink href="/terra/about">About</NavLink>
		<BackToMenuLink />
	</aside>

	<nav slot="nav" class="h-full w-full flex flex-row items-center">
		<h2 class="ml-auto font-bold text-xl">Terra</h2>
		<img src={luna2logo} alt="Luna 2.0" class="ml-6 max-w-full max-h-full p-1" />
	</nav>
	<div slot="main" class="flex flex-col p-3 w-full">
		<slot />
	</div>
</AnalyticsLayout>
