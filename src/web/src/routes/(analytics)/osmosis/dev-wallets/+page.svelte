<script lang="ts">
	import type { OsmosisEpochInfoDTO } from '$lib/models/DTOs/OsmosisDTOs';
	import OsmosisCore from '$lib/static/osmosis-core.webp';
	import {
		genesisEpochProvisions,
		reductionFactor,
		reductionPeriodInEpochs
	} from '$lib/utils/OsmosisChainParams';
	import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
	import { onMount } from 'svelte';
	import { writable } from 'svelte/store';

	var connection: HubConnection;

	const epochInfo = writable<OsmosisEpochInfoDTO | null>(null);

	onMount(async () => {
		connection = new HubConnectionBuilder()
			.withUrl('/api/hub/osmosis')
			.withAutomaticReconnect()
			.build();

		connection.on('CurrentEpochInfo', (newEpochInfo) => {
			epochInfo.set(newEpochInfo);
		});

		await connection.start();
	});
</script>

<div class="grid-cols-1 w-full p-5 rounded-xl transparent-background mb-3">
	<h1 class="text-3xl text-center border p-2 rounded-lg bg-purple-400 font-bold">
		Osmosis - Developer Wallet Tracking
	</h1>

	<div class="p-4 grid grid-cols-1 gap-4">
		<div class="flex flex-col lg:flex-row gap-6">
			<div class="lg:w-2/3 bg-purple-400 p-5 rounded-3xl">
				<h2 class="font-bold text-2xl border-b-2 border-black pb-1 mb-1">About Osmosis</h2>

				<div class="p-2">
					<p>
						Osmosis is a DAPP - Blockchain built using the Cosmos SDK. The main purpose of the chain
						is to host the
						<a class="text-blue-700 font-bold" href="https://app.osmosis.zone/"
							>Osmosis Decentralized Exchange.</a
						>
						The chain has it's own token: <b>$OSMO</b> which is used for securing the chain, paying for
						gas and incentivizing liquidity providers.
					</p>
					<p class="mt-4">
						This site is going to give you a detailed overview over a specific section of the
						Tokenomics of $OSMO:
						<b>The Developer Vesting.</b>
					</p>
				</div>
			</div>
			<img src={OsmosisCore} alt="" class="lg:w-1/3 rounded-3xl" />
		</div>

		<h2 class="font-bold text-2xl text-center py-1 border-black border-b-2 border-t-2">
			Osmosis Tokenomics
		</h2>

		<div class="flex flex-col lg:flex-row gap-6">
			<img
				class="lg:w-1/2 rounded-3xl"
				src="https://miro.medium.com/max/720/0*hjY8neGuW_2qxKTa"
				alt=""
			/>
			<div class="lg:w-1/2 bg-purple-400 p-5 rounded-3xl flex flex-col gap-3">
				<p>As you can see in the attached image the Osmosis Tokenomics are fairly simple.</p>
				<ul class="list-inside list-disc font-black">
					<li>50M Airdrop</li>
					<li>50M Strategic Reserve</li>
					<li>5% Inflation Community Pool</li>
					<li>45% LP Incentives</li>
					<li>25% Staking Rewards</li>
					<li>25% Developer Vesting</li>
				</ul>
				<p>
					But especially in the recent months we have seen a lot of dicussions regarding the high
					$OSMO inflation. A decent chunk of LP incentives have been redirected to the Community
					Pool and there are ongoing discussions to reduce the overall emissions.
				</p>
				<p>
					The only category of inflation that was barely looked at is the Developer Vesting.
					Ususally the argument is that the Developers barely touch their tokens and therefore this
					inflation does not matter.
				</p>
				<b class="text-lg mt-4"
					>Explore the pages on the sidebar to find out if that is valid or not!</b
				>
			</div>
		</div>

		<h2 class="font-bold text-2xl text-center py-1 border-black border-b-2 border-t-2">
			How is $OSMO Minted & Distributed?
		</h2>

		<div class="bg-purple-400 p-5 rounded-3xl">
			<h2 class="font-bold text-2xl  border-b-2 border-black pb-1 mb-1">Epoch</h2>

			<div class="grid grid-cols-2 gap-6 p-3">
				<div class="flex flex-col gap-3">
					<p>
						Epochs are a feature allowing the Osmosis blockchain to execute certain tasks at a fixed
						interval.
					</p>
					<p>
						The main task of the daily epoch on Osmosis is $OSMO minting & reward distribution to
						liquidity provider wallets.
					</p>
					<p>
						Additionally the number of passed epochs is used to reduce $OSMO inflation over time.
						The amount of $OSMO minted per epoch is decreased by {100 - reductionFactor * 100}%
						every {reductionPeriodInEpochs} epochs.
					</p>
				</div>
				<div class="text-center grid grid-cols-1 gap-2">
					<div class="transparent-background p-2 rounded-lg flex flex-col justify-around">
						<p class="font-bold text-lg">Current Epoch</p>
						<p>{$epochInfo?.currentEpoch ?? 'Loading...'}</p>
					</div>
					<div class="transparent-background p-2 rounded-lg flex flex-col justify-around">
						<p class="font-bold text-lg">$OSMO minted per Epoch</p>
						{#if $epochInfo != null}
							<p>
								{(
									genesisEpochProvisions *
									Math.pow(
										reductionFactor,
										Math.floor($epochInfo.currentEpoch / reductionPeriodInEpochs)
									)
								).toLocaleString()} $OSMO
							</p>
						{:else}
							<p>Loading...</p>
						{/if}
					</div>
					<div class="transparent-background p-2 rounded-lg flex flex-col justify-around">
						<p class="font-bold text-lg">Epochs to next thirdening</p>
						{#if $epochInfo != null}
							<p>
								{reductionPeriodInEpochs - ($epochInfo.currentEpoch % reductionPeriodInEpochs)}
							</p>
						{:else}
							<p>Loading...</p>
						{/if}
					</div>
				</div>
			</div>
		</div>
	</div>
</div>
