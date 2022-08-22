# Developer Overview

## Main folders

| Name | Content |
|------|---------|
| build | All build output from projects in _src_ folder |
| docs | The documentation you are reading right now |
| src | Contains the "real" projects (i.e. DeriSock and DeriSock.DevTools). |
| test | Contains test projects |

## DeriSock.DevTools

This project mainly contains the code generator used to generate nearly all the classes used in DeriSock.

It can:
- Parse the HTML from the [Deribit API Documentation](https://docs.deribit.com/) Website
- Transform the parse result using override files and store it as a JSON file.
- This JSON file, along with enumeration and object mapping files, is used to generate the source code.

Generated source code files end in `.g.cs`.

## DeriSock

The main part of this repository.

### Structure

#### Api

Contains Interfaces reflecting the Deribit API. It also contains implementations as nested private classes.

Interfaces and implementations are partial, so they can be customized.

snippet: example-customized-api-interface

snippet: example-customized-api-interface-impl

#### Constants

Contains some _constant_ values. For example `ErrorCode` informations.

#### Converter

Contains JSON Converters needed for deserializing responses.

#### JsonRpc

Contains JsonRpc message handling used by `DeribitClient`.

#### Model

Contains all types that are used for requests and responses. Those classes are all partial, so they can be customized.

snippet: example-customized-model-class

#### Utils

Contains internal utilities.

#### WebSocket

Contains WebSocket handling infrastructure.

#### DeribitClient.cs / DeribitClient_*.cs

This is the client itself, split up into the different method categories. The `Internal<Scope><Method>()` Methods contained in there is, what effectively gets called.