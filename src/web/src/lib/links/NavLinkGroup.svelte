<script lang="ts">
	export let title: string;

	var expanded: boolean = false;

	var contentElement: HTMLElement;
</script>

<button
	on:click={() => (expanded = !expanded)}
	class="flex flex-row items-center p-2 m-2 rounded-lg hover:font-bold"
>
	<p class="text-xl mr-4 transition-all duration-300" class:rotate-180={expanded}>▲</p>
	<p>
		{title}
	</p>
</button>

<div
	bind:this={contentElement}
	class="grid ml-3 max-h-0 overflow-hidden transition-all duration-300"
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
		background: var(--linkgroupcolor);
		opacity: 80%;
		border-radius: inherit;
	}

	button:hover::before {
		background: #f3570f;
	}
</style>
