<script lang="ts">
	import BackToMenuLink from '$lib/links/BackToMenuLink.svelte';
	import AnalyticsLayout from '../AnalyticsLayout.svelte';
	import luna2logo from '$lib/static/logo/luna2.png';
	import { beforeUpdate, setContext } from 'svelte';
	import NavLink from '$lib/links/NavLink.svelte';
	import LinkBumper from '$lib/links/LinkBumper.svelte';
	import ToggleSwitch from '$lib/components/ToggleSwitch.svelte';
	import { writable } from 'svelte/store';
	import { isWeeklyModeStoreName } from '$lib/utils/Utils';
	import NavLinkGroup from '$lib/links/NavLinkGroup.svelte';

	beforeUpdate(() => {
		const rootElement = document.querySelector(':root')! as any;

		rootElement.style.setProperty('--color1', '#f2e373');
		rootElement.style.setProperty('--color2', '#de3633');
	});
</script>

<AnalyticsLayout>
	<aside slot="sidebar">
		<NavLink href="/terra">Overview</NavLink>
		<NavLinkGroup title="Wallets">
			<NavLink href="/terra/wallets/activity">Activity</NavLink>
			<NavLink href="/terra/wallets/richlist">#100 Richlist</NavLink>
		</NavLinkGroup>

		<NavLinkGroup title="Transactions">
			<NavLink href="/terra/transactions/speed">Speed</NavLink>
			<NavLink href="/terra/transactions/fees">Fees</NavLink>
		</NavLinkGroup>

		<NavLinkGroup title="Developer Activity">
			<NavLink href="/terra/contracts">Contract Deployments</NavLink>
		</NavLinkGroup>

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
