import { sveltekit } from '@sveltejs/kit/vite';
import { defineConfig } from 'vite';

const config = {
	plugins: [sveltekit()]
};

const devConfig = {
	plugins: [sveltekit()],
	server: {
		host: '0.0.0.0',
		proxy: {
			'/Api': {
				target: 'http://127.0.0.1:4565',
				ws: true,
				changeOrigin: true
			},
			'/api': {
				target: 'http://127.0.0.1:4565',
				ws: true,
				changeOrigin: true
			}
		}
	}
};

export default defineConfig(({ command, mode }) => {
	const isDev = mode === 'development';

	return isDev ? devConfig : config;
});
