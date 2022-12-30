<script lang="ts" context="module">
	var rotation: number = 0;
</script>

<script lang="ts">
	import '../app.css';
	import pattern from '$lib/static/pattern.png';
	import { beforeUpdate, onMount } from 'svelte';

	onMount(tickRotation);

	function tickRotation() {
		const rootElement = document.querySelector(':root')! as any;
		rootElement.style.setProperty('--rotation', rotation);

		rotation++;
		setTimeout(tickRotation, 50);
	}

	beforeUpdate(() => {
		const rootElement = document.querySelector(':root')! as any;
		rootElement.style.setProperty('--color1', 'red');
		rootElement.style.setProperty('--color2', 'blue');
		rootElement.style.setProperty('--pattern', "url('" + pattern + "')");
	});
</script>

<div class="background">
	<div class="pattern">
		<div>
			<slot />
		</div>
	</div>
</div>

<style>
	.background {
		background: #4261cf;
		background: -moz-linear-gradient(
			calc(var(--rotation) * 1deg),
			var(--color1),
			0%,
			var(--color2) 100%
		);
		background: -webkit-gradient(
			linear,
			left bottom,
			right top,
			color-stop(0%, var(--color1)),
			color-stop(100%, var(--color2))
		);
		background: -webkit-linear-gradient(
			calc(var(--rotation) * 1deg),
			var(--color1) 0%,
			var(--color2) 100%
		);
		background: -o-linear-gradient(
			calc(var(--rotation) * 1deg),
			var(--color1) 0%,
			var(--color2) 100%
		);
		background: -ms-linear-gradient(
			calc(var(--rotation) * 1deg),
			var(--color1) 0%,
			var(--color2) 100%
		);
		background: linear-gradient(calc(var(--rotation) * 1deg), var(--color1) 0%, var(--color2) 100%);

		height: 100vh;
		width: 100vw;

		animation: rotate 120s infinite;
	}

	.pattern:before {
		content: ' ';
		display: block;
		position: absolute;
		left: 0;
		top: 0;
		width: 100%;
		height: 100%;
		opacity: 0.35;
		background-image: var(--pattern);
		background-size: 90% !important;
		background-repeat: repeat;
		background-position: 50% 0;
		pointer-events: none;
		z-index: 0;
	}

	.pattern {
		position: relative;
		height: 100vh;
		width: 100vw;
	}
</style>
