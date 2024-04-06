import multiInput from 'rollup-plugin-multi-input';
import terser from '@rollup/plugin-terser';

export default {
	input: ['./wwwroot/js/**/*.js'],
	output: {
		format: 'esm',
    dir: '.',
    preserveModules: true,
	},
  plugins: [ multiInput.default(), terser() ],
};
