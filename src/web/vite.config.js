import { sveltekit } from '@sveltejs/kit/vite';
import { defineConfig } from 'vite';

const config = {
	plugins: [sveltekit()]
};

const devConfig = {
	plugins: [sveltekit()],
	server: {
		proxy: {
			'/Api': {
				target: 'http://127.0.0.1:4565'
			}
		}
	}
};

export default defineConfig(({ command, mode }) => {
	const isDev = mode === 'development';

	return isDev ? devConfig : config;
});
