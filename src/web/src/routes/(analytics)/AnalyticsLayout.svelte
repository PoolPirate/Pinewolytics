<script lang="ts">
	import Hamburger from '$lib/components/Hamburger.svelte';
	import ToggleSwitch from '$lib/components/ToggleSwitch.svelte';
	import { isWeeklyModeStoreName } from '$lib/utils/Utils';
	import { onMount, setContext } from 'svelte';
	import { writable } from 'svelte/store';
	import { screens } from 'tailwindcss/defaultTheme';

	var animateSidebar = false;

	var open: boolean = false;
	var forcedOpen: boolean = false;

	onMount(() => {
		open = window.innerWidth > Number.parseInt(screens.xl.split('px')[0]);
		forcedOpen = !window.matchMedia('(max-width: ' + screens.xl + ')').matches;

		window.matchMedia('(max-width: ' + screens.xl + ')').addEventListener('change', (result) => {
			if (forcedOpen && result.matches) {
				open = false;
			} else {
				open = open || !result.matches;
			}

			forcedOpen = !result.matches;
		});

		setTimeout(() => (animateSidebar = true));
	});

	function handleOuterClick() {
		if (open && !forcedOpen) {
			open = false;
		}
	}

	var isWeeklyMode: boolean = true;
	const weeklyModeStore = writable(isWeeklyMode);

	$: weeklyModeStore.set(isWeeklyMode);

	setContext(isWeeklyModeStoreName, weeklyModeStore);
</script>

<div class="fixed m-4 xl:hidden z-10">
	<Hamburger bind:open />
</div>

<div class="flex flex-row">
	<div class="fixed limit-shrink xl:relative pointer-events-none w-1/2 lg:w-1/4 h-screen ontop">
		<div
			class:open
			class:animateSidebar
			class="pointer-events-auto w-full h-full px-4 pb-4 relative flex flex-col border-r-4 border-white 
			   sidebar transparent-background xl:bg-transparent bg-black pt-16 sm:pt-0
			   before:opacity-75 before:bg-black "
		>
			<div class="top-0 h-14 pt-1">
				<slot name="sidebar-head" />
			</div>
			<div class="top-6 [&>aside]:contents flex flex-col h-full">
				<slot name="sidebar" />
			</div>
		</div>
	</div>

	<div
		class="flex flex-col w-full h-screen"
		on:click={() => handleOuterClick()}
		on:keydown={() => handleOuterClick()}
	>
		<div
			class:blurred={open && !forcedOpen}
			class="relative flex flex-row justify-around items-center h-14 navbar px-4 text-white overflow-hidden"
		>
			<div class="w-1/3 sm:w-full xl:hidden" />
			<div class="flex flex-row items-center">
				<p class="ml-auto xl:ml-0 mr-3">Daily</p>
				<ToggleSwitch bind:checked={isWeeklyMode} />
				<p class="ml-3">Weekly</p>
			</div>

			<slot name="nav" />
		</div>

		<main
			class:blurred={open && !forcedOpen}
			class="relative flex justify-center w-full h-full overflow-x-hidden overflow-y-auto"
		>
			<slot name="main" />
		</main>
	</div>
</div>

<style>
	.ontop {
		z-index: 5;
	}

	.blurred {
		filter: blur(4px);
		pointer-events: none;
	}

	.limit-shrink {
		min-width: 250px;
	}

	.navbar:before {
		content: ' ';
		display: block;
		position: absolute;
		background-color: black;
		width: 100%;
		height: 100%;
		opacity: 50%;
	}

	.sidebar {
		height: 100%;
		position: absolute;
		top: 0;
		left: -100%;
		overflow-x: hidden;
		z-index: 5;
	}

	.animateSidebar {
		transition: left 0.4s ease-in-out;
	}

	.open {
		left: 0;
	}
</style>
