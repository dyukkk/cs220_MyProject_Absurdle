module Feedback

type Color = Green | Yellow | Gray
type Pattern = Color array

// wordle feedback for `guess` against `secret`. Has to be two passes
// because of repeated letters: e.g. guess SPEED vs secret ABIDE, you
// can't mark both E's yellow, only one of them should be.
let compute (guess: string) (secret: string) : Pattern =
    let n = guess.Length
    let result = Array.create n Gray
    let used = Array.create n false  // secret positions already matched

    // pass 1: greens
    for i in 0 .. n - 1 do
        if guess.[i] = secret.[i] then
            result.[i] <- Green
            used.[i] <- true

    // pass 2: yellows. for each non-green position in the guess, look
    // for an unused matching letter anywhere in secret.
    for i in 0 .. n - 1 do
        if result.[i] = Gray then
            let mutable j = 0
            let mutable found = false
            while not found && j < n do
                if not used.[j] && secret.[j] = guess.[i] then
                    result.[i] <- Yellow
                    used.[j] <- true
                    found <- true
                j <- j + 1

    result

let isAllGreen (p: Pattern) =
    p |> Array.forall ((=) Green)
