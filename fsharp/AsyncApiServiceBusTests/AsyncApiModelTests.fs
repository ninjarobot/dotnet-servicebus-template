module AsyncApiModelTests

open System
open AsyncApiServiceBus.AsyncApi.Model
open Xunit
open YamlDotNet.Serialization

let StreetlightsOperationSecurity = IO.File.ReadAllText "sample-specs/streetlights-operation-security.yml"

[<Fact>]
let ``Deserialize spec`` () =
    let deserializer =
        DeserializerBuilder()
            .WithNamingConvention(NamingConventions.LowerCaseNamingConvention.Instance)
            .IgnoreUnmatchedProperties()
            .Build()
    let asyncApiDoc = deserializer.Deserialize<AsyncApiDocument>(StreetlightsOperationSecurity)
    Assert.Equal("Apache 2.0", asyncApiDoc.Info.License.Name)
    Assert.Equal(Uri "https://www.apache.org/licenses/LICENSE-2.0", asyncApiDoc.Info.License.Url)
