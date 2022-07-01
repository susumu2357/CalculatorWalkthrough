#load "Types2.fsx"
open Types2

#load "CalculatorImplementation.fsx"
open CalculatorImplementation

// ================================================
// Implementation of CalculatorConfiguration
// ================================================
module CalculatorConfiguration =

    // A record to store configuration options
    // (e.g. loaded from a file or environment)
    type Configuration =
        { decimalSeparator: string
          divideByZeroMsg: string
          maxDisplayLength: int }

    let loadConfig () =
        { decimalSeparator = System.Globalization.CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalSeparator
          divideByZeroMsg = "ERR-DIV0"
          maxDisplayLength = 10 }

// ================================================
// Implementation of CalculatorServices
// ================================================
module CalculatorServices =
    open Types2
    open CalculatorConfiguration

    let updateDisplayFromDigit (config: Configuration) : UpdateDisplayFromDigit =
        fun (digit, display) ->

            // determine what character should be appended to the display
            let appendCh =
                match digit with
                | Zero ->
                    // only allow one 0 at start of display
                    if display = "0" then "" else "0"
                | One -> "1"
                | Two -> "2"
                | Three -> "3"
                | Four -> "4"
                | Five -> "5"
                | Six -> "6"
                | Seven -> "7"
                | Eight -> "8"
                | Nine -> "9"
                | DecimalSeparator ->
                    if display = "" then
                        // handle empty display with special case
                        "0" + config.decimalSeparator
                    else if display.Contains(config.decimalSeparator) then
                        // don't allow two decimal separators
                        ""
                    else
                        config.decimalSeparator

            // ignore new input if there are too many digits
            if (display.Length > config.maxDisplayLength) then
                display // ignore new input
            else
                // append the new char
                display + appendCh

    let getDisplayNumber: GetDisplayNumber =
        fun display ->
            match System.Double.TryParse display with
            | true, d -> Some d
            | false, _ -> None

    let setDisplayNumber: SetDisplayNumber = fun f -> sprintf "%g" f

    let setDisplayError divideByZeroMsg : SetDisplayError =
        fun f ->
            match f with
            | DivideByZero -> divideByZeroMsg

    let doMathOperation: DoMathOperation =
        fun (op, f1, f2) ->
            match op with
            | Add -> Success(f1 + f2)
            | Subtract -> Success(f1 - f2)
            | Multiply -> Success(f1 * f2)
            | Divide ->
                try
                    Success(f1 / f2)
                with
                | :? System.DivideByZeroException -> Failure DivideByZero

    let initState: InitState = fun () -> { display = ""; pendingOp = None }

    let createServices (config: Configuration) =
        { updateDisplayFromDigit = updateDisplayFromDigit config
          doMathOperation = doMathOperation
          getDisplayNumber = getDisplayNumber
          setDisplayNumber = setDisplayNumber
          setDisplayError = setDisplayError (config.divideByZeroMsg)
          initState = initState }


let config = CalculatorConfiguration.loadConfig ()
let services = CalculatorServices.createServices config
let initState = services.initState ()
let calculate = CalculatorImplementation.createCalculate services

let one = Digit One
let result = calculate (one, initState)
printfn "Result: %s" result.display

let plus = Op Add
let result2 = calculate (plus, result)

let two = Digit Two
let result3 = calculate (two, result2)
printfn "Result: %s" result3.display

let mul = Op Multiply
let result4 = calculate (mul, result3)

let four = Digit Four
let result5 = calculate (four, result4)

let eq = Action Equals
let result6 = calculate (eq, result5)
printfn "Result: %s" result6.display

module CalculatorTests =
    open Types2
    open System

    let config = CalculatorConfiguration.loadConfig ()
    let services = CalculatorServices.createServices config
    let calculate = CalculatorImplementation.createCalculate services

    let emptyState = services.initState ()

    /// Given a sequence of inputs, start with the empty state
    /// and apply each input in turn. The final state is returned
    let processInputs inputs =
        // helper for fold
        let folder state input = calculate (input, state)

        inputs |> List.fold folder emptyState

    /// Check that the state contains the expected display value
    let assertResult testLabel expected state =
        let actual = state.display

        if (expected <> actual) then
            printfn "Test %s failed: expected=%s actual=%s" testLabel expected actual
        else
            printfn "Test %s passed" testLabel

    let ``when I input 1 + 2, I expect 3`` () =
        [ Digit One
          Op Add
          Digit Two
          Action Equals ]
        |> processInputs
        |> assertResult "1+2=3" "3"

    let ``when I input 1 + 2 + 3, I expect 6`` () =
        [ Digit One
          Op Add
          Digit Two
          Op Add
          Digit Three
          Action Equals ]
        |> processInputs
        |> assertResult "1+2+3=6" "6"

    // run tests
    do
        ``when I input 1 + 2, I expect 3`` ()
        ``when I input 1 + 2 + 3, I expect 6`` ()

open CalculatorTests
let test1 = CalculatorTests.``when I input 1 + 2, I expect 3`` ()
let test2 = CalculatorTests.``when I input 1 + 2 + 3, I expect 6`` ()
