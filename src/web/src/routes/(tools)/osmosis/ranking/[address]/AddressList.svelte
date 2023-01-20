<script lang="ts">
	export let assets: string[] | null;

	let clazz: string = '';
	export { clazz as class };
</script>

<div class={clazz}>
	{#if assets != null}
		<ul>
			{#each assets as asset}
				<li>{asset}</li>
			{/each}
		</ul>
	{:else}
		<p>Unknown</p>
	{/if}
</div>
