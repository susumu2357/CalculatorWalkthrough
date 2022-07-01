// Use tuple because both parameters are always needed and a tuple makes this clear.
// Don't want to be partially applying the input.
type Calculate = CalculatorInput * CalculatorState -> CalculatorState

// Temporal definition of CalculatorState
(*
and CalculatorState = { display: CalculatorDisplay }
*)

and CalculatorState =
    { display: CalculatorDisplay
      pendingOp: (CalculatorMathOp * Number) option }

and CalculatorDisplay = string

// Naive definition
(*
and CalculatorInput =
    | Zero
    | One
    | Two
    | Three
    | Four
    | Five
    | Six
    | Seven
    | Eight
    | Nine
    | DecimalSeparator
    | Add
    | Subtract
    | Multiply
    | Divide
    | Equals
    | Clear
*)

and CalculatorInput =
    | Digit of CalculatorDigit
    | Op of CalculatorMathOp
    | Action of CalculatorAction

and CalculatorDigit =
    | Zero
    | One
    | Two
    | Three
    | Four
    | Five
    | Six
    | Seven
    | Eight
    | Nine
    | DecimalSeparator

and CalculatorMathOp =
    | Add
    | Subtract
    | Multiply
    | Divide

and CalculatorAction =
    | Equals
    | Clear

and UpdateDisplayFromDigit = CalculatorDigit * CalculatorDisplay -> CalculatorDisplay

// Only binary operation is assumed.
and DoMathOperation = CalculatorMathOp * Number * Number -> MathOperationResult

and Number = float

and MathOperationResult =
    | Success of Number
    | Failure of MathOperationError

and MathOperationError = | DivideByZero


// The display could be "error" not a number string, so the output is optinal
type GetDisplayNumber = CalculatorDisplay -> Number option

type SetDisplayNumber = Number -> CalculatorDisplay

// New function
type SetDisplayError = MathOperationError -> CalculatorDisplay

// Clear button
type InitState = unit -> CalculatorState

// Mock services
type CalculatorServices =
    { updateDisplayFromDigit: UpdateDisplayFromDigit
      doMathOperation: DoMathOperation
      getDisplayNumber: GetDisplayNumber
      setDisplayNumber: SetDisplayNumber
      setDisplayError: SetDisplayError
      initState: InitState }