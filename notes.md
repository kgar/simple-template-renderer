## Ideas

The plan, so far:  
- Copying, transforming, and writing new files recursively: C# latest
- Associating new projects with an existing solution: [`dotnet sln add project`](https://docs.microsoft.com/en-us/dotnet/core/tools/dotnet-sln)
  - After days of scouring the internet for "programmatically add project to solution", and after getting completely stuck staring at pages about EnvDTE 🤢 at least a hundred time, I stumbled upon .NET Core's simple CLI commands. I was both surprised and not surprised, as .NET Core has proven over time to provide really powerful command line tools for getting the job done.
- Distributing this feature: [`dotnet tool`](https://docs.microsoft.com/en-us/dotnet/core/tools/dotnet-tool-install)
  - I need a way to quickly and easily pull down the feature and also ensure that the dotnet 5+ SDK is installed. That's where `dotnet tool` comes in. I can package up my code in a simple dotnet tool console application, take in some args from the caller, and run the logic.
- Automating project generation and solution association: run the tool in Azure DevOps build pipelines! I have this absurd idea of creating a build with required fields for the settings variables needed, then
  - Commence the build in an adequately configured docker container
  - Call the dotnet tool with 
    - a project path to generate from
    - ❓ trying to decide how to submit the key value pairs...
      - a file path to a JSON data file with key-value pairs where the key is the template variable name and the value is what to put in its place
        - The main problem with this approach is that I may want to dynamically determine some of the configs... unless I just want to create the file on the spot when preparing to run the tool...
      - 🤨 a series of args that work out to key value key value key value
        - How would I automate this in a build pipeline...?
      - 🤔 a series of flags for -key "someKey" -value "some value" -key "some other key" -value "some other fabulous value!"
        - How would I automate this in a build pipeline...?
    - an optional output paths; else default to the same root as the existing project
  - The tool passes these args through to the main script for handling the job
    - For each project
      - For each file, recursively,
        - Apply text transform to file contents and file path / name
        - Write new file to transformed file path, scoped to the root directory of the indicated SLN file
  - Call dotnet CLI for adding the newly generated project to the target solution
    - TODO: Figure out if build configuration can / should be adjusted here for a solution-configured project. For example, in the project where I want to use this tool, everything MUST compile in 64-bit, regardless of env.
  - Repeat the project generation process for all target projects
  - The build should then run an update on the config dictionaries for the build environments, copying the template key-value-pairs and writing them as newlines in the file, but with a generated name, based on the settings
  - Ensure the new projects compile
  - Have git cut a branch with a generated branch name, based on the build ID or something similar
  - Have git commit/push the branch with its changes and 🤞 create a PR in the target repository with a generated title and description based on the job
  - 🤞🤞🤞 Push a new build / release definition for the deployable
  - 🤞🤞🤞 Push a new branch build policy to `master` which filters down to the new projects and runs the new build when the project(s) are affected by future PRs
  - ...
  - Save hundreds of hours on the creation of projects in 2021 for this large-scale project.

Oof. 😰