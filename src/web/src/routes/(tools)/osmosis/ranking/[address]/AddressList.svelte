<script lang="ts">
	export let assets: string[];
</script>

<ul>
	{#each assets as asset}
		<li>{asset}</li>
	{/each}
</ul>
