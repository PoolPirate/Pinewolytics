<div class="h-full flex flex-col justify-center items-center">
	<div class="bg-white p-4 rounded-xl lime-border bg-gray-300">
		<slot name="nav" />
		<hr class="h-kek mt-2 bg-black" />
		<slot name="main" />
	</div>
</div>

<style>
	.h-kek {
		height: 0.125rem;
	}

	.lime-border {
		border: 2px solid lime;
	}
</style>
