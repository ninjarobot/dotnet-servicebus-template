const Generator = require('@asyncapi/generator');
const path = require('path');
const { readFile } = require('fs').promises;

const {describe, test, expect} = require('@jest/globals');

describe("Template generation", () => {
    test('Generate deployment template from sb-end-to-end spec', async () => {
        const params = { location: 'eastus' };
        let outputDir =  path.resolve('tests', 'outputs')
        const generator = new Generator(path.normalize('./'), outputDir, { forceWrite:true, templateParams: params });
        await generator.generateFromFile(path.resolve('fsharp', 'AsyncApiServiceBusTests','sample-specs','sb-end-to-end.yml'));
        const deployment = await readFile(path.join(outputDir, 'deploy.json'), 'utf8');
        expect(deployment).toMatchSnapshot();
    });
});