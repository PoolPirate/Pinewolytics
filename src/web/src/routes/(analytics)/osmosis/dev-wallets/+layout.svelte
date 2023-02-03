<script lang="ts">
	import AnalyticsLayout from '../../AnalyticsLayout.svelte';
	import osmologo from '$lib/static/logo/osmosis.png';
	import NavLink from '$lib/links/NavLink.svelte';
	import NavLinkGroup from '$lib/links/NavLinkGroup.svelte';
	import LinkBumper from '$lib/links/LinkBumper.svelte';
	import BackToMenuLink from '$lib/links/BackToMenuLink.svelte';
</script>

<AnalyticsLayout>
	<nav slot="nav" class="h-full w-full flex flex-row items-center">
		<h2 class="ml-auto font-bold text-xl">Osmosis</h2>
		<img src={osmologo} alt="Luna 2.0" class="ml-6 max-w-full max-h-full p-1" />
	</nav>
	<div class="flex justify-center items-center h-full lg:ml-8 xl:ml-0" slot="sidebar-head">
		<h1 class="text-white font-bold text-lg">Developer Wallet Tracking</h1>
	</div>
	<aside slot="sidebar">
		<NavLink href="/osmosis/dev-wallets">Overview</NavLink>
		<NavLink href="/osmosis/dev-wallets/wallets">Wallets</NavLink>
		<NavLink href="/osmosis/dev-wallets/movement">Movements</NavLink>

		<LinkBumper />
		<BackToMenuLink />
	</aside>
	<div slot="main" class="flex flex-col p-3 w-full">
		<slot />
	</div>
</AnalyticsLayout>
