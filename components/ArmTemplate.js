function getTopics(channels) {
    let topics = new Set();
    Object.keys(channels).map((sub) => {
        topics.add(sub.split('/')[0])
    });
    return Array.from(topics);
}

function getResources(asyncapi, location, server) {
    let topics = getTopics(asyncapi.channels());
    let resources = [];
    let nsName = server.url().split('.')[0];
    let sku = server._json['x-azure-service-bus-sku'];
    const ns = {
        type: "Microsoft.ServiceBus/namespaces",
        apiVersion: "2017-04-01",
        location: location,
        name: nsName,
        sku: {
            name: sku,
            tier: sku
        }
    };
    resources.push(ns);
    topics.forEach((topicName) =>
        resources.push({
            type: "Microsoft.ServiceBus/namespaces/topics",
            apiVersion: "2017-04-01",
            dependsOn: [
                `[resourceId('Microsoft.ServiceBus/namespaces', '${nsName}')]`
            ],
            name: `${nsName}/${topicName}`
        })
    );
    Object.entries(asyncapi.channels()).forEach(([topicSubName, channel]) => {
            let [topic, sub] = topicSubName.split('/', 2);
            if (channel.hasSubscribe()) {
                let operationId = channel.subscribe().id();
                if (channel.hasBinding('amqp1')) {
                    let amqp1 = channel.binding('amqp1');
                    resources.push({
                        type: "Microsoft.ServiceBus/namespaces/topics/subscriptions",
                        apiVersion: "2017-04-01",
                        dependsOn: [
                            `[resourceId('Microsoft.ServiceBus/namespaces/topics', '${nsName}', '${topic}')]`
                        ],
                        name: `${nsName}/${topicSubName}`,
                        properties: {
                            operationId: operationId
                        },
                        resources: [
                            {
                                apiVersion: "2017-04-01",
                                dependsOn: [
                                    sub
                                ],
                                name: `on-${operationId}`,
                                properties: {
                                    correlationFilter: {
                                        properties: amqp1['x-azure-service-bus-headers']
                                    },
                                    filterType: "CorrelationFilter"
                                },
                                type: "Rules"
                            }
                        ]
                    })
                }
            }
        }
    )
    return resources;
}

export function ArmTemplate({asyncapi, location}) {
    let template = {
        $schema: "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
        metadata: {
            description: asyncapi.info().title()
        },
        contentVersion: "1.0.0.0",
        resources: getResources(asyncapi, location, asyncapi.servers()["dev"])
    }
    return JSON.stringify(template, null, 4)
}