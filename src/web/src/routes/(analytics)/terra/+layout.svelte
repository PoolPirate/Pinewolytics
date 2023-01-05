<script lang="ts" context="module">
	export const isWeeklyModeStoreName = 'weeklyModeStore';
</script>

<script lang="ts">
	import BackToMenuLink from '$lib/links/BackToMenuLink.svelte';
	import AnalyticsLayout from '../AnalyticsLayout.svelte';
	import luna2logo from '$lib/static/logo/luna2.png';
	import { beforeUpdate, setContext } from 'svelte';
	import NavLink from '$lib/links/NavLink.svelte';
	import LinkBumper from '$lib/links/LinkBumper.svelte';
	import ToggleSwitch from '$lib/components/ToggleSwitch.svelte';
	import { writable } from 'svelte/store';

	beforeUpdate(() => {
		const rootElement = document.querySelector(':root')! as any;

		rootElement.style.setProperty('--color1', '#f2e373');
		rootElement.style.setProperty('--color2', '#de3633');
	});

	var isWeeklyMode: boolean = true;
	const weeklyModeStore = writable(isWeeklyMode);

	$: weeklyModeStore.set(isWeeklyMode);

	setContext(isWeeklyModeStoreName, weeklyModeStore);
</script>

<AnalyticsLayout>
	<aside slot="sidebar">
		<NavLink href="/terra">Overview</NavLink>
		<NavLink href="/terra/supply/richlist">Richlist</NavLink>
		<NavLink href="/terra/transactions/speed">Speed</NavLink>
		<NavLink href="/terra/transactions/fees">Transaction Fees</NavLink>
		<NavLink href="/terra/wallets">Wallets</NavLink>
		<NavLink href="/terra/contracts">Contracts</NavLink>

		<LinkBumper />
		<NavLink href="/terra/about">About</NavLink>
		<BackToMenuLink />
	</aside>

	<nav slot="nav" class="h-full w-full flex flex-row items-center">
		<p class="ml-auto xl:ml-0 mr-3">Daily</p>
		<ToggleSwitch bind:checked={isWeeklyMode} />
		<p class="ml-3">Weekly</p>

		<h2 class="ml-auto font-bold text-xl">Terra</h2>
		<img src={luna2logo} alt="Luna 2.0" class="ml-6 max-w-full max-h-full p-1" />
	</nav>
	<div slot="main" class="flex flex-col p-3 w-full">
		<slot {isWeeklyMode} />
	</div>
</AnalyticsLayout>
