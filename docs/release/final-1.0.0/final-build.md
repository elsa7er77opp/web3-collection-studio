# Final 1.0.0 Build Record

The candidate build was produced from `Web3CollectionStudio.Desktop.csproj` as a self-contained `win-x64` publish. The published output root is `artifacts/Web3CollectionStudio-Final-win-x64`, and the Inno installer consumes that root directly. The build is local-first and contains no wallet, seed phrase, minting, blockchain, cloud upload, marketplace, payment, trading, or investment workflow.

| Build gate | Evidence | Status |
| --- | --- | --- |
| Release publish | `post-alpha-fix-publish.log` reports a successful `Release` self-contained `win-x64` publish. | PASS |
| Source identity contract | `FlatPrismaticLBrandContractTests` verifies Login, Splash, and MainWindow route to `/Assets/Brand/nft-studio-prismatic-l.png` and retain no globe/rotation identity code. | PASS |
| Alpha identity contract | The runtime PNG must expose transparent background samples and no opaque neutral-light checkerboard fringe. | PASS |

The Flat Prismatic L alpha repair was deliberately limited to the approved asset pipeline: the baked checkerboard pixels were converted to true alpha before crop and again after resize to remove interpolation residue. The mark geometry and violet/cobalt content were not redesigned in this repair.[1]

## References

[1]: ./flat-prismatic-l-alpha-regression.md "Flat Prismatic L Alpha Regression"
