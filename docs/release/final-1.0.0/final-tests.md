# Final 1.0.0 Test Record

The full solution regression completed after the alpha repair. All four test assemblies passed, with no skipped or failed tests recorded in the final run.[1]

| Test assembly | Passed | Failed | Skipped |
| --- | ---: | ---: | ---: |
| Domain | 14 | 0 | 0 |
| Application | 9 | 0 | 0 |
| Infrastructure | 14 | 0 | 0 |
| Desktop | 39 | 0 | 0 |
| **Total** | **76** | **0** | **0** |

The Desktop count includes the Flat Prismatic L regression contract. Its RED logs showed the contaminated image carried four interior background samples with alpha `255`; the final GREEN run confirms that transparent background and neutral-fringe assertions now pass.[2] [3]

## References

[1]: ./post-alpha-fix-full-regression.log "Post-alpha-fix full regression"
[2]: ./flat-prismatic-l-alpha-red.log "Initial alpha RED run"
[3]: ./flat-prismatic-l-fringe-green.log "Final alpha/fringe GREEN run"
