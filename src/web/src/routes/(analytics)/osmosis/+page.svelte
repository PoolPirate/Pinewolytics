<script lang="ts">
	import { beforeUpdate } from "svelte";

	beforeUpdate(() => {
		const rootElement = document.querySelector(':root')! as any;

		rootElement.style.setProperty('--color1', '#f200c9');
		rootElement.style.setProperty('--color2', '#0602bf');

		rootElement.style.setProperty('--linkgroupcolor', '#f3570f');
		rootElement.style.setProperty('--linkcolor', '#D2573D');
	});
</script>

<div
	class="
	w-full h-screen p-8
	flex flex-col gap-8
	text-center
	overflow-y-auto
	"
>
	<div class="w-full h-full flex flex-col transparent-background p-4 rounded-3xl">
		<h2 class="text-2xl font-bold">Analytic Dashboards</h2>
		<hr class="m-2 border-gray-500 border-2" />
		<div
			class="w-full h-full 
				   grid grid-cols-1 md:grid-cols-3
				   place-items-center gap-4 p-2
				   [&>a]:bg-orange-100 [&>a]:w-full [&>a]:h-full [&>a]:flex [&>a]:justify-center [&>a]:items-center [&>a]:rounded-lg"
		>
			<a href="osmosis/dev-wallets">Dev Wallets</a>
			<a href="osmosis/protorev">Protorev Revenue</a>
		</div>
	</div>
	<div class="w-full h-full flex flex-col transparent-background p-4 rounded-3xl">
		<h2 class="text-2xl font-bold">Tooling</h2>
		<hr class="m-2 border-gray-500 border-2" />
		<div
			class="w-full h-full 
				   grid grid-cols-1 md:grid-cols-3
				   place-items-center gap-4 p-2
				   [&>a]:bg-orange-100 [&>a]:w-full [&>a]:h-full [&>a]:flex [&>a]:justify-center [&>a]:items-center [&>a]:rounded-lg"
		>
			<a href="osmosis/ranking">Wallet Ranking</a>
			<a href="osmosis/flipside-query-helpers">Flipside Query Helpers</a>
		</div>
	</div>
</div>
