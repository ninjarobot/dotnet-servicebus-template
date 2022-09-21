module ServiceBusDeploymentTests

open System.Collections.Generic
open AsyncApiServiceBus.AsyncApi.Model
open Xunit

let expectedTemplateNamespaceOnly = """{
  "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
  "contentVersion": "1.0.0.0",
  "outputs": {},
  "parameters": {},
  "resources": [
    {
      "apiVersion": "2017-04-01",
      "dependsOn": [],
      "location": "eastus",
      "name": "sbtest",
      "sku": {
        "name": "Standard",
        "tier": "Standard"
      },
      "tags": {},
      "type": "Microsoft.ServiceBus/namespaces"
    }
  ]
}"""

[<Fact>]
let ``Generate empty service bus namespace`` () =
    let asyncApiDoc = AsyncApiDocument(Servers = Dictionary<_,_>())
    asyncApiDoc.Servers["sbtest"] <- AsyncApiServer(Protocol = "amqp1", Url = "sbtest.servicebus.windows.net", Sku = "standard")
    let template = asyncApiDoc.ArmTemplate "eastus"
    Assert.Equal(expectedTemplateNamespaceOnly, template)
