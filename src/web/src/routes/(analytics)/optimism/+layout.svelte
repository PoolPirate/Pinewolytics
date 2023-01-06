<script lang="ts">
	import BackToMenuLink from '$lib/links/BackToMenuLink.svelte';
	import AnalyticsLayout from '../AnalyticsLayout.svelte';
	import optimismLogo from '$lib/static/logo/optimism.svg';
	import { beforeUpdate, setContext } from 'svelte';
	import NavLink from '$lib/links/NavLink.svelte';
	import { writable } from 'svelte/store';
	import { isWeeklyModeStoreName } from '$lib/utils/Utils';
	import LinkBumper from '$lib/links/LinkBumper.svelte';
	import NavLinkGroup from '$lib/links/NavLinkGroup.svelte';

	beforeUpdate(() => {
		const rootElement = document.querySelector(':root')! as any;

		rootElement.style.setProperty('--color1', '#ffffff');
		rootElement.style.setProperty('--color2', '#fe0420');
	});

	var isWeeklyMode: boolean = true;
	const weeklyModeStore = writable(isWeeklyMode);

	$: weeklyModeStore.set(isWeeklyMode);

	setContext(isWeeklyModeStoreName, weeklyModeStore);
</script>

<AnalyticsLayout>
	<aside slot="sidebar">
		<NavLink href="/optimism">Overview</NavLink>

		<NavLinkGroup title="Transactions">
			<NavLink href="/optimism/transactions/speed">Speed</NavLink>
			<NavLink href="/optimism/transactions/fees">Fees</NavLink>
		</NavLinkGroup>

		<NavLinkGroup title="Wallets">
			<NavLink href="/optimism/wallets/activity">Activity</NavLink>
		</NavLinkGroup>

		<NavLinkGroup title={'OP Token'}>
			<NavLink href="/optimism/op-token">Overview</NavLink>
			<NavLink href="/optimism/op-token/richlist">#100 Richlist</NavLink>
			<NavLink href="/optimism/op-token/delegates">Delegates</NavLink>
		</NavLinkGroup>

		<NavLinkGroup title={'Developer Activity'}>
			<NavLink href="/optimism/developers/contracts">Contract Deployments</NavLink>
		</NavLinkGroup>

		<LinkBumper />
		<BackToMenuLink />
	</aside>

	<nav slot="nav" class="h-full w-full flex flex-row items-center">
		<h2 class="ml-auto font-bold text-xl">Optimism</h2>
		<img src={optimismLogo} alt="Optimism" class="ml-6 max-w-full max-h-full p-1" />
	</nav>
	<nav />
	<slot slot="main" />
</AnalyticsLayout>

<style>
</style>
