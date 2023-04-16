<script lang="ts">
	import { writable } from 'svelte/store';
	import flipsideLogo from '$lib/static/logo/flipside.png';
	import apiLogo from '$lib/static/logo/api.png';
	import coingeckologo from '$lib/static/logo/coingecko.webp';
	import twitterlogo from '$lib/static/logo/twitter.png';
	import discordlogo from '$lib/static/logo/discord.png';
	import githubLogo from '$lib/static/logo/github.png';
	const bday = new Date('2001-09-16T00:00:00+01:00');

	const nextBDay =
		new Date(Date.now()) > new Date(new Date(Date.now()).getFullYear() + '-09-16T00:00:00+01:00')
			? new Date(new Date(Date.now()).getFullYear() + 1 + '-09-16T00:00:00+01:00')
			: new Date(new Date(Date.now()).getFullYear() + '-09-16T00:00:00+01:00');

	const remainingTime = writable<number>(0);
	updateRemainingTime();

	setTimeout(updateRemainingTime, 1000);

	function updateRemainingTime() {
		remainingTime.set(nextBDay.getTime() - new Date().getTime());
		setTimeout(updateRemainingTime, 1000);
	}
</script>

<div>
	<h1 class="text-3xl font-bold">About</h1>

	<h2 class="text-2xl font-bold">The Creator</h2>
	<b>Hey there!</b>
	<p>I'm Elias and</p>
	<ul class="list-disc list-inside m-2">
		<li>am known online by the name Playwo</li>
		<li>do anything regarding Crypto & Software Development</li>
		<li>
			gonna turn {nextBDay.getFullYear() - bday.getFullYear()} in {(
				$remainingTime / 1000
			).toFixed()} seconds
		</li>
	</ul>

	<h2 class="text-2xl font-bold">My Socials</h2>

	<div class="social-area">
		<img class="h-16" src={twitterlogo} alt="" />
		<div class="ml-6">
			<h3 class="text-xl">Twitter</h3>
			<p>Stay up to date to what I'm up to</p>
		</div>

		<a
			class="docsbutton ml-auto"
			href="https://twitter.com/Playwo_"
			rel="noreferrer"
			target="_blank">Goto Profile</a
		>
	</div>
	<div class="social-area">
		<img class="h-16" src={discordlogo} alt="" />
		<div class="ml-6">
			<h3 class="text-xl">Discord</h3>
			<p>The only place where you can get me to respond quickly!</p>
		</div>

		<h3 class="text-xl ml-auto">Playwo#9999</h3>
	</div>

	<h2 class="text-2xl font-bold">Data Sources</h2>

	<div class="datasource-area">
		<img class="h-16" src={flipsideLogo} alt="" />
		<div class="ml-6">
			<h3 class="text-xl">Flipside Crypto</h3>
			<p>Entire blockchain indexed & queryable using SQL</p>
		</div>
		<a
			class="docsbutton ml-auto"
			href="https://docs.flipsidecrypto.com/shroomdk-sdk/get-started"
			rel="noreferrer"
			target="_blank">Goto Docs</a
		>
	</div>
	<div class="datasource-area">
		<img class="h-16" src={apiLogo} alt="" />
		<div class="ml-6">
			<h3 class="text-xl">Luna Endpoints</h3>
			<p>Real-time blockchain data</p>
		</div>

		<a
			class="docsbutton ml-auto"
			href="https://phoenix-lcd.terra.dev/swagger/#/"
			target="_blank"
			rel="noreferrer">Goto Docs</a
		>
	</div>
	<div class="datasource-area">
		<img class="h-16" src={coingeckologo} alt="" />
		<div class="ml-6">
			<h3 class="text-xl">Coingecko</h3>
			<p>Price Data</p>
		</div>

		<a
			class="docsbutton ml-auto"
			href="https://www.coingecko.com/en/api/documentation"
			target="_blank"
			rel="noreferrer">Goto Docs</a
		>
	</div>

	<h2 class="text-2xl font-bold">Source Code</h2>
	<div class="datasource-area">
		<img class="h-16" src={githubLogo} alt="" />
		<div class="ml-6">
			<h3 class="text-xl">Github</h3>
			<p>Source Code Hosting</p>
		</div>

		<a
			class="docsbutton ml-auto"
			href="https://github.com/PoolPirate/Pinewolytics"
			target="_blank"
			rel="noreferrer">Goto Repo</a
		>
	</div>
</div>

<style>
	.datasource-area {
		display: flex;
		flex-direction: row;
		padding: 0.75rem;
		align-items: center;
		border: black 2px solid;
		margin-top: 1rem;
		border-radius: 1rem;
		background-color: var(--color2);
	}

	.social-area {
		display: flex;
		flex-direction: row;
		padding: 0.75rem;
		align-items: center;
		border: black 2px solid;
		margin-top: 1rem;
		border-radius: 1rem;
		background-color: var(--color2);
	}

	.docsbutton {
		border: black 2px solid;
		padding: 0.5rem;
		border-radius: 0.5rem;
		background-color: var(--color1);
	}
</style>
