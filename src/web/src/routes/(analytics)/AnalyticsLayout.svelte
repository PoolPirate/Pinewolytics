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
			class="flex flex-col justify-between pointer-events-auto w-full h-full px-4 pt-16 pb-4
			   sidebar transparent-background xl:bg-transparent bg-black
			   [&>aside]:contents"
		>
			<slot name="sidebar" />
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
			class="relative flex justify-center w-full h-full h-full overflow-x-hidden overflow-y-auto"
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

	.transparent-background {
		position: relative;
	}

	.transparent-background::before {
		content: ' ';
		position: absolute;
		left: 0;
		right: 0;
		top: 0;
		bottom: 0;
		background: black;
		opacity: 75%;
		border-radius: inherit;
		pointer-events: none;
		z-index: -1;
		border-right: white 4px solid;
	}
</style>
