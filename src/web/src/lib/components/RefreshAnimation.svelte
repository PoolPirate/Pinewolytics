<script lang="ts" context="module">
	export enum RefreshAnimationType {
		Circle = 0,
		ArrowUp
	}
</script>

<script lang="ts">
	var svgElement: SVGSVGElement;

	export let type: RefreshAnimationType = RefreshAnimationType.Circle;

	export function play() {
		const newspaperSpinning = [
			{ transform: 'scale(0)' },
			{ transform: 'scale(1)' },
			{ transform: 'scale(0)' }
		];

		const newspaperTiming = {
			duration: 800,
			iterations: 1
		};

		svgElement.animate(newspaperSpinning, newspaperTiming);
	}
</script>

{#if type == RefreshAnimationType.Circle}
	<svg bind:this={svgElement} viewBox="0 0 100 100" xmlns="http://www.w3.org/2000/svg">
		<g color="green">
			<circle cx="50" cy="50" r="35" stroke="currentcolor" fill="lime" stroke-width="5" />
		</g>
	</svg>
{/if}
{#if type == RefreshAnimationType.ArrowUp}
	<svg bind:this={svgElement} viewBox="0 0 100 100" xmlns="http://www.w3.org/2000/svg">
		<g color="green">
			<circle cx="50" cy="50" r="35" stroke="currentcolor" fill="lime" stroke-width="5" />
		</g>
	</svg>
{/if}

<style>
	svg {
		opacity: 0.5;
		transform: scale(0);
	}
</style>
