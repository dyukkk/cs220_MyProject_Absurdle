module Render

open Feedback

// ANSI background codes. I'm using 100 (bright black) for gray instead
// of 40 because plain "black on black" looks awful on dark terminals.
let private greenBg  = "\u001b[42;30m"
let private yellowBg = "\u001b[43;30m"
let private grayBg   = "\u001b[100;30m"
let private reset    = "\u001b[0m"

let private bg = function
    | Green -> greenBg
    | Yellow -> yellowBg
    | Gray -> grayBg

// One guess rendered as " A  B  C  D  E " with each letter on a colored
// background. The padding spaces inside the colored block make the
// tiles look like little squares.
let renderGuess (guess: string) (pattern: Pattern) =
    let sb = System.Text.StringBuilder()
    let up = guess.ToUpper()
    for i in 0 .. guess.Length - 1 do
        sb.Append(bg pattern.[i]) |> ignore
        sb.Append(' ') |> ignore
        sb.Append(up.[i]) |> ignore
        sb.Append(' ') |> ignore
        sb.Append(reset) |> ignore
        if i < guess.Length - 1 then
            sb.Append(' ') |> ignore
    sb.ToString()

// History is stored newest-first (consed onto the front of a list), so
// reverse before printing so older guesses appear on top.
let printHistory (history: (string * Pattern) list) =
    for (g, p) in List.rev history do
        printfn "%s" (renderGuess g p)
