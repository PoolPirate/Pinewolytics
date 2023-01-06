<script lang="ts">
	export let title: string;

	var expanded: boolean = false;

	var contentElement: HTMLElement;
</script>

<button on:click={() => (expanded = !expanded)} class="p-2 m-2 rounded-lg hover:font-bold">
	{title}
</button>

<div
	bind:this={contentElement}
	class="grid ml-3 max-h-0 overflow-hidden"
	style="max-height: {expanded ? contentElement.scrollHeight : 0}px;"
>
	<slot />
</div>

<style>
	button {
		position: relative;
	}
	button::before {
		content: ' ';
		position: absolute;
		left: 0;
		right: 0;
		top: 0;
		bottom: 0;
		z-index: -1;
		background: linear-gradient(0, var(--color1), var(--color2));
		opacity: 80%;
		border-radius: inherit;
	}

	div {
		transition: max-height 0.2s ease-in-out;
	}
</style>
