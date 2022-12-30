<div class="flex flex-col w-full h-screen">
	<div class="flex flex-row justify-around items-center h-12 bg-gray-600">
		<slot name="nav" />
	</div>

	<div class="w-full h-full bg-gray-400 h-full overflow-auto flex flex-col items-center">
		<main class="pad-around w-full h-full">
			<slot name="main" />
		</main>
	</div>
</div>
