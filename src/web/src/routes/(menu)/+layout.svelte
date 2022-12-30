<div class="flex flex-col w-full h-screen">
	<nav class="flex flex-row justify-around items-center h-12 bg-gray-600">
		<a href="/osmosis" class="hover:text-sky-200">Osmosis</a>
		<a href="/terra" class="hover:text-sky-200">Terra</a>
	</nav>

	<main class="w-full h-full bg-gray-400 h-full overflow-auto flex flex-col items-center">
		<div class="pad-around w-full h-full">
			<slot />
		</div>
	</main>
</div>

<style>
	.pad-around {
	}
</style>
