<div class="flex flex-col w-full h-screen">
	<nav class="flex flex-row justify-around items-center h-12">
		<a href="/osmosis" class="hover:text-sky-200">Osmosis</a>
		<a href="/terra" class="hover:text-sky-200">Terra</a>
		<a href="/optimism" class="hover:text-sky-200">Optimism</a>
	</nav>

	<main class="w-full h-full h-full overflow-auto flex flex-col items-center">
		<div class="w-full h-full">
			<slot />
		</div>
	</main>
</div>

<style>
</style>
