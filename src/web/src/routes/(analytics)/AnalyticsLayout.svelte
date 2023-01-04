<script lang="ts">
	import Hamburger from '$lib/components/Hamburger.svelte';

	var open: boolean;
</script>

<div class="fixed m-4 ontopontop">
	<Hamburger bind:open />
</div>

<div class="pointer-events-none fixed w-1/2 lg:w-1/4 h-screen ontop">
	<div
		class:open
		class="pointer-events-auto sidebar ontop w-full h-full px-4 pt-16
		       [&>aside]:flex [&>aside]:flex-col
			   [&>*>a]:p-4 [&>*>a]:bg-gray-400 [&>*>a]:m-2"
	>
		<slot name="sidebar" />
	</div>
</div>

<div class="flex flex-col w-full h-screen">
	<div class="relative flex flex-row justify-around items-center h-14 navbar px-4 text-white">
		<slot name="nav" />
	</div>

	<div class="flex justify-center w-full h-full h-full overflow-x-hidden overflow-y-auto">
		<slot name="main" />
	</div>
</div>

<style>
	.ontop {
		z-index: 2;
	}
	.ontopontop {
		z-index: 3;
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
		background-color: black;
		overflow-x: hidden;
		transition: left 0.4s ease-in-out;
	}

	.open {
		left: 0;
	}
</style>
