<script lang="ts">
	export let href: string;
</script>

<a {href} class="m-2 p-4 rounded-lg hover:font-bold">
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
		background: linear-gradient(45deg, var(--color1), var(--color2));
		opacity: 80%;
		border-radius: inherit;
	}
</style>
