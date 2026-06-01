module Words

open System.IO

// load 5-letter lowercase words from a file. anything that isn't
// exactly 5 a-z letters gets dropped silently (blank lines, weird
// unicode, longer words, etc).
let load (path: string) =
    if not (File.Exists path) then
        failwithf "couldn't find word list: %s" path

    File.ReadAllLines path
    |> Array.map (fun s -> s.Trim().ToLowerInvariant())
    |> Array.filter (fun s ->
        s.Length = 5
        && s |> Seq.forall (fun c -> c >= 'a' && c <= 'z'))
    |> Array.distinct
    |> Array.sort
