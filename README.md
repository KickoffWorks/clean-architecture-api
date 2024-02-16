# clean-architecture-api
Repository with outter core libraries and presentation. Application library and Presentation API alongside Architectural, Integration and Unit tests.

## Objective

Our objective is to use an external library package for our core domain / infrastructure layers which originates from our clean-architecture-code repository.

https://github.com/KickoffWorks/clean-architecture-core

We'll do this by importing our private Github Package Registry source through the nuget.config and then add the dependencies to our Sample.Users solution projects.

## Setup

### Architecture

- Sample.Users
	- Sample.Users.Api (REST API)
	- Sample.Users.Application (for application logic, DTOs and endpoints)
		- Sample.Core (our external NuGet package)
		
- Sample.Users.Tests
	- Sample.Core.Tests
	- Sample.Core.Tests.Architecture
	- Sample.Core.Tests.Integration
	
### Installation

To install / use make sure you either clone, fork or use this repository as a template and set up your nuget.config file in the root folder of the repository.

Make sure all dependencies are installed and up to date. If you update your NuGet package on the other repository do not forget to update the version via NuGet package manager or by updating your Sample.Application.csproj file.

You can then build the solution, run the automatic tests and debug/test the main API. It uses the NuGet package's domain and infrastructure libraries.


