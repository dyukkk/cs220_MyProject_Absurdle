module Program

open System

let private usage () =
    eprintfn "usage: dotnet run -- [--seed N] [--words path]"

[<EntryPoint>]
let main argv =
    // really basic arg parsing, just walks left to right
    let mutable seed : int option = None
    let mutable wordsPath = "words.txt"
    let mutable i = 0
    let mutable bad = false

    while not bad && i < argv.Length do
        match argv.[i] with
        | "--seed" when i + 1 < argv.Length ->
            match Int32.TryParse(argv.[i + 1]) with
            | true, n -> seed <- Some n; i <- i + 2
            | _ -> eprintfn "bad seed value: %s" argv.[i + 1]; bad <- true
        | "--words" when i + 1 < argv.Length ->
            wordsPath <- argv.[i + 1]
            i <- i + 2
        | "-h" | "--help" ->
            usage ()
            exit 0
        | other ->
            eprintfn "unknown argument: %s" other
            bad <- true

    if bad then
        usage ()
        exit 2

    let words =
        try Words.load wordsPath
        with ex ->
            eprintfn "%s" ex.Message
            exit 1

    if words.Length = 0 then
        eprintfn "no usable words in '%s'" wordsPath
        exit 1

    let rng =
        match seed with
        | Some s -> Random(s)
        | None -> Random()

    // banner + instructions
    printfn "=== ABSURDLE ==="
    printfn "Type a 5-letter word and press Enter to make a guess."
    printfn "There's no guess limit. The game is adversarial - it tries to"
    printfn "keep its options open as long as it can, while staying consistent"
    printfn "with the feedback it already gave you."
    printfn ""
    printfn "Commands:"
    printfn "  :quit    exit"
    printfn "  :reveal  give up and see one possible answer"
    printfn ""
    printfn "Loaded %d words from '%s'." words.Length wordsPath
    printfn ""

    // play-again loop
    let mutable keepGoing = true
    while keepGoing do
        match Game.run words rng with
        | Game.Won k ->
            printfn ""
            printfn "Solved in %d guesses!" k
        | Game.Revealed w ->
            printfn ""
            printfn "The answer could have been: %s" (w.ToUpperInvariant())
        | Game.Quit ->
            printfn "Goodbye."
            keepGoing <- false

        if keepGoing then
            printf "\nPlay again? (y/n): "
            let resp =
                match Console.ReadLine() with
                | null -> ""
                | s -> s.Trim().ToLowerInvariant()
            if resp = "y" then
                printfn ""
            else
                printfn "Goodbye."
                keepGoing <- false

    0
