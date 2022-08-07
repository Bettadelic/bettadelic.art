const path = require('path');

module.exports = {
  entry: './src/Store.ts',
  devtool: 'inline-source-map',
  mode: 'development',
  module: {
    rules: [
      {
        use: 'ts-loader',
        exclude: /node_modules/,
      },
    ],
  },
  resolve: {
    extensions: ['.tsx', '.ts', '.js'],
  },
  output: {
    filename: 'Store.js',
    path: path.resolve(__dirname, '../static/compiled_ts/'),
    library: "BettadelicStore"
  },
};