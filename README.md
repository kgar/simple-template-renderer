# dotnet Template Project Generator

Goal: Create a dotnet tool, a powershell script, or some similar facility that will copy template project, transform text in the template files, write the new files with the context applied, and add the newly-created project to an existing solution.  

## Why?

At my day job, there is at least one need for being able to feed in template projects and generate groups of projects with transformations done throughout all the files. We have a big project involving the creation of groups of projects over time, and over time, the structure of those projects will evolve. Additionally, the individual projects, after their initial creation, tend to grow and mutate as needs change. With those things said, I need some template-driven approach, whereby I can upgrade the template over time as needed and keep the automation code generating projects unchanged.  

This is the first time I've needed such a thing in my career so far, but I was having trouble finding something that fit this need, so I'm taking what's out there and making it happen.

I feel like it might help others, so I'm putting it all out here. Who knows, maybe someone else will need to automate things like generating multiple projects and adding them to a solution.

## Work in progress

Whew, there's a lot to do, it feels like, for something so simple.

## Ideas

My initial approach is to do this:  
- Copying, transforming, and writing new files recursively: PowerShell
  - Powershell is really good at this particular kind of thing.
- Associating new projects with an existing solution: [`dotnet sln add project`](https://docs.microsoft.com/en-us/dotnet/core/tools/dotnet-sln)
  - After days of scouring the internet for "programmatically add project to solution", and after getting completely stuck staring at pages about EnvDTE 🤢 at least a hundred time, I stumbled upon .NET Core's simple CLI commands. I was both surprised and not surprised, as .NET Core has proven over time to provide really powerful command line tools for getting the job done.
- Distributing this feature: [`dotnet tool`](https://docs.microsoft.com/en-us/dotnet/core/tools/dotnet-tool-install)
  - Yeah, so after making my scripts, I need a way to quickly and easily pull down the feature and also ensure that the dotnet 5+ SDK is installed. That's where `dotnet tool` comes in. I can package up my scripts in a simple dotnet tool wrapper application, take in some args from the caller, and run my scripts. I've heard the distribution of dotnet tool is pretty simple (usually via NuGet), so I'm excited about that!
- Automating project generation and solution association: run the tool in Azure DevOps build pipelines! I have this absurd idea of creating a build with required fields for the settings variables needed, then
  - Commence the build in an adequately configured docker container
  - Call the dotnet tool with 
    - a list of the template project paths to generate from
    - a hashtable of settings to use in file transforms
    - a full path to the target solution
  - The tool passes these args through to the main script for handling the job
    - For each project
      - For each file, recursively,
        - Apply text transform to file contents and file path / name
        - Write new file to transformed file path, scoped to the root directory of the indicated SLN file
      - Call dotnet CLI for adding the newly generated project to the target solution
        - TODO: Figure out if build configuration can / should be adjusted here for a solution-configured project. For example, in the project where I want to use this tool, everything MUST compile in 64-bit, regardless of env.
  - The build should then run an update on the config dictionaries for the build environments, copying the template key-value-pairs and writing them as newlines in the file, but with a generated name, based on the settings
  - When the tool is done running, ensure the new projects compile
  - Have git cut a branch with a generated branch name, based on the build ID or something similar
  - Have git commit/push the branch with its changes and 🤞 create a PR in the target repository with a generated title and description based on the job
  - 🤞🤞🤞 Push a new build / release definition for the deployable
  - 🤞🤞🤞 Push a new branch build policy to `master` which filters down to the new projects and runs the new build when the project(s) are affected by future PRs
  - ...
  - Save hundreds of hours on the creation of projects in 2021 for this large-scale project.

Oof. 😰