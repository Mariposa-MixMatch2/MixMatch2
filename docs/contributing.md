# Contributing Guidelines
1. Push changes to a separate branch, and make a PR into dev. Dev should contain upcoming features that are nearly or fully complete.
   1. Once enough features have been added, a PR will be made into main as a new release.
   2. When created a new branch, make a pr requesting the deletion of the CODEOWNERS file.
2. Follow MVVM style, or the code will not be accepted for a PR.
3. Code should be as efficient as possible.
   1. One should consider porting slow business logic to rust. 
4. All code should conform to the appropriate base types, and should be able to be added as a self-contained module.
