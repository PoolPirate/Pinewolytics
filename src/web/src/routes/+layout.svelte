<script lang="ts" context="module">
	var rotation: number = 0;
</script>

<script lang="ts">
	import '../app.css';
	import favicon from '$lib/static/favicon.ico';
	import pattern from '$lib/static/pattern.png';
	import { beforeUpdate, onMount } from 'svelte';

	onMount(() => {
		tickRotation();

		const rootElement = document.querySelector(':root')! as any;
		rootElement.style.setProperty('--pattern', "url('" + pattern + "')");
	});

	function tickRotation() {
		const rootElement = document.querySelector(':root')! as any;
		rootElement.style.setProperty('--rotation', rotation);

		rotation += 1;
		setTimeout(tickRotation, 50);
	}

	beforeUpdate(() => {
		const rootElement = document.querySelector(':root')! as any;
		rootElement.style.setProperty('--color1', 'green');
		rootElement.style.setProperty('--color2', 'orange');
	});
</script>

<svelte:head>
	<link rel="icon" type="image/x-icon" href={favicon} />
</svelte:head>

<div class="background">
	<div class="pattern">
		<slot />
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
