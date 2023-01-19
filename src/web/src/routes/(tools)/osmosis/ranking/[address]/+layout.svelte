<script lang="ts">
	import { beforeUpdate } from 'svelte';

	beforeUpdate(() => {
		const rootElement = document.querySelector(':root')! as any;

		rootElement.style.setProperty('--color1', '#f200c9');
		rootElement.style.setProperty('--color2', '#0602bf');
	});
</script>

<div class="h-full flex flex-col justify-center items-center">
	<div class="bg-white p-4 rounded-xl">
		<nav>
			<ul class="flex flex-row justify-around gap-4">
				<li><a>Ranking</a></li>
				<li><a>Checklist</a></li>
			</ul>
		</nav>
		<hr class="h-kek mt-2 bg-black" />
		<slot />
	</div>
</div>

<style>
	.h-kek {
		height: 0.125rem;
	}
</style>
