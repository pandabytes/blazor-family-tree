import multiInput from 'rollup-plugin-multi-input';
import terser from '@rollup/plugin-terser';
import nodeResolve from '@rollup/plugin-node-resolve';
import commonjs from "@rollup/plugin-commonjs";

const DistDir = './wwwroot/js';

export default {
  // Input is the output of tsc i.e the
  // unbundled & unminified JS files
  input: [`${DistDir}/**/*.js`],
  output: {
    format: 'esm',
    dir: DistDir,
  },
  plugins: [
    multiInput.default({ relative: DistDir }),
    terser(),
    commonjs(),
    nodeResolve()
  ],
};
