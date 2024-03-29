//import adapter from '@sveltejs/adapter-node';
import adapter from '@sveltejs/adapter-static';
import { vitePreprocess } from '@sveltejs/kit/vite';
import preprocess from 'svelte-preprocess';

/** @type {import('@sveltejs/kit').Config} */
const config = {
	// Consult https://kit.svelte.dev/docs/integrations#preprocessors
	// for more information about preprocessors
	preprocess: [
		preprocess({
			postcss: true
		}),
		vitePreprocess()
	],
	kit: {
		adapter: adapter({})
	}
};

export default config;
