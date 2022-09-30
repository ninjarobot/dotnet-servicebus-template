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

let expectedTemplateWithSubscriptions = """{
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
    },
    {
      "apiVersion": "2017-04-01",
      "dependsOn": [
        "[resourceId('Microsoft.ServiceBus/namespaces', 'sbtest')]"
      ],
      "name": "sbtest/topic1",
      "properties": {},
      "type": "Microsoft.ServiceBus/namespaces/topics"
    },
    {
      "apiVersion": "2017-04-01",
      "dependsOn": [
        "[resourceId('Microsoft.ServiceBus/namespaces/topics', 'sbtest', 'topic1')]"
      ],
      "name": "sbtest/topic1/t1-sub1",
      "properties": {},
      "resources": [],
      "type": "Microsoft.ServiceBus/namespaces/topics/subscriptions"
    },
    {
      "apiVersion": "2017-04-01",
      "dependsOn": [
        "[resourceId('Microsoft.ServiceBus/namespaces/topics', 'sbtest', 'topic1')]"
      ],
      "name": "sbtest/topic1/t1-sub2",
      "properties": {},
      "resources": [],
      "type": "Microsoft.ServiceBus/namespaces/topics/subscriptions"
    },
    {
      "apiVersion": "2017-04-01",
      "dependsOn": [
        "[resourceId('Microsoft.ServiceBus/namespaces/topics', 'sbtest', 'topic1')]"
      ],
      "name": "sbtest/topic1/t1-sub3",
      "properties": {},
      "resources": [],
      "type": "Microsoft.ServiceBus/namespaces/topics/subscriptions"
    },
    {
      "apiVersion": "2017-04-01",
      "dependsOn": [
        "[resourceId('Microsoft.ServiceBus/namespaces', 'sbtest')]"
      ],
      "name": "sbtest/topic2",
      "properties": {},
      "type": "Microsoft.ServiceBus/namespaces/topics"
    },
    {
      "apiVersion": "2017-04-01",
      "dependsOn": [
        "[resourceId('Microsoft.ServiceBus/namespaces/topics', 'sbtest', 'topic2')]"
      ],
      "name": "sbtest/topic2/t2-sub1",
      "properties": {},
      "resources": [],
      "type": "Microsoft.ServiceBus/namespaces/topics/subscriptions"
    },
    {
      "apiVersion": "2017-04-01",
      "dependsOn": [
        "[resourceId('Microsoft.ServiceBus/namespaces/topics', 'sbtest', 'topic2')]"
      ],
      "name": "sbtest/topic2/t2-sub2",
      "properties": {},
      "resources": [],
      "type": "Microsoft.ServiceBus/namespaces/topics/subscriptions"
    }
  ]
}"""

let expectedTemplateWithHeaderSubscriptions = """{
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
    },
    {
      "apiVersion": "2017-04-01",
      "dependsOn": [
        "[resourceId('Microsoft.ServiceBus/namespaces', 'sbtest')]"
      ],
      "name": "sbtest/topic1",
      "properties": {},
      "type": "Microsoft.ServiceBus/namespaces/topics"
    },
    {
      "apiVersion": "2017-04-01",
      "dependsOn": [
        "[resourceId('Microsoft.ServiceBus/namespaces/topics', 'sbtest', 'topic1')]"
      ],
      "name": "sbtest/topic1/t1-sub0",
      "properties": {},
      "resources": [
        {
          "apiVersion": "2017-04-01",
          "dependsOn": [
            "t1-sub0"
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
    },
    {
      "apiVersion": "2017-04-01",
      "dependsOn": [
        "[resourceId('Microsoft.ServiceBus/namespaces', 'sbtest')]"
      ],
      "name": "sbtest/topic2",
      "properties": {},
      "type": "Microsoft.ServiceBus/namespaces/topics"
    },
    {
      "apiVersion": "2017-04-01",
      "dependsOn": [
        "[resourceId('Microsoft.ServiceBus/namespaces/topics', 'sbtest', 'topic2')]"
      ],
      "name": "sbtest/topic2/t2-sub1",
      "properties": {},
      "resources": [
        {
          "apiVersion": "2017-04-01",
          "dependsOn": [
            "t2-sub1"
          ],
          "name": "on-operation-poweroff-operationresource-host",
          "properties": {
            "correlationFilter": {
              "properties": {
                "operation": "poweroff",
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
let ``Generate empty service bus namespace`` () =
    let asyncApiDoc = AsyncApiDocument(Servers = Dictionary<_,_>())
    asyncApiDoc.Servers["sbtest"] <- AsyncApiServer(Protocol = "amqp1", Url = "sbtest.servicebus.windows.net", Sku = "standard")
    let template = asyncApiDoc.ArmTemplate "eastus"
    Assert.Equal(expectedTemplateNamespaceOnly, template)

[<Fact>]
let ``Generate service bus namespace with topics and subscriptions`` () =
    let asyncApiDoc = AsyncApiDocument(
      Servers = Dictionary<_,_>(),
      Channels = (
        [
          "topic1/t1-sub1", AsyncApiChannel()
          "topic1/t1-sub2", AsyncApiChannel()
          "topic1/t1-sub3", AsyncApiChannel()
          "topic2/t2-sub1", AsyncApiChannel()
          "topic2/t2-sub2", AsyncApiChannel()
        ] |> dict |> Dictionary)
    )
    asyncApiDoc.Servers["sbtest"] <- AsyncApiServer(Protocol = "amqp1", Url = "sbtest.servicebus.windows.net", Sku = "standard")
    let template = asyncApiDoc.ArmTemplate "eastus"
    Assert.Equal(expectedTemplateWithSubscriptions, template)

[<Fact>]
let ``Generate service bus namespace with header subscriptions`` () =
    let asyncApiDoc = AsyncApiDocument(
      Servers = Dictionary<_,_>(),
      Channels = (
        [
          "topic1/t1-sub0", AsyncApiChannel(
            Subscribe = AsyncApiOperation (
              Bindings = ([
                AsyncApiChannelBinding(
                  Amqp1 = Amqp1ChannelBinding(
                    Headers = (
                        [
                          "operation", "reset"
                          "operationresource", "host"
                        ] |> dict |> Dictionary
                      )
                    )
                  )
                ] |> ResizeArray)
              )
            )
          "topic2/t2-sub1", AsyncApiChannel(
            Subscribe = AsyncApiOperation (
              Bindings = ([
                AsyncApiChannelBinding(
                  Amqp1 = Amqp1ChannelBinding(
                    Headers = (
                        [
                          "operation", "poweroff"
                          "operationresource", "host"
                        ] |> dict |> Dictionary
                      )
                    )
                  )
                ] |> ResizeArray)
              )
            )
        ] |> dict |> Dictionary)
    )
    asyncApiDoc.Servers["sbtest"] <- AsyncApiServer(Protocol = "amqp1", Url = "sbtest.servicebus.windows.net", Sku = "standard")
    let template = asyncApiDoc.ArmTemplate "eastus"
    Assert.Equal(expectedTemplateWithHeaderSubscriptions, template)

