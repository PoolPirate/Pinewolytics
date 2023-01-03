<div class="flex flex-col w-full h-screen">
	<div class="relative flex flex-row justify-around items-center h-14 navbar px-4 text-white">
		<slot name="nav" />
	</div>

	<main class="w-full h-full h-full overflow-x-hidden overflow-y-auto">
		<slot name="main" />
	</main>
</div>

<style>
	.navbar:before {
		content: ' ';
		display: block;
		position: absolute;
		background-color: black;
		width: 100%;
		height: 100%;
		opacity: 50%;
	}
</style>
