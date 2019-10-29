// Karma configuration file, see link for more information
// https://karma-runner.github.io/1.0/config/configuration-file.html

module.exports = function (config) {
  config.set({
    basePath: '',
    frameworks: ['jasmine', '@angular-devkit/build-angular'],
    plugins: [
      require('karma-jasmine'),
      require('karma-chrome-launcher'),
      require('karma-jasmine-html-reporter'),
      require('karma-coverage-istanbul-reporter'),
      require('karma-webpack'),
      require('karma-junit-reporter'),
      require('karma-sonarqube-unit-reporter'),
      require('@angular-devkit/build-angular/plugins/karma')
    ],
    client: {
      clearContext: false // leave Jasmine Spec Runner output visible in browser
    },
    coverageIstanbulReporter: {
      dir: require('path').join(__dirname, '../coverage'),
      reports: ['html', 'lcovonly', 'text-summary'],
      fixWebpackSourcePaths: true
    },
    reporters: ['progress', 'kjhtml', 'junit', 'sonarqubeUnit'],
    port: 9876,
    colors: true,
    logLevel: config.LOG_LOG,
    autoWatch: true,
    customLaunchers: {
      ChromeDebugging: {
        base: 'Chrome',
        flags: ['--remote-debugging-port=9222']
      }
    },
    preprocessors: {
      './src/test.ts': ['webpack']
    },
    browsers: ['ChromeDebugging'],
    singleRun: false,
    browserConsoleLogOptions: {
      level: 'log',
      terminal: true
    },
    tfsReporter: {
        outputDir: 'testresults',
        outputFile: 'testresults_${date}.xml'
    },
    sonarQubeUnitReporter: {
      sonarQubeVersion: 'LATEST',
      outputFile: 'ut_report.xml',
      useBrowserName: false
    }
  });
};
