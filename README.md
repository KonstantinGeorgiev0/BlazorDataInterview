# Blazor Data Project

## Smart applicator 
#### Set-up: 
Camera and an IPC (industrial PC) next to the production line.
Initially bought large IPC’s to make sure everything worked. 
#### Given: 
Measurement data of the IPC's over 2 months, such as max CPU usage, average per measurement moment, and capacity. 
#### ToDo: 
Use these measurements to define if there are IPC's that do not use their full capacity.

## Getting started
Run the command `dotnet build` to install all required packages.

To start the web page, go to folder `BlazorInterview` and run `dotnet watch` to start the webpack dev server. This server is reachable at `localhost:5157`
and autoreloads any changes you make in the code.

To test the project, go to folder `BlazorInterview.Tests` and run `dotnet test`. 
