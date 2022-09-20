export function Namespace({server}) {
    return `
    {
        "type": "Microsoft.ServiceBus/namespaces",
        "apiVersion": "2017-04-01",
        "location": "eastus",
        "name": "${ server }",
        "sku": {
            "name": "Standard",
            "tier": "Standard"
        }
    }`
}