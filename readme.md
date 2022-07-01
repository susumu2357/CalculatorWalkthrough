# Calculator Walkthrough

For practice purpose, reproduce the results demonstrating in [the original article](https://swlaschin.gitbooks.io/fsharpforfunandprofit/content/posts/calculator-design.html).

Development environment:
- Windows 10
- .NET Framework 4.8
- F# 6.0
- F# Interactive 12.0.4.0
- VSCode + Ionide extension

## Overview
- First, execute `dotnet new console -lang "F#" -o calculator_walkthrough`
- Understand the domain, translate it into  the domain data types ("nouns") and the domain activities ("verbs")
- Draw State Machine diagram
  ![alt text](test.png "StateMachine")
- Implement a prototype, check if the domain assumption works, go back to the previous stage if it is needed
