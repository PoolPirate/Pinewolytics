<script lang="ts">
	export let href: string;
</script>

<a {href} class="m-2 bg-gray-400 p-4">
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
		background: linear-gradient(45odeg, var(--color1), var(--color2));
		opacity: 80%;
	}
</style>
