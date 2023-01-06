<script lang="ts">
	export let href: string;
</script>

<a {href} class="p-2 rounded-lg hover:font-bold m-2">
	<slot />
</a>

<style>
	a {
		position: relative;
	}
	a::before {
		content: ' ';
		position: absolute;
		left: 0;
		right: 0;
		top: 0;
		bottom: 0;
		z-index: -1;
		background: var(--linkcolor);
		opacity: 80%;
		border-radius: inherit;
	}
</style>
