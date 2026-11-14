# Web3 Collection Studio

A local desktop studio prepares layered PNG collections. A project manifest captures ordered layers, traits, weights and compatibility rules. The workstation renders local images and exports metadata for avatar, badge and digital-art profiles.

## Engineering scope

The Final release decision records a regression rerun, local workflow acceptance and launch through CyberBank Panel. The limitations record explains the unverified splash capture and limits on interruption during synchronous image processing. Wallet, minting and publishing operations are outside the application scope.

## Getting started

Use the .NET 10 SDK on Windows. From the repository root, build with `dotnet build Web3CollectionStudio.slnx`. To run the isolated domain suite, use `dotnet test tests/Web3CollectionStudio.Domain.Tests/Web3CollectionStudio.Domain.Tests.csproj`. Start the desktop application with `dotnet run --project src/Web3CollectionStudio.Desktop/Web3CollectionStudio.Desktop.csproj`.

## Status and verification

Final 1.0.0 documented.

14 domain tests passed locally. Installed UI acceptance and the documented 76-test release regression were not repeated.

## Source and evidence

- [LocalMetadataExporter.cs](src/Web3CollectionStudio.Infrastructure/Export/LocalMetadataExporter.cs)
- [release-decision.md](docs/release/final-1.0.0/release-decision.md)
- [known-limitations.md](docs/release/final-1.0.0/known-limitations.md)
- [panel-host-launch-validation.md](docs/release/final-1.0.0/panel-host-launch-validation.md)
- [WEB3_COLLECTION_STUDIO_FINAL_FULL_CHAT_REPORT_AR.md](docs/release/final-1.0.0/WEB3_COLLECTION_STUDIO_FINAL_FULL_CHAT_REPORT_AR.md)

## Author

Ahmed Mohamed Elsayed Elreefy. Engineering work includes requirements, technical decisions, AI-assisted implementation, debugging and acceptance review.
