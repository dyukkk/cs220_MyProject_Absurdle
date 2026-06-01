module Game

open System
open Feedback

type GameOutcome =
    | Won of int
    | Revealed of string
    | Quit

// read one line, trim, lowercase. If stdin is closed (EOF / Ctrl+D),
// pretend the user typed :quit so scripted runs don't hang.
let private readLine () =
    let line = Console.ReadLine()
    if isNull line then ":quit"
    else line.Trim().ToLowerInvariant()

// R4 only specifies two error messages, so I deliberately don't have
// a separate one for non-letter chars. e.g. "ab12!" is length 5 but
// not in the word set, so it gets "Not in word list."
let private validate (input: string) (wordSet: Set<string>) =
    if input.Length <> 5 then
        Error "Guess must be exactly 5 letters."
    elif not (Set.contains input wordSet) then
        Error "Not in word list."
    else
        Ok ()

let run (allWords: string array) (rng: Random) : GameOutcome =
    let wordSet = Set.ofArray allWords
    let mutable candidates = allWords
    let mutable guessNum = 1
    let mutable history : (string * Pattern) list = []
    let mutable outcome : GameOutcome option = None

    while outcome.IsNone do
        printf "Guess #%d: " guessNum
        let input = readLine ()

        match input with
        | ":quit" -> outcome <- Some Quit
        | ":reveal" ->
            // pick any word still in the candidate set
            let w = candidates.[rng.Next(candidates.Length)]
            outcome <- Some (Revealed w)
        | _ ->
            match validate input wordSet with
            | Error msg -> printfn "%s" msg
            | Ok () ->
                let pattern, newCands = Adversary.chooseFeedback input candidates
                candidates <- newCands
                history <- (input, pattern) :: history

                printfn ""
                Render.printHistory history

                // win condition: an all-green pattern can only happen
                // when the only candidate left IS the guessed word
                // (since any other word would produce some non-green
                // position somewhere), so this is equivalent to R8.
                if Feedback.isAllGreen pattern then
                    outcome <- Some (Won guessNum)
                else
                    printfn ""
                    printfn "Possible answers remaining: %d" candidates.Length
                    printfn ""
                    guessNum <- guessNum + 1

    outcome.Value
