namespace AsyncApiServiceBus.AsyncApi

open System
open System.Collections.Generic
open YamlDotNet.Serialization
open Farmer
open Farmer.Builders

module Model =
    [<AllowNullLiteral>]
    type AsyncApiContact () =
        member val Name : string = null with get, set
        member val Url : Uri = null with get, set
        member val Email : string = null with get, set

    [<AllowNullLiteral>]
    type AsyncApiLicense () =
        member val Name : string = null with get, set
        member val Url : Uri = null with get, set

    [<AllowNullLiteral>]
    type AsyncApiInfo() =
        member val Title : string = null with get, set
        member val Version : string = null with get, set
        member val Description : string = null with get, set
        member val TermsOfService : string = null with get, set
        member val Contact : AsyncApiContact = null with get, set
        member val License : AsyncApiLicense = null with get, set

    [<AllowNullLiteral>]
    type AsyncApiSchema () =
        [<YamlMember(Alias="$ref")>]
        member val Ref : string = null with get, set
        member val Type : string = null with get, set

    [<AllowNullLiteral>]
    type AsyncApiMessage () =
        [<YamlMember(Alias="$ref")>]
        member val Ref : string = null with get, set
        member val MessageId : string = null with get, set
        member val Headers : AsyncApiSchema = null with get, set
        member val Payload : obj = null with get, set
        member val Name : string = null with get, set
        member val Title : string = null with get, set
        member val Summary : string = null with get, set
        member val ContentType : string = null with get, set

    [<AllowNullLiteral>]
    type AsyncApiServer () =
        [<YamlMember(Alias="$ref")>]
        member val Ref : string = null with get, set
        member val Url : string = null with get, set
        member val Protocol : string = null with get, set
        member val ProtocolVersion : string = null with get, set
        member val Description : string = null with get, set
        /// Schema extension for Azure Service Bus SKU
        [<YamlMember(Alias="x-azure-service-bus-sku")>]
        member val Sku : string = null with get, set

    [<AllowNullLiteral>]
    type AsyncApiOperation () =
        [<YamlMember(Alias="$ref")>]
        member val Ref : string = null with get, set
        member val OperationId : string = null with get, set
        member val Summary : string = null with get, set
        member val Description : string = null with get, set
        //member val Bindings : string = null with get, set
        member val Message : AsyncApiMessage = null with get, set

    [<AllowNullLiteral>]
    type AsyncApiChannel () =
        [<YamlMember(Alias="$ref")>]
        member val Ref : string = null with get, set
        member val Description : string = null with get, set
        member val Servers : ResizeArray<string> = null with get, set
        member val Subscribe : AsyncApiOperation = null with get, set
        member val Publish : AsyncApiOperation = null with get, set

    [<AllowNullLiteral>]
    type AsyncApiComponents () =
        member val Schemas : Dictionary<string,AsyncApiSchema> = null with get, set
        member val Servers : Dictionary<string,AsyncApiServer> = null with get, set
        member val Channels : Dictionary<string,AsyncApiChannel> = null with get, set
        member val Messages : Dictionary<string,AsyncApiMessage> = null with get, set

    [<AllowNullLiteral>]
    type AsyncApiDocument () =
        static let deserializer =
            DeserializerBuilder()
                .WithNamingConvention(NamingConventions.CamelCaseNamingConvention.Instance)
                .IgnoreUnmatchedProperties()
                .Build()
        [<YamlMember(Alias="asyncapi")>]
        member val AsyncApi : string = null with get, set
        member val Id : string = null with get, set
        member val Info : AsyncApiInfo = Unchecked.defaultof<_> with get, set
        member val Servers : Dictionary<string,AsyncApiServer> = null with get, set
        member val Channels : Dictionary<string,AsyncApiChannel> = null with get, set
        member val Components : AsyncApiComponents = null with get, set
        static member Deserialize (yaml:string) : AsyncApiDocument =
            deserializer.Deserialize<AsyncApiDocument>(yaml)
        member this.ArmTemplate (loc:string) =
            let sb =
                serviceBus {
                    name (this.Servers |> Seq.head |> fun server -> server.Key)
                    sku (
                        this.Servers
                        |> Seq.head
                        |> fun server ->
                            match server.Value.Sku with
                            | sku when String.IsNullOrWhiteSpace sku -> ServiceBus.Sku.Basic
                            | sku when sku.Equals("basic", StringComparison.OrdinalIgnoreCase) -> ServiceBus.Sku.Basic
                            | sku when sku.Equals("standard", StringComparison.OrdinalIgnoreCase) -> ServiceBus.Sku.Standard
                            | sku when sku.Equals("premium", StringComparison.OrdinalIgnoreCase) -> ServiceBus.Sku.Premium ServiceBus.MessagingUnits.OneUnit
                            | _ -> ServiceBus.Sku.Basic
                        )
                }
            let deployment = arm {
                location (Location.Location loc)
                add_resources [
                    sb
                ]
            }
            deployment.Template |> Writer.toJson
