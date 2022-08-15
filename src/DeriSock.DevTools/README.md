# Introduction

DeriSock DevTools is a set of tools intended to help in developing the DeriSock WebSocket API Client

## Overview

The DevTools currently consists of the following parts

### Documentation Parser/Generator

The `DeriSock.DevTools.ApiDoc.DocumentationBuilder` class can be used to parse the official documentation page from Deribit and generate a JSON document out of it.

It also comes with the `ApplyOverrides` Method which can apply transformations to the JSON.

Overrides are recognized based on the filename of the JSON: `<filename>.<number>.<name>.overrides.json`

Currently the following overrides exist:

- **0.fixes**: Fixes faulty values provided in the official documentation (e.g. 'underlying_index' in 'public/get_order_book' is a string, not a number)
- **1.spelling**: Fixes typos and such
- **2.extensions**: Adds types, descriptions and other information that is currently missing in the documentation
- **3.managedTypes**: Adds information about managed types to be used by the code generator

### Documentation Analyzer

The `DeriSock.DevTools.ApiDoc.DocumentationAnalyzer` class checks for structural errors in the documentation.

Currently the following checks are implemented:

- Find parameters with a type "array of objects" without data about the objects (`AnalysisType.MissingObjectArrayParams`)
- Find parameters with a "complex" type but without data about the object (`AnalysisType.MissingObjectParams`)
- Find parameters with a "complex" type but without a managed type defined (`AnalysisType.MissingManagedType`)

### Code Generator (TBD/WIP)

The `DeriSock.DevTools.CodeDom` namespace provides classes to generate the DeribitClient. At least substantial parts of it.

This is work in progress.