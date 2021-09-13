# symbility-claim-creator
Automated tool to create &amp; assign claims to vendors in CoreLogic's Symbility claim management &amp; estimate system

## Requirements
* .NET Core 5.0
* Visual Studio 2019

## Projects
* MockDataUtils - Utility class library to mock data for creating a claim (addresses as of 9/13/21)
* SymbilityApiGenerator - CLI to generate the API services from Symbility's OpenAPI specification. Output is used in the SymbilityClaimAccess project using [NSwag](https://github.com/RicoSuter/NSwag)
* SymbilityClaimAccess - Class library to provide API access to Symbility
* SymbilityClaimCreator - Class library with claim creation logic
* SymbilityClaimCreator.Cli - CLI to create & assign claims using the SymbilityClaimCreator class library 

## Usage
### Debug
1. Create & populate `appsettings.Debug.json` with appropriate values, using `appsettings.json` as a template
2. More information about the formatting of "From-User-Specification" can be found in Symbility's REST API documentation
3. Run using Visual Studio 2019
### API Generator
*TODO: Usage details for the API Generator*
