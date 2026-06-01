module Adversary

open Feedback

// I'm using a string as the groupBy key because F# arrays don't have
// structural equality but strings do. 0=gray, 1=yellow, 2=green — these
// values also happen to put gray < yellow < green in lex order, which
// is what the tie-break rule in R6 wants.
let private colorChar = function Gray -> '0' | Yellow -> '1' | Green -> '2'
let private charColor = function '0' -> Gray | '1' -> Yellow | _ -> Green

let private toKey (p: Pattern) =
    p |> Array.map colorChar |> System.String

let private fromKey (s: string) =
    s.ToCharArray() |> Array.map charColor

// counting occurrences of a char in a string
let private count ch (s: string) =
    let mutable n = 0
    for c in s do
        if c = ch then n <- n + 1
    n

// The actual adversarial logic. Given the player's guess and the
// current candidate set, group candidates by what feedback pattern
// they'd produce, then pick the biggest group. Ties broken by:
// fewer greens, then fewer yellows, then lex on the encoded key.
// (See R6.)
let chooseFeedback (guess: string) (candidates: string array) =
    let groups =
        candidates
        |> Array.groupBy (fun w -> Feedback.compute guess w |> toKey)

    // tuples compare lexicographically in F#, so I can stuff all the
    // sort criteria in one tuple. negate the size since I want largest
    // first but minBy picks smallest.
    let bestKey, bestWords =
        groups
        |> Array.minBy (fun (key, ws) ->
            let size = -ws.Length
            let g = count '2' key
            let y = count '1' key
            (size, g, y, key))

    fromKey bestKey, bestWords
