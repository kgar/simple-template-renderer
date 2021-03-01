# dotnet Template Project Generator

Goal: Create a dotnet tool, a powershell script, or some similar facility that will copy template project, transform text in the template files, write the new files with the context applied, and add the newly-created project to an existing solution.  

## Why?

At my day job, there is at least one need for being able to feed in template projects and generate groups of projects with transformations done throughout all the files. We have a big project involving the creation of groups of projects over time, and over time, the structure of those projects will evolve. Additionally, the individual projects, after their initial creation, tend to grow and mutate as needs change. With those things said, I need some template-driven approach, whereby I can upgrade the template over time as needed and keep the automation code generating projects unchanged.  

This is the first time I've needed such a thing in my career so far, but I was having trouble finding something that fit this need, so I'm taking what's out there and making it happen.

I feel like it might help others, so I'm putting it all out here. Who knows, maybe someone else will need to automate things like generating multiple projects and adding them to a solution.

## To Do
- Create a test project with template tokens for replacement
  - In files
  - In file names
- Switch from console logging to actual file creation
- Apply error handling for edge cases
- Do validation better
  - What is needed for dotnet tool failure cases? Do you need something special for these command line tools to register failure with a build pipeline?
- Figure out how to detect read/write permissions early on, before the generator gets too far into processing
- Apply input validation
- Create tool documentation
  - help contents
  - XML doc comments
  - ...?
- 

