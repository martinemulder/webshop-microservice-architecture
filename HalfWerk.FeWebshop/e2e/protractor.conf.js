// Protractor configuration file, see link for more information
// https://github.com/angular/protractor/blob/master/lib/config.ts

const { SpecReporter } = require('jasmine-spec-reporter');

exports.config = {
  allScriptsTimeout: 11000,
  specs: [
    './features/*.feature'
  ],
  capabilities: {
    'browserName': 'chrome',
    chromeOptions: {
      args: [
        '--no-sandbox'
      ]
    }
  },
  directConnect: true,
  baseUrl: 'http://localhost:4200/',

  // framework: 'jasmine',
  framework: 'custom',
  frameworkPath: require.resolve('protractor-cucumber-framework'),

  // cucumber command line options
  cucumberOpts: {
    // require step definition files before executing features
    require: ['./steps/*.ts'],
    // <string[]> (expression) only execute the features or scenarios with tags matching the expression
    // tags: [],
    // <boolean> fail if there are any undefined or pending steps
    strict: true,
    // <string[]> (type[:path]) specify the output format, optionally supply PATH to redirect formatter output (repeatable)
    format: [
      'json:e2e/reports/summary.json'
    ],
    // <boolean> invoke formatters without executing steps
    dryRun: false,
    // <string[]> ("extension:module") require files with the given EXTENSION after requiring MODULE (repeatable)
    compiler: []
  },
  onPrepare() {
    require('ts-node').register({
      project: require('path').join(__dirname, './tsconfig.e2e.json')
    });
  }
};
