<script lang="ts">
	import Hamburger from '$lib/components/Hamburger.svelte';
	import { onMount } from 'svelte';
	import { screens } from 'tailwindcss/defaultTheme';

	var animateSidebar = false;

	var open: boolean = false;
	var forcedOpen: boolean = false;

	onMount(() => {
		open = window.innerWidth > Number.parseInt(screens.xl.split('px')[0]);

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
</script>

<div class="fixed m-4 xl:hidden z-10">
	<Hamburger bind:open />
</div>

<div class="flex flex-row">
	<div class="fixed xl:relative pointer-events-none w-1/2 lg:w-1/4 h-screen ontop">
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

	<div class="flex flex-col w-full h-screen">
		<div class="relative flex flex-row justify-around items-center h-14 navbar px-4 text-white">
			<slot name="nav" />
		</div>

		<main class="flex justify-center w-full h-full h-full overflow-x-hidden overflow-y-auto">
			<slot name="main" />
		</main>
	</div>
</div>

<style>
	.ontop {
		z-index: 5;
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
		min-width: 250px;
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
