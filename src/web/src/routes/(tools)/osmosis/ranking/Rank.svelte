<script lang="ts">
	export let rank: number;

	let clazz: string = '';
	export { clazz as class };

	var rankColor =
		rank < 100
			? 'gold'
			: rank < 200
			? 'silver'
			: rank < 300
			? 'orange'
			: rank < 500
			? 'green'
			: rank < 1000
			? 'skyblue'
			: rank < 5000
			? 'blue'
			: rank < 10000
			? 'purple'
			: 'red';
</script>

<p style="color: {rankColor}" class={clazz}>#{rank}</p>
