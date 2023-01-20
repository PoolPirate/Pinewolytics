<script lang="ts">
	import { beforeUpdate } from 'svelte';

	beforeUpdate(() => {
		const rootElement = document.querySelector(':root')! as any;

		rootElement.style.setProperty('--color1', '#f200c9');
		rootElement.style.setProperty('--color2', '#0602bf');
	});
</script>

<slot />
