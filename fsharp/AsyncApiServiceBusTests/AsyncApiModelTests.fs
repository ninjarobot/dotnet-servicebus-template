module AsyncApiModelTests

open System
open AsyncApiServiceBus.AsyncApi.Model
open Xunit

let StreetlightsOperationSecurity = IO.File.ReadAllText "sample-specs/streetlights-operation-security.yml"
let SbEndToEnd = IO.File.ReadAllText "sample-specs/sb-end-to-end.yml"

[<Fact>]
let ``Deserialize asyncapi sample spec`` () =
    let asyncApiDoc = AsyncApiDocument.Deserialize(StreetlightsOperationSecurity)
    // Info
    Assert.Equal("Apache 2.0", asyncApiDoc.Info.License.Name)
    Assert.Equal(Uri "https://www.apache.org/licenses/LICENSE-2.0", asyncApiDoc.Info.License.Url)
    // Servers
    Assert.Contains("test", asyncApiDoc.Servers.Keys)
    Assert.Equal("test.mykafkacluster.org:8092", asyncApiDoc.Servers["test"].Url)
    Assert.Equal("kafka-secure", asyncApiDoc.Servers["test"].Protocol)
    Assert.Equal("Test broker", asyncApiDoc.Servers["test"].Description)
    Assert.Contains("test_oauth", asyncApiDoc.Servers.Keys)
    Assert.Equal("test.mykafkacluster.org:8093", asyncApiDoc.Servers["test_oauth"].Url)
    Assert.Equal("kafka-secure", asyncApiDoc.Servers["test_oauth"].Protocol)
    Assert.Equal("Test port for oauth", asyncApiDoc.Servers["test_oauth"].Description)
    // Channels
    Assert.Contains("smartylighting.streetlights.1.0.event.{streetlightId}.lighting.measured", asyncApiDoc.Channels.Keys)
    Assert.Equal("The topic on which measured values may be produced and consumed.", asyncApiDoc.Channels["smartylighting.streetlights.1.0.event.{streetlightId}.lighting.measured"].Description)
    Assert.Equal("receiveLightMeasurement", asyncApiDoc.Channels["smartylighting.streetlights.1.0.event.{streetlightId}.lighting.measured"].Publish.OperationId)
    Assert.Equal("Inform about environmental lighting conditions of a particular streetlight.", asyncApiDoc.Channels["smartylighting.streetlights.1.0.event.{streetlightId}.lighting.measured"].Publish.Summary)
    Assert.Contains("smartylighting.streetlights.1.0.action.{streetlightId}.turn.on", asyncApiDoc.Channels.Keys)
    Assert.Equal("turnOn", asyncApiDoc.Channels["smartylighting.streetlights.1.0.action.{streetlightId}.turn.on"].Subscribe.OperationId)
    Assert.Contains("smartylighting.streetlights.1.0.action.{streetlightId}.turn.off", asyncApiDoc.Channels.Keys)
    Assert.Contains("smartylighting.streetlights.1.0.action.{streetlightId}.dim", asyncApiDoc.Channels.Keys)
    // Messages
    Assert.Contains("lightMeasured", asyncApiDoc.Components.Messages.Keys)
    Assert.Contains("Light measured", asyncApiDoc.Components.Messages["lightMeasured"].Title)
    Assert.Contains("Inform about environmental lighting conditions of a particular streetlight.", asyncApiDoc.Components.Messages["lightMeasured"].Summary)
    Assert.Contains("application/json", asyncApiDoc.Components.Messages["lightMeasured"].ContentType)

let sampleEndToEndTemplate = """{
  "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
  "contentVersion": "1.0.0.0",
  "outputs": {},
  "parameters": {},
  "resources": [
    {
      "apiVersion": "2017-04-01",
      "dependsOn": [],
      "location": "eastus",
      "name": "my-test-sbus",
      "sku": {
        "name": "Standard",
        "tier": "Standard"
      },
      "tags": {},
      "type": "Microsoft.ServiceBus/namespaces"
    },
    {
      "apiVersion": "2017-04-01",
      "dependsOn": [
        "[resourceId('Microsoft.ServiceBus/namespaces', 'my-test-sbus')]"
      ],
      "name": "my-test-sbus/topic1",
      "properties": {},
      "type": "Microsoft.ServiceBus/namespaces/topics"
    },
    {
      "apiVersion": "2017-04-01",
      "dependsOn": [
        "[resourceId('Microsoft.ServiceBus/namespaces/topics', 'my-test-sbus', 'topic1')]"
      ],
      "name": "my-test-sbus/topic1/sub1",
      "properties": {},
      "resources": [
        {
          "apiVersion": "2017-04-01",
          "dependsOn": [
            "sub1"
          ],
          "name": "on-operation-reset-operationresource-host",
          "properties": {
            "correlationFilter": {
              "properties": {
                "operation": "reset",
                "operationresource": "host"
              }
            },
            "filterType": "CorrelationFilter"
          },
          "type": "Rules"
        }
      ],
      "type": "Microsoft.ServiceBus/namespaces/topics/subscriptions"
    }
  ]
}"""

[<Fact>]
let ``Deserialize end to end sample`` () =
    let asyncApiDoc = AsyncApiDocument.Deserialize(SbEndToEnd)
    let template = asyncApiDoc.ArmTemplate "eastus"
    Assert.Equal(sampleEndToEndTemplate.ReplaceLineEndings(), template)
    ()
