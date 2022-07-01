#load "Types2.fsx"

open Types2

let updateDisplayFromDigit services digit state =
    let newDisplay = services.updateDisplayFromDigit (digit, state.display)
    let newState = { state with display = newDisplay }
    newState //return

(*
    Imperative procedure.

    First, check if there is any pending op. If not, then do nothing.
    Next, try to get the current number from the display. If you can't, then do nothing.
    Next, run the op with the pending number and the current number from the display. If you get an error, then do nothing.
    Finally, update the display with the result and return a new state.
    The new state also has the pending op set to None, as it has been processed.
*)

let updateDisplayFromPendingOp services state =
    let getCurrentNumber (op, pendingNumber) =
        state.display
        |> services.getDisplayNumber
        |> Option.map (fun currentNumber -> (op, pendingNumber, currentNumber))

    let doMathOp (op, pendingNumber, currentNumber) =
        let result = services.doMathOperation (op, pendingNumber, currentNumber)

        let newDisplay =
            match result with
            | Success resultNumber -> services.setDisplayNumber resultNumber
            | Failure error -> services.setDisplayError error

        let newState =
            { display = newDisplay
              pendingOp = None }

        Some newState

    (*
    let ifNone defaultValue input =
        // just reverse the parameters!
        defaultArg input defaultValue

    let result =
        state.pendingOp
        |> Option.bind getCurrentNumber
        |> Option.bind doMathOp
        |> ifNone state
*)

    let result =
        state.pendingOp
        |> Option.bind getCurrentNumber
        |> Option.bind doMathOp
        |> defaultArg
        <| state

    result

(*
    Imperative procedure.

    Try to get the current number from the display. If you can't, then do nothing.
    Update the state with the op and current number.
*)
let addPendingMathOp services op state =
    let newStateWithPending currentNumber =
        let pendingOp = Some(op, currentNumber)
        { state with pendingOp = pendingOp }

    state.display
    |> services.getDisplayNumber
    |> Option.map newStateWithPending
    |> defaultArg
    <| state

let createCalculate (services: CalculatorServices) : Calculate =
    fun (input, state) ->
        match input with
        | Digit d ->
            let newState = updateDisplayFromDigit services d state
            newState //return
        | Op op ->
            let newState1 = updateDisplayFromPendingOp services state
            let newState2 = addPendingMathOp services op newState1
            newState2 //return
        | Action Clear ->
            let newState = services.initState ()
            newState //return
        | Action Equals ->
            let newState = updateDisplayFromPendingOp services state
            newState //return
